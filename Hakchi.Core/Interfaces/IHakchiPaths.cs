using System;
using System.Collections.Generic;
using System.Text;

namespace Hakchi.Core.Interfaces
{
    public interface IHakchiPaths
    {
        public string BaseDirectoryInternal { get; }
        public string BaseDirectoryExternal { get; }

        // Libretro Info
        public string InfoPath { get; }

        public string ArtPath { get; }
        public string PatchesPath { get; }
        public string FolderImagesPath { get; }
        public string UserModsPath { get; }
        public string SpineTemplatesPath { get; }
        public string ScreenshotsPath { get; }
        public string DumpPath { get; }
        public string MoonHashesPath { get; }

        #region Data
        public string DataPath { get; }
        public string RomFilesPath { get; }
        public string LatestHmodPath { get; }
        #endregion

        #region GamePaths
        public string GamesOriginalsPath { get; }
        public string GamesCachePath { get; }
        public string GamesPath { get; }
        public string GamesMdPath { get; }
        public string GamesSnesPath { get; }
        #endregion

        #region Config
        public string ConfigPath { get; }
        public string ConfigFilePath { get; }
        public string LegacyConfigFilePath { get; }
        public string GameGenieDatabasePath { get; }
        #endregion

        #region Folders
        public string FoldersPath { get; }
        public string FoldersFamicomPath { get; }
        public string FoldersShonenJumpPath { get; }
        public string FoldersSnesEurPath { get; }
        public string FoldersSnesUsaPath { get; }
        public string FoldersSuperFamicomPath { get; }
        public string FoldersMdJpnPath { get; }
        public string FoldersMdUsaPath { get; }
        public string FoldersMdEurPath { get; }
        public string FoldersMdAsiaPath { get; }
        #endregion

        #region Cache
        public string CachePath { get; }
        public string ReadmeCachePath { get; }
        public string MotdFilePath { get; }
        public string TheGamesDbCachePath { get; }
        #endregion

        #region SFROM Tool
        public string SfromToolPath { get; }
        public string SfromToolPatchesPath { get; }
        public string SfromToolExePath { get; }
        #endregion
    }
}
