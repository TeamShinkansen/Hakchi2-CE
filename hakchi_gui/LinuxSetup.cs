using com.clusterrr.hakchi_gui.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace com.clusterrr.hakchi_gui
{
    /// <summary>
    /// Linux only: detects missing USB/network setup and fixes it by running the bundled hakchi-setup.sh as root
    /// through pkexec, so users get their desktop's normal password prompt instead of terminal instructions.
    /// </summary>
    static class LinuxSetup
    {
        const string udevRuleFile = "/etc/udev/rules.d/70-hakchi.rules";
        static readonly string scriptPath = Path.Combine(Program.BaseDirectoryInternal, "hakchi-setup.sh");

        // ask at most once per session so a declined or ineffective setup doesn't nag
        static bool offered = false;
        static DateTime? unconfiguredSince = null;

        public static bool Available => !Shared.isWindows && File.Exists(scriptPath);

        public static bool UsbRuleInstalled => File.Exists(udevRuleFile);

        /// <summary>Network interfaces created by a console in USB network mode (driver rndis_host).</summary>
        public static List<string> ConsoleNetworkInterfaces()
        {
            var result = new List<string>();
            try
            {
                foreach (var dev in Directory.GetDirectories("/sys/class/net"))
                {
                    var uevent = Path.Combine(dev, "device", "uevent");
                    if (File.Exists(uevent) && File.ReadAllLines(uevent).Contains("DRIVER=rndis_host"))
                        result.Add(Path.GetFileName(dev));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not list network interfaces: " + ex.Message);
            }
            return result;
        }

        static bool hasIPv4(string interfaceName)
        {
            try
            {
                var nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(n => n.Name == interfaceName);
                return nic != null && nic.GetIPProperties().UnicastAddresses.Any(a => a.Address.AddressFamily == AddressFamily.InterNetwork);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Called periodically while offline: offers the setup when a console's USB network link has stayed
        /// without an IPv4 address for a while (nothing on the PC configures it, so hakchi can't reach the console).
        /// </summary>
        public static void CheckWhileOffline(Form owner)
        {
            if (!Available || offered)
                return;

            var unconfigured = ConsoleNetworkInterfaces().Where(i => !hasIPv4(i)).ToList();
            if (unconfigured.Count == 0)
            {
                unconfiguredSince = null;
                return;
            }
            if (unconfiguredSince == null)
            {
                unconfiguredSince = DateTime.Now;
                return;
            }
            // give NetworkManager & co. time to assign an address on their own
            if (DateTime.Now.Subtract(unconfiguredSince.Value).TotalSeconds < 10)
                return;

            Trace.WriteLine("Console USB network link without an IPv4 address: " + string.Join(", ", unconfigured));
            Offer(owner, "Your console is connected over USB, but this PC hasn't set up the network link to it, so hakchi can't reach it.");
        }

        /// <summary>Asks the user whether to run the setup (once per session). Returns true if it ran successfully.</summary>
        public static bool Offer(IWin32Window owner, string problem)
        {
            if (!Available || offered)
                return false;
            offered = true;

            var answer = Tasks.MessageForm.Show(
                owner as Form, "Linux setup",
                problem + "\n\nHakchi can set this up for you: USB access to the console, the network link, and a firewall exception. " +
                "You'll be asked for your password once.\n\nSet it up now?",
                Resources.sign_cogs,
                new Tasks.MessageForm.Button[] { Tasks.MessageForm.Button.Yes, Tasks.MessageForm.Button.No },
                Tasks.MessageForm.DefaultButton.Button1);
            return answer == Tasks.MessageForm.Button.Yes && Run(owner);
        }

        /// <summary>Runs hakchi-setup.sh as root via pkexec and reports the outcome. Returns true on success.</summary>
        public static bool Run(IWin32Window owner)
        {
            offered = true;
            var output = new StringBuilder();
            int exitCode;
            // Run a copy: inside an AppImage the app lives on a FUSE mount that root can't read
            var scriptCopy = Path.Combine(Path.GetTempPath(), "hakchi-setup-" + Guid.NewGuid().ToString("N") + ".sh");
            try
            {
                File.Copy(scriptPath, scriptCopy);
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo("pkexec", "/bin/sh \"" + scriptCopy + "\" --user \"" + Environment.UserName + "\"")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                proc.OutputDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
                proc.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (output) output.AppendLine(e.Data); };
                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                // keep the window responsive while the password prompt is open
                while (!proc.WaitForExit(100))
                    Application.DoEvents();
                proc.WaitForExit();
                exitCode = proc.ExitCode;
            }
            catch (Win32Exception)
            {
                Tasks.MessageForm.Show(owner as Form, "Linux setup",
                    "pkexec was not found, so hakchi can't ask for your password. Run this in a terminal instead:\n\nsudo sh \"" + scriptCopy + "\"",
                    Resources.sign_error);
                return false; // keep the copy so the command above works
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Could not run hakchi-setup.sh: " + ex.Message);
                try { File.Delete(scriptCopy); } catch { }
                return false;
            }
            try { File.Delete(scriptCopy); } catch { }

            Trace.WriteLine("hakchi-setup.sh exited with " + exitCode + ":\n" + output);
            if (exitCode == 0)
            {
                Tasks.MessageForm.Show(owner as Form, "Linux setup",
                    "Setup finished. If your console is plugged in, unplug it and plug it back in.\n\n" + output.ToString().Trim(),
                    Resources.sign_check);
                return true;
            }
            if (exitCode == 126 || exitCode == 127)
                return false; // password prompt cancelled or not authorized

            Tasks.MessageForm.Show(owner as Form, "Linux setup",
                "Setup failed (exit code " + exitCode + "):\n\n" + output.ToString().Trim(),
                Resources.sign_error);
            return false;
        }
    }
}
