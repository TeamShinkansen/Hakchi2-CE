# Hakchi2 CE on Linux

Hakchi2 CE is a Windows Forms app. On Linux it runs under [Mono](https://www.mono-project.com/).

**Most people should use the AppImage** from the [Releases page](https://github.com/TeamShinkansen/Hakchi2-CE/releases):
it includes Mono, so you only download it, make it executable and run it (see the [main README](../README.md)).
This guide covers the tarball, manual setup, troubleshooting and building.

Linux support is new and has had little testing with real consoles, so **try memboot / adding games and
syncing before you flash a custom kernel**, and keep your NAND backup safe.

## 1. Install dependencies

| Distro | Command |
| --- | --- |
| Arch, Manjaro, EndeavourOS, Omarchy | `sudo pacman -S mono libgdiplus libusb` |
| Debian, Ubuntu, Mint, Pop!_OS | `sudo apt install mono-complete libgdiplus libusb-1.0-0` |
| Fedora | `sudo dnf install mono-complete libgdiplus libusb1` (see note below) |

**Fedora:** Mono packaging has changed between Fedora releases. If `dnf` can't find `mono-complete`, use the
packages from the [Mono project's download page](https://www.mono-project.com/download/stable/) instead. Fedora
is untested so far; reports welcome.

Other distros: install Mono (including its WinForms library), libgdiplus and libusb 1.0 from your package manager.
Arch-based distros have been tested with a real console; Debian/Ubuntu are what the CI build runs on.

## 2. USB and network setup (once)

**Easiest:** just start hakchi (step 3) and plug in your console. If USB access or the console's network link
isn't set up yet, hakchi offers to do it and asks for your password once. You can also run it any time from
**Tools → Linux USB setup...**, or from a terminal:

```sh
sudo ./hakchi-setup.sh            # set everything up
sudo ./hakchi-setup.sh --uninstall  # undo it
```

It installs the udev rule below (or the plugdev variant on distros without systemd), adds the network profile
from step 4 (NetworkManager or systemd-networkd) and allows the console's address in ufw/firewalld. It's safe to
run more than once. The rest of this section and step 4 describe the same steps done by hand.

### Doing it by hand

```sh
sudo cp 70-hakchi.rules /etc/udev/rules.d/
sudo udevadm control --reload-rules && sudo udevadm trigger
```

Unplug and replug the console afterwards. (If you installed an older `99-hakchi.rules`, delete it:
it was ordered after systemd's `73-seat-late.rules`, so it never granted access.)

### Distros without systemd (Void, Alpine, Artix, Gentoo with OpenRC, …)

`70-hakchi.rules` uses `TAG+="uaccess"`, which only works with systemd-logind. Without it, grant access through
a group instead (log out and back in afterwards so the new group applies):

```sh
sudo groupadd -f plugdev
sudo usermod -aG plugdev "$USER"
echo 'SUBSYSTEM=="usb", ATTRS{idVendor}=="1f3a", ATTRS{idProduct}=="efe8", MODE="0660", GROUP="plugdev"' \
  | sudo tee /etc/udev/rules.d/70-hakchi.rules
sudo udevadm control --reload-rules && sudo udevadm trigger
```

Check with `groups` that `plugdev` is listed, then replug the console.

## 3. Run

Extract the archive somewhere you can write to (for example `~/hakchi2-ce`); settings and games are kept
next to the program. Then:

```sh
./hakchi.sh
```

The first run adds **Hakchi2 CE** to your desktop's app menu (for your user only), so from then on you can
start it from there. If you move the folder, the next run updates the entry. To remove it and stop it from
coming back: `./hakchi.sh --remove-desktop-entry` (undo with `./hakchi.sh --add-desktop-entry`).

## 4. Connect the console (USB network)

Once hakchi is installed on the console, it shows up as a USB network device (`rndis_host`, an interface
like `enp…u2`) and always uses the address **169.254.13.37**. Your PC needs an address on the same
link-local network, and your firewall must accept traffic from it.

Quick test (lasts until you unplug; replace the interface name with yours from `ip -br link`):

```sh
sudo ip addr add 169.254.1.2/16 dev enp193s0f3u2
ping -c 3 169.254.13.37
```

Permanent setup with NetworkManager (most desktops, including Omarchy):

```sh
sudo nmcli connection add type ethernet con-name hakchi match.driver rndis_host \
  ipv4.method link-local ipv6.method link-local connection.autoconnect-priority 10
```

With systemd-networkd instead, create `/etc/systemd/network/50-hakchi-usb.network`:

```ini
[Match]
Driver=rndis_host

[Network]
DHCP=yes
LinkLocalAddressing=ipv4
```

If a firewall is enabled (Omarchy ships with UFW on), allow the interface:

```sh
sudo ufw allow in on enp193s0f3u2
```

hakchi then goes from "Offline" to online within about 15 seconds.

## Notes and known limitations

- **Tiling window managers (Hyprland/Omarchy, i3, Sway):** if the main window stops responding, a dialog is
  probably open behind it or on another workspace. Find it with `hyprctl clients` (Hyprland), or make the
  main window float (Omarchy: **Super + T**).
- **Driver install button:** that's a Windows-only step. On Linux the udev rule above replaces it.
- **Still "Offline"?** Run hakchi with `./hakchi.sh /debug`: it logs `Found hakchi at its default USB address`
  when it reaches the console. If not, check `ping 169.254.13.37` and the firewall rule from step 4.
- **FTP / Telnet menu items** open with `xdg-open`. You can change the commands in `config.ini`
  (`FtpCommand`, `TelnetCommand`).
- **Readme / message-of-the-day panels** use plain text instead of the embedded browser.
- **SFROM Tool** is a Windows program and is not available.

## Building from source

Needs the .NET SDK (8 or newer), Mono (for `resgen`) and `wget`:

```sh
git submodule update --init --recursive
dotnet build hakchi_gui/hakchi_gui.csproj -c Release
cp linux/hakchi.sh linux/hakchi-setup.sh linux/hakchi2-ce.png linux/70-hakchi.rules linux/README-linux.md hakchi_gui/bin/Release/net48/
```
