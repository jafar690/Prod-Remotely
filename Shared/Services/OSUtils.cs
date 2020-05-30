using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Silgred.Shared.Enums;

namespace Silgred.Shared.Services
{
    public static class OSUtils
    {
        public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string ClientExecutableFileName
        {
            get
            {
                var fileExt = "";
                if (IsWindows)
                    fileExt = "Remotely_Agent.exe";
                else if (IsLinux) fileExt = "Remotely_Agent";
                return fileExt;
            }
        }

        public static string ScreenCastExecutableFileName
        {
            get
            {
                if (IsWindows)
                    return "Remotely_ScreenCast.exe";
                if (IsLinux)
                    return "Remotely_ScreenCast.Linux";
                throw new Exception("Unsupported operating system.");
            }
        }

        public static Platform Platform
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return Platform.Windows;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return Platform.Linux;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return Platform.OSX;
                return Platform.Unknown;
            }
        }

        public static string StartProcessWithResults(string command, string arguments)
        {
            var psi = new ProcessStartInfo(command, arguments);
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.Verb = "RunAs";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;

            var proc = new Process();
            proc.StartInfo = psi;

            proc.Start();
            proc.WaitForExit();

            return proc.StandardOutput.ReadToEnd();
        }
    }
}