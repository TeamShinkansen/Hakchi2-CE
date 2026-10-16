#!/bin/sh
# Launches Hakchi2 CE under Mono on Linux.
# Options: --remove-desktop-entry  remove the app menu entry and stop re-creating it
# Distribution packages ship their own menu entry and set HAKCHI_NO_DESKTOP_ENTRY=1.
here="$(cd "$(dirname "$0")" && pwd)"

config_dir="${XDG_CONFIG_HOME:-$HOME/.config}/hakchi2-ce"
desktop_file="${XDG_DATA_HOME:-$HOME/.local/share}/applications/hakchi2-ce.desktop"

# Tell the user about a problem in the terminal and, when started from the app menu, as a desktop notification.
fail() {
  echo "hakchi: $*" >&2
  if [ ! -t 2 ] && command -v notify-send >/dev/null 2>&1; then
    notify-send -i "$here/hakchi2-ce.png" "Hakchi2 CE" "$*"
  fi
  exit 1
}

if [ "${1:-}" = "--remove-desktop-entry" ]; then
  rm -f "$desktop_file"
  mkdir -p "$config_dir" && touch "$config_dir/no-desktop-entry"
  echo "hakchi: removed the app menu entry (run ./hakchi.sh --add-desktop-entry to bring it back)"
  exit 0
fi
if [ "${1:-}" = "--add-desktop-entry" ]; then
  rm -f "$config_dir/no-desktop-entry"
  shift
fi

# App menu entry for the current user; rewritten when this folder moves.
if [ -z "${HAKCHI_NO_DESKTOP_ENTRY:-}" ] && [ ! -e "$config_dir/no-desktop-entry" ] && ! grep -qF "Exec=\"$here/hakchi.sh\"" "$desktop_file" 2>/dev/null; then
  mkdir -p "${desktop_file%/*}"
  cat > "$desktop_file" <<EOF
[Desktop Entry]
Type=Application
Name=Hakchi2 CE
GenericName=NES/SNES Classic game manager
Comment=Add more games to your NES/SNES Classic Mini
Exec="$here/hakchi.sh"
Path=$here
Icon=$here/hakchi2-ce.png
Terminal=false
Categories=Game;
Keywords=nes;snes;famicom;classic;mini;hakchi;
EOF
  command -v update-desktop-database >/dev/null 2>&1 && update-desktop-database -q "${desktop_file%/*}" 2>/dev/null
  echo "hakchi: added Hakchi2 CE to your app menu"
fi

if ! command -v mono >/dev/null 2>&1; then
  fail "Mono was not found. Install Mono and libgdiplus first (see README-linux.md)."
fi

# Mono looks for libusb as "libusb-1.0.so", which most distros only ship in their -dev package.
# If only the runtime library (libusb-1.0.so.0) is installed, point Mono at it through a private symlink.
ldconfig_p() { PATH="$PATH:/sbin:/usr/sbin" ldconfig -p 2>/dev/null; }
if ! ldconfig_p | grep -q 'libusb-1\.0\.so '; then
  case "$(uname -m)" in
    x86_64) arch="x86-64" ;;
    aarch64) arch="AArch64" ;;
    *) arch="" ;;
  esac
  lib="$(ldconfig_p | grep 'libusb-1\.0\.so\.0 ' | grep -F "$arch" | sed 's/.*=> //' | head -n 1)"
  if [ -n "$lib" ]; then
    shim="${XDG_CACHE_HOME:-$HOME/.cache}/hakchi2-ce/lib"
    mkdir -p "$shim" && ln -sf "$lib" "$shim/libusb-1.0.so"
    export LD_LIBRARY_PATH="$shim${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
  else
    echo "hakchi: libusb-1.0 not found; USB (FEL/clovershell) connections will not work." >&2
  fi
fi

cd "$here" || exit 1
exec mono hakchi.exe "$@"
