#!/bin/sh
# One-time Linux setup for Hakchi2 CE, run as root (the app starts it through pkexec):
#  - udev rule so the logged-in user can open the console over USB (FEL mode / clovershell)
#  - network profile giving the console's USB network link (rndis_host) a link-local address
#  - firewall exception for the console's fixed address
# Safe to run more than once. Usage: hakchi-setup.sh [--user NAME] [--uninstall]

CONSOLE_IP=169.254.13.37
RULE=/etc/udev/rules.d/70-hakchi.rules
NM_CONNECTION=hakchi
NETWORKD_FILE=/etc/systemd/network/50-hakchi-usb.network

log() { echo "hakchi-setup: $*"; }
has() { command -v "$1" >/dev/null 2>&1; }
active() { has systemctl && systemctl is-active --quiet "$1" 2>/dev/null; }

# network interfaces created by a console in USB network mode
console_ifaces() {
  for dev in /sys/class/net/*; do
    driver="$(readlink -f "$dev/device/driver" 2>/dev/null)"
    [ "${driver##*/}" = rndis_host ] && echo "${dev##*/}"
  done
}

user=""
[ -n "${PKEXEC_UID:-}" ] && user="$(id -nu "$PKEXEC_UID" 2>/dev/null)"
[ -z "$user" ] && [ -n "${SUDO_USER:-}" ] && user="$SUDO_USER"
uninstall=0
while [ $# -gt 0 ]; do
  case "$1" in
    --user) user="$2"; shift ;;
    --uninstall) uninstall=1 ;;
  esac
  shift
done

if [ "$(id -u)" != 0 ]; then
  log "this must run as root (try: sudo $0)"
  exit 1
fi

if [ "$uninstall" = 1 ]; then
  for f in "$RULE" /etc/udev/rules.d/99-hakchi.rules; do
    [ -e "$f" ] && rm -f "$f" && log "removed $f"
  done
  has udevadm && udevadm control --reload-rules
  if has nmcli && nmcli -t -f NAME connection show 2>/dev/null | grep -qx "$NM_CONNECTION"; then
    nmcli connection delete "$NM_CONNECTION" >/dev/null && log "removed NetworkManager profile"
  fi
  if [ -f "$NETWORKD_FILE" ]; then
    rm -f "$NETWORKD_FILE" && log "removed systemd-networkd file"
    has networkctl && networkctl reload
  fi
  if has ufw && ufw status 2>/dev/null | grep -q "Status: active"; then
    ufw delete allow in from "$CONSOLE_IP" >/dev/null 2>&1 && log "removed ufw rule"
  fi
  if active firewalld; then
    firewall-cmd --permanent --zone=trusted --remove-source="$CONSOLE_IP/32" >/dev/null 2>&1
    firewall-cmd --reload >/dev/null 2>&1 && log "removed firewalld rule"
  fi
  log "done"
  exit 0
fi

# 1. USB access
if [ -d /run/systemd/system ]; then
  access='TAG+="uaccess"'
else
  # without systemd-logind, uaccess does nothing: grant access through a group instead
  has groupadd && groupadd -f plugdev
  if [ -n "$user" ] && has usermod && ! id -nG "$user" 2>/dev/null | tr ' ' '\n' | grep -qx plugdev; then
    usermod -aG plugdev "$user" && log "added $user to the plugdev group (log out and back in to apply)"
  fi
  access='GROUP="plugdev"'
fi
mkdir -p "${RULE%/*}"
if ! cat > "$RULE" <<EOF
# Hakchi2 CE: let the logged-in user talk to the NES/SNES Classic over USB (FEL mode and clovershell, 1f3a:efe8).
# Must sort before systemd's 73-seat-late.rules, which is what applies TAG+="uaccess".
SUBSYSTEM=="usb", ATTRS{idVendor}=="1f3a", ATTRS{idProduct}=="efe8", MODE="0660", $access
EOF
then
  log "could not write $RULE"
  exit 1
fi
rm -f /etc/udev/rules.d/99-hakchi.rules
log "installed $RULE"
if has udevadm; then
  udevadm control --reload-rules
  udevadm trigger --subsystem-match=usb --attr-match=idVendor=1f3a
fi

# 2. Console USB network link (the console itself always uses $CONSOLE_IP)
ifaces="$(console_ifaces)"
if active NetworkManager && has nmcli; then
  if ! nmcli -t -f NAME connection show 2>/dev/null | grep -qx "$NM_CONNECTION"; then
    if nmcli connection add type ethernet con-name "$NM_CONNECTION" match.driver rndis_host \
        ipv4.method link-local ipv6.method link-local connection.autoconnect-priority 10 >/dev/null 2>&1; then
      log "added NetworkManager profile '$NM_CONNECTION'"
    else
      # NetworkManager too old for match.driver: bind the profile to the interface that is plugged in now
      for iface in $ifaces; do
        nmcli connection add type ethernet con-name "$NM_CONNECTION" ifname "$iface" \
          ipv4.method link-local ipv6.method link-local >/dev/null && log "added NetworkManager profile for $iface"
        break
      done
    fi
  fi
  for iface in $ifaces; do
    nmcli connection up "$NM_CONNECTION" ifname "$iface" >/dev/null 2>&1 && log "activated $iface"
  done
elif active systemd-networkd; then
  mkdir -p "${NETWORKD_FILE%/*}"
  cat > "$NETWORKD_FILE" <<EOF
[Match]
Driver=rndis_host

[Network]
DHCP=yes
LinkLocalAddressing=ipv4
EOF
  log "installed $NETWORKD_FILE"
  networkctl reload
  for iface in $ifaces; do networkctl reconfigure "$iface"; done
else
  # no network manager to configure: set the address directly (lost when the console is unplugged)
  for iface in $ifaces; do
    ip link set "$iface" up
    ip addr add 169.254.1.2/16 dev "$iface" 2>/dev/null
    log "set a temporary address on $iface"
  done
fi

# 3. Firewall: accept traffic from the console (by address, so it works on any USB port)
if has ufw && ufw status 2>/dev/null | grep -q "Status: active"; then
  ufw allow in from "$CONSOLE_IP" comment 'hakchi console' >/dev/null && log "allowed $CONSOLE_IP in ufw"
fi
if active firewalld; then
  firewall-cmd --permanent --zone=trusted --add-source="$CONSOLE_IP/32" >/dev/null 2>&1
  firewall-cmd --reload >/dev/null 2>&1 && log "allowed $CONSOLE_IP in firewalld"
fi

log "done"
exit 0
