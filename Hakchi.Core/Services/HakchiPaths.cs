using Hakchi.Core.Interfaces;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Xml;

namespace Hakchi.Core.Services
{
    internal class HakchiPaths : IHakchiPaths
    {
        public string BaseDirectoryInternal { get; private set; }
        public string BaseDirectoryExternal { get; private set; }
        public HakchiPaths(IAssemblyInfo assemblyInfo, ILaunchFlags launchFlags, ISpecialLocations specialLocations)
        {
            BaseDirectoryInternal = assemblyInfo.EntryAssemblyDirectory;
            BaseDirectoryExternal = launchFlags.IsPortable ? BaseDirectoryInternal : Path.Combine(specialLocations.Documents, "hakchi2");
        }

        // Libretro Info
        public string InfoPath => field ??= Path.Combine(BaseDirectoryExternal, "info");

        public string ArtPath => field ??= Path.Combine(BaseDirectoryExternal, "art");
        public string PatchesPath => field ??= Path.Combine(BaseDirectoryExternal, "patches");
        public string FolderImagesPath => field ??= Path.Combine(BaseDirectoryExternal, "folder_images");
        public string UserModsPath => field ??= Path.Combine(BaseDirectoryExternal, "user_mods");
        public string SpineTemplatesPath => field ??= Path.Combine(BaseDirectoryExternal, "spine_templates");
        public string ScreenshotsPath => field ??= Path.Combine(BaseDirectoryExternal, "screenshots");
        public string DumpPath => field ??= Path.Combine(BaseDirectoryExternal, "dump");
        public string MoonHashesPath => field ??= Path.Combine(BaseDirectoryExternal, "moon_hashes");

        #region Data
        public string DataPath => field ??= Path.Combine(BaseDirectoryExternal, "data");
        public string RomFilesPath => field ??= Path.Combine(DataPath, "romfiles.xml");
        public string LatestHmodPath => field ??= Path.Combine(DataPath, "hakchi-latest.hmod");
        #endregion

        #region GamePaths
        public string GamesOriginalsPath => field ??= Path.Combine(BaseDirectoryExternal, "games_originals");
        public string GamesCachePath => field ??= Path.Combine(BaseDirectoryExternal, "games_cache");
        public string GamesPath => field ??= Path.Combine(BaseDirectoryExternal, "games");
        public string GamesMdPath => field ??= Path.Combine(BaseDirectoryExternal, "games_md");
        public string GamesSnesPath => field ??= Path.Combine(BaseDirectoryExternal, "games_snes");
        #endregion

        #region Config
        public string ConfigPath => field ??= Path.Combine(BaseDirectoryExternal, "config");
        public string ConfigFilePath => field ??= Path.Combine(ConfigPath, "config.json");
        public string LegacyConfigFilePath => field ??= Path.Combine(ConfigPath, "config.ini");
        public string GameGenieDatabasePath => field ??= Path.Combine(ConfigPath, "GameGenieDB.xml");
        #endregion

        #region Folders
        public string FoldersPath => field ??= Path.Combine(ConfigPath, "folders");
        public string FoldersFamicomPath => field ??= Path.Combine(ConfigPath, "folders_famicom");
        public string FoldersShonenJumpPath => field ??= Path.Combine(ConfigPath, "folders_shonen_jump");
        public string FoldersSnesEurPath => field ??= Path.Combine(ConfigPath, "folders_snes_eur");
        public string FoldersSnesUsaPath => field ??= Path.Combine(ConfigPath, "folders_snes_usa");
        public string FoldersSuperFamicomPath => field ??= Path.Combine(ConfigPath, "folders_super_famicom");
        public string FoldersMdJpnPath => field ??= Path.Combine(ConfigPath, "folders_md_jpn");
        public string FoldersMdUsaPath => field ??= Path.Combine(ConfigPath, "folders_md_usa");
        public string FoldersMdEurPath => field ??= Path.Combine(ConfigPath, "folders_md_eur");
        public string FoldersMdAsiaPath => field ??= Path.Combine(ConfigPath, "folders_md_asia");
        #endregion

        #region Cache
        public string CachePath => field ??= Path.Combine(BaseDirectoryExternal, "cache");
        public string ReadmeCachePath => field ??= Path.Combine(CachePath, "readme_cache");
        public string MotdFilePath => field ??= Path.Combine(CachePath, "motd.md");
        public string TheGamesDbCachePath => field ??= Path.Combine(CachePath, "thegamesdb");
        #endregion

        #region SFROM Tool
        public string SfromToolPath => field ??= Path.Combine(BaseDirectoryInternal, "sfrom_tool");
        public string SfromToolPatchesPath => field ??= Path.Combine(SfromToolPath, "patches");
        public string SfromToolExePath => field ??= Path.Combine(SfromToolPath, "SFROM Tool.exe");
        #endregion

    }
}
