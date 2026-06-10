#pragma warning disable 0618
using com.clusterrr.hakchi_gui.Properties;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32.SafeHandles;
using SpineGen.DrawingBitmaps;
using SpineGen.JSON;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using TeamShinkansen.Scrapers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Hakchi.Core.Interfaces;
using Hakchi.Core;

namespace com.clusterrr.hakchi_gui
{
    static class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        private const int MY_CODE_PAGE = 437;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint OPEN_EXISTING = 0x3;
        public static string BaseDirectoryInternal { get; private set; }
        public static string BaseDirectoryExternal { get; private set; }
        public static bool ConsoleVisible { get; private set; } = false;
        public static bool isPortable = false;
        public static List<Stream> debugStreams = new List<Stream>();
        private static Dictionary<string, SpineTemplate<Bitmap>> _SpineTemplates;
        public static IReadOnlyDictionary<string, SpineTemplate<Bitmap>> SpineTemplates
        {
            get => _SpineTemplates;
        }

        private static List<IScraper> _Scrapers = new List<IScraper>();
        public static IReadOnlyList<IScraper> Scrapers
        {
            get => _Scrapers;
        }

        public static MultiFormContext FormContext;
        internal static TeamShinkansen.Scrapers.TheGamesDB.Scraper TheGamesDBAPI = null;
        static void SetupScrapers()
        {
            if (Resources.TheGamesDBKey != "")
            {
                TheGamesDBAPI = new TeamShinkansen.Scrapers.TheGamesDB.Scraper()
                {
                    ApiKey = Resources.TheGamesDBKey,
                    CachePath = Path.Combine(Program.BaseDirectoryExternal, "cache", "thegamesdb"),
                    ArtSize = TeamShinkansen.Scrapers.TheGamesDB.ArtSize.Thumb
                };

                _Scrapers.Add(TheGamesDBAPI);
                TeamShinkansen.Scrapers.TheGamesDB.API.TraceURLs = true;
            }
        }
        
        public static IServiceProvider ServiceProvider { get; private set; }
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHakchiServices(args)
                        .AddSingleton(new MultiFormContext())
                        .AddAttributedServices(typeof(Program).Assembly);
                });
        }

        private static IServiceProvider _services;

        public static T GetRequiredService<T>()
        {
            return _services.GetRequiredService<T>();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Start();
            _services = host.Services;
            var formContext = FormContext = GetRequiredService<MultiFormContext>();
            var launchFlags = GetRequiredService<ILaunchFlags>();
            var launchArguments = GetRequiredService<ILaunchArguments>();
            var hakchiPaths = GetRequiredService<IHakchiPaths>();

            BaseDirectoryExternal = hakchiPaths.BaseDirectoryExternal;
            BaseDirectoryInternal = hakchiPaths.BaseDirectoryInternal;
            isPortable = launchFlags.IsPortable;

            var stdout = Console.OpenStandardOutput();

            int versionFormatArgIndex;
            if (!string.IsNullOrEmpty(launchFlags.VersionFormat))
            {
                Stream versionStream = null;

                if (!string.IsNullOrEmpty(launchFlags.VersionFile))
                {
                    versionStream = File.Create(launchFlags.VersionFile);
                } 
                else
                {
                    versionStream = stdout;
                }

                using (var writer = new StreamWriter(versionStream))
                {
                    writer.Write(String.Format(launchFlags.VersionFormat, Shared.AppDisplayVersion));
                    writer.Flush();
                }
                
                return;
            }

            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(stdout));
            
#if !DUMPER
            if (launchFlags.IsDebug)
#endif
            {
                try
                {
                    AllocConsole();
                    IntPtr stdHandle = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);
                    SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
                    FileStream consoleFileStream = new FileStream(safeFileHandle, FileAccess.Write);
                    Encoding encoding = System.Text.Encoding.GetEncoding(MY_CODE_PAGE);
                    StreamWriter standardOutput = new StreamWriter(consoleFileStream, encoding);
                    standardOutput.AutoFlush = true;
                    Console.SetOut(standardOutput);
                    debugStreams.Add(consoleFileStream);
                    System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(System.Console.Out));
                    ConsoleVisible = true;
                }
                catch { }
                try
                {
                    Stream logFile = File.Create("debuglog.txt");
                    debugStreams.Add(logFile);
                    System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(logFile));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                }
                Debug.AutoFlush = true;
            }
#if TRACE
            try
            {
                MemoryStream inMemoryLog = new MemoryStream();
                debugStreams.Add(inMemoryLog);
                System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(new StreamWriter(inMemoryLog, System.Text.Encoding.GetEncoding(MY_CODE_PAGE))));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
            }
            Trace.AutoFlush = true;
