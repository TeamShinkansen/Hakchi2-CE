﻿namespace com.clusterrr.hakchi_gui
{
    public static class BuiltInCores
    {
        public readonly static CoreCollection.CoreInfo[] List = new CoreCollection.CoreInfo[] {
            new CoreCollection.CoreInfo("clover-canoe-shvc-wr -rom") // canoe snes emulator
            {
                DefaultArgs = "--volume 100 -rollback-snapshot-period 600",
                Name = "Canoe",
                DisplayName = "Nintendo - Super Nintendo Entertainment System (Canoe)",
                SupportedExtensions = new string[] { ".sfrom", ".smc", ".sfc" },
                Systems = new string[] { "Nintendo - Super Famicom", "Nintendo - Super Nintendo Entertainment System" },
                Kind = CoreCollection.CoreKind.BuiltIn
            },
            new CoreCollection.CoreInfo("clover-kachikachi-wr") // kachikachi nes emulator
            {
                DefaultArgs = "--guest-overscan-dimensions 0,0,9,3 --initial-fadein-durations 10,2 --volume 75 --enable-armet",
                Name = "Kachikachi",
                DisplayName = "Nintendo - Nintendo Entertainment System (Kachikachi)",
                SupportedExtensions = new string[] { ".nes", ".fds", ".qd" },
                Systems = new string[] { "Nintendo - Nintendo Entertainment System", "Nintendo - Family Computer", "Nintendo - Family Computer Disk System" },
                Kind = CoreCollection.CoreKind.BuiltIn
            },
            new CoreCollection.CoreInfo("m2engage") // sega mini emulator
            {
                DefaultArgs = "",
                Name = "M2engage",
                DisplayName = "Sega - Genesis / Mega Drive (M2engage)",
                SupportedExtensions = new string[] { ".bin" },
                Systems = new string[] { "Sega - Mega Drive - Genesis" },
                Kind = CoreCollection.CoreKind.BuiltIn,
               
            },
            new CoreCollection.CoreInfo("hsqs") // exception core for hsqs image files
            {
                DefaultArgs = string.Empty,
                Name = "SquashFS",
                DisplayName = "SquashFS Image Mount",
                SupportedExtensions = new string[] { ".hsqs" },
                Systems = new string[] { "SquashFS" },
                Kind = CoreCollection.CoreKind.BuiltIn
            },
            new CoreCollection.CoreInfo("sh") // exception core for shell scripts
            {
                DefaultArgs = string.Empty,
                Name = "Bash",
                DisplayName = "Bash Script Runner",
                SupportedExtensions = new string[] { ".sh" },
                Systems = new string[] { "Shell Script" },
                Kind = CoreCollection.CoreKind.BuiltIn
            }
        };
    }
}
