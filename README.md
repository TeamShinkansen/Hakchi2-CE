# Hakchi2 CE

Hakchi2 CE lets you add more games to your NES Classic Mini and SNES Classic Mini (and their Famicom/Super
Famicom variants). It runs on **Windows** and **Linux**.

## Download

Get the latest version from the **[Releases page](https://github.com/TeamShinkansen/Hakchi2-CE/releases)**:

| System | File |
| --- | --- |
| Windows | `hakchi2-ce-…-installer.exe` (or `…-portable.zip`) |
| Linux (any distro) | `Hakchi2_CE-x86_64.AppImage` |
| Linux (alternative) | `hakchi2-ce-linux.tar.gz` |

## Install on Windows

Run the installer, or unzip the portable version anywhere and start `hakchi.exe`.

## Install on Linux (AppImage — easiest)

The AppImage is a single file that includes everything Hakchi2 CE needs; there is nothing else to install.

1. Download `Hakchi2_CE-x86_64.AppImage` from the Releases page.
2. Allow it to run: right-click the file → **Properties** → **Permissions** → tick **Allow executing file as
   program** (wording varies by desktop), or in a terminal:
   ```sh
   chmod +x ~/Downloads/Hakchi2_CE-x86_64.AppImage
   ```
3. Double-click it. On first start it adds **Hakchi2 CE** to your app menu, so next time you can open it from there.
   Your settings and games are kept in `~/hakchi2`.
4. Plug in your console (USB cable to the PC) and switch it on. If USB access isn't set up yet, Hakchi2 CE asks
   to set it up for you — click **Yes** and enter your password once.

If the AppImage doesn't start, your system is probably missing FUSE, which AppImages use to open themselves:
`sudo apt install fuse3` (Ubuntu/Debian/Mint), `sudo pacman -S fuse3` (Arch/CachyOS/Omarchy),
`sudo dnf install fuse3` (Fedora). Or start it without FUSE:
`~/Downloads/Hakchi2_CE-x86_64.AppImage --appimage-extract-and-run`.

## Install on Linux (tarball)

If you prefer not to use the AppImage, install Mono and run the program from a folder:

```sh
sudo pacman -S --needed mono libgdiplus libusb                  # Arch, CachyOS, Omarchy
sudo apt install mono-complete libgdiplus libusb-1.0-0          # Debian, Ubuntu, Mint
tar -xzf hakchi2-ce-linux.tar.gz -C ~
~/hakchi2-ce/hakchi.sh
```

The first run adds it to your app menu. More details, including manual USB/network setup and troubleshooting,
are in **[the Linux guide](linux/README-linux.md)**.

## Using it

1. Click **Add more games** and pick your ROM files.
2. Plug in and switch on the console. When the bottom-left corner shows it's online, click
   **Synchronize selected games with mini**.
3. Unplug the console and play.

A console that has never been modded needs hakchi installed first (**Kernel → Install / Repair**, with the
console in FEL mode: hold RESET while switching it on). Back up your console from the **Kernel** menu before
doing that.

## Building

- **Windows:** open `hakchi_gui.sln` in Visual Studio, or see `.github/workflows/dotnet-desktop.yml`.
- **Linux:** see [Building from source](linux/README-linux.md#building-from-source); the AppImage is built by
  `linux/appimage/build-appimage.sh`.
