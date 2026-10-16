#!/bin/sh
# Builds a Hakchi2 CE AppImage that carries its own Mono runtime, so users only download and run one file.
# Needs: mono (runtime + WinForms + libgdiplus) and libusb-1.0 installed on the build machine, plus
# linuxdeploy and appimagetool (paths in $LINUXDEPLOY / $APPIMAGETOOL, default: on PATH).
# Usage: build-appimage.sh <built app folder (hakchi.exe + linux files)> <output .AppImage>
set -eu

app="$(cd "$1" && pwd)"
out="$2"
here="$(cd "$(dirname "$0")" && pwd)"
linuxdeploy="${LINUXDEPLOY:-linuxdeploy}"
appimagetool="${APPIMAGETOOL:-appimagetool}"

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT
appdir="$work/AppDir"
mkdir -p "$appdir/usr/bin" "$appdir/usr/lib/mono" "$appdir/usr/etc" "$appdir/opt"

# The app, run in non-portable mode (the AppImage is read-only): settings and games go to ~/hakchi2
cp -a "$app" "$appdir/opt/hakchi2-ce"
rm -rf "$appdir/opt/hakchi2-ce/config" "$appdir/opt/hakchi2-ce/games" "$appdir/opt/hakchi2-ce/cache"
rm -f "$appdir/opt/hakchi2-ce/"*.bat
touch "$appdir/opt/hakchi2-ce/nonportable.flag"

# Mono finds its assemblies and config relative to its own binary (../lib/mono, ../etc/mono)
cp -L "$(command -v mono)" "$appdir/usr/bin/mono"
cp -a /usr/lib/mono/4.5 /usr/lib/mono/gac "$appdir/usr/lib/mono/"
cp -a /etc/mono "$appdir/usr/etc/mono"

# Native libraries Mono and the app load by name at runtime (not linked, so linuxdeploy can't see them)
libs=""
for name in libmono-native.so libMonoPosixHelper.so libmono-btls-shared.so libgdiplus.so.0 libusb-1.0.so.0; do
  path="$(ldconfig -p | grep -F "$name " | grep -E 'x86-64|x86_64' | sed 's/.*=> //' | head -n 1)"
  [ -z "$path" ] && for dir in /usr/lib /usr/lib/x86_64-linux-gnu; do [ -e "$dir/$name" ] && path="$dir/$name"; done
  [ -n "$path" ] || { echo "missing $name" >&2; exit 1; }
  libs="$libs --library=$path"
done

cp "$here/AppRun" "$appdir/AppRun"
cp "$app/hakchi2-ce.png" "$appdir/hakchi2-ce.png"
cat > "$appdir/hakchi2-ce.desktop" <<EOF
[Desktop Entry]
Type=Application
Name=Hakchi2 CE
Comment=Add more games to your NES/SNES Classic Mini
Exec=AppRun
Icon=hakchi2-ce
Terminal=false
Categories=Game;
EOF

# Pull in the shared libraries those depend on (glibc and other base libraries stay on the host)
# shellcheck disable=SC2086
"$linuxdeploy" --appdir "$appdir" --executable "$appdir/usr/bin/mono" $libs \
  --desktop-file "$appdir/hakchi2-ce.desktop" --icon-file "$appdir/hakchi2-ce.png"
# Mono looks for libusb by its unversioned name
ln -sf libusb-1.0.so.0 "$appdir/usr/lib/libusb-1.0.so"

ARCH=x86_64 "$appimagetool" "$appdir" "$out"
