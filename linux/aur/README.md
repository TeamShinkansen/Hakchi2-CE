# AUR package: hakchi2-ce-bin

Packages the Linux build from this repository's GitHub releases. Installs the app to `/opt/hakchi2-ce`
(settings and games go to `~/hakchi2`), the `hakchi2-ce` command, a menu entry and the USB udev rule.

## Publishing a new version

1. **Release on GitHub.** Push a tag, e.g. `git tag v3.9.3 && git push origin v3.9.3`. CI builds Windows and
   Linux and creates a *draft* release with `hakchi2-ce-linux.tar.gz` attached. Open the release on GitHub and
   click **Publish release**; the AUR can't download from a draft.
2. **Update the package** (on an Arch machine, in a clone of the AUR repo):
   ```sh
   cp /path/to/Hakchi2-CE/linux/aur/{PKGBUILD,hakchi2-ce.sh,hakchi2-ce.desktop} .
   # set pkgver to the new version (without the "v"), reset pkgrel=1, set the Maintainer line
   updpkgsums                          # fills in the real checksum of the release tarball
   makepkg --printsrcinfo > .SRCINFO
   makepkg -si                         # build and install it locally to test
   ```
3. **Push to the AUR:**
   ```sh
   git add PKGBUILD .SRCINFO hakchi2-ce.sh hakchi2-ce.desktop
   git commit -m "Update to 3.9.3"
   git push
   ```

## First-time setup

```sh
git clone ssh://aur@aur.archlinux.org/hakchi2-ce-bin.git   # empty repo; creates the package on first push
```
Then follow steps 2 and 3. Cloning a name that doesn't exist yet is how new AUR packages are created.