#endif

            bool isFirstRun = false;
            
            if (!isPortable)
            {
                isFirstRun = Shared.isFirstRun();
            }

            try
            {
                bool createdNew = true;
                using (Mutex mutex = new Mutex(true, "hakchi2", out createdNew))
                {
                    if (createdNew)
                    {
#if !DUMPER
                        if (!isPortable)
                        {
                            try
                            {
                                if (!Directory.Exists(BaseDirectoryExternal))
                                {
                                    Directory.CreateDirectory(BaseDirectoryExternal);
                                }

                                // There are some folders which should be accessed by user
                                // Moving them to "My documents"
                                var externalDirs = new string[]
                                    { "art", "folder_images", "info", "patches", "sfrom_tool", "user_mods", "spine_templates" };
                                foreach (var dir in externalDirs)
                                {
                                    var sourceDir = Path.Combine(BaseDirectoryInternal, dir);
                                    var destDir = Path.Combine(BaseDirectoryExternal, dir);
                                    if (isFirstRun || !Directory.Exists(destDir))
                                    {
                                        Shared.DirectoryCopy(sourceDir, destDir, true, false, true, false);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // TODO: Test it on Windows XP
                                Trace.WriteLine(ex.Message);
                            }
                        }

                        Directory.SetCurrentDirectory(BaseDirectoryInternal);

                        Trace.WriteLine("Base directory: " + BaseDirectoryExternal + " (" + (isPortable ? "portable" : "non-portable") + " mode)");
                        ConfigIni.Load();
                        try
                        {
                            if (!string.IsNullOrEmpty(ConfigIni.Instance.Language))
                                Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigIni.Instance.Language);
                        }
                        catch { }

                        string languagesDirectory = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "languages");
                        const string langFileNames = "hakchi.resources.dll";
                        AppDomain.CurrentDomain.AppendPrivatePath(languagesDirectory);
                        // For updates
                        var oldFiles = Directory.GetFiles(Path.GetDirectoryName(Application.ExecutablePath), langFileNames, SearchOption.AllDirectories);
                        foreach (var d in oldFiles)
                        {
                            if (!d.Contains(Path.DirectorySeparatorChar + "languages" + Path.DirectorySeparatorChar))
                            {
                                var dir = Path.GetDirectoryName(d);
                                Trace.WriteLine("Removing old directory: " + dir);
                                if (!isPortable)
                                {
                                    var targetDir = Path.Combine(languagesDirectory, Path.GetFileName(dir));
                                    Directory.CreateDirectory(targetDir);
                                    var targetFile = Path.Combine(targetDir, langFileNames);
                                    if (File.Exists(targetFile))
                                        File.Delete(targetFile);
                                    File.Copy(Path.Combine(dir, langFileNames), targetFile);
                                }
                                else
                                    Directory.Delete(dir, true);
                            }
                        }

                        Trace.WriteLine("Loading spine templates");
                        var templateDir = new DirectoryInfo(Path.Combine(BaseDirectoryExternal, "spine_templates"));
                        _SpineTemplates = new Dictionary<string, SpineTemplate<Bitmap>>();
                        if (templateDir.Exists)
                        {
                            foreach (var dir in templateDir.GetDirectories())
                            {
                                if (dir.GetFiles().Where(file => file.Name == "template.json" || file.Name == "template.png").Count() == 2)
                                {
                                    using (var file = File.OpenRead(Path.Combine(dir.FullName, "template.png")))
                                        _SpineTemplates.Add(dir.Name, SpineTemplate<Bitmap>.FromJsonFile(new SystemDrawingBitmap(new Bitmap(file) as Bitmap), Path.Combine(dir.FullName, "template.json")));
                                }
                            }
                        }

                        SetupScrapers();
#else
                        BaseDirectoryExternal = BaseDirectoryInternal;
#endif

                        Trace.WriteLine("Starting, version: " + Shared.AppDisplayVersion);

                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);

                        formContext.AllFormsClosed += Process.GetCurrentProcess().Kill; // Suicide! Just easy and dirty way to kill all threads.

#if !DUMPER
                        formContext.AddForm(GetRequiredService<MainForm>());
#else
                        formContext.AddForm(GetRequiredService<DumperForm>());
#endif
                        Application.Run(formContext);
                        Trace.WriteLine("Done.");
                    }
                    else
                    {
                        Process current = Process.GetCurrentProcess();
                        foreach (Process process in Process.GetProcessesByName("hakchi"))
                        {
                            if (process.Id != current.Id)
                            {
                                ShowWindow(process.MainWindowHandle, 9); // Restore
                                SetForegroundWindow(process.MainWindowHandle); // Foreground
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + ex.StackTrace);
                MessageBox.Show(ex.Message + ex.StackTrace, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static string GetCurrentLogContent()
        {
            MemoryStream stream = debugStreams.OfType<MemoryStream>().FirstOrDefault();
            if (stream != default(MemoryStream))
            {
                return Encoding.GetEncoding(MY_CODE_PAGE).GetString(stream.GetBuffer(), 0, (int)stream.Length);
            }
            return "";
        }
    }
}
