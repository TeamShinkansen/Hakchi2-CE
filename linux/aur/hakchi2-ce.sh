#!/bin/sh
# Hakchi2 CE launcher installed by the hakchi2-ce-bin package.
# The package provides the menu entry, so the launcher must not create a per-user one.
export HAKCHI_NO_DESKTOP_ENTRY=1
exec /opt/hakchi2-ce/hakchi.sh "$@"
