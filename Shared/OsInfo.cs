using Microsoft.Win32;
using System;
using System.Security.Principal;

namespace wslcontrol_gui
{
    static class OsInfo
    {
        public enum UACStatus
        {
            Elevated,
            Limited
        }
        public static int GetOsBuild()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
            string buildstr = registryKey.GetValue("CurrentBuild")!.ToString()!;
            return int.Parse(buildstr);
        }
        public static bool IsAdmin()
        {
            UACStatus status = GetUACStatus();
            switch (status)
            {
                case UACStatus.Elevated:
                    return true;
                case UACStatus.Limited:
                    return false;
                default:
                    return false;
            }
        }

        public static string WSLVersion()
        {
            WSLInterface wsli = new();
            string result = wsli.PassToWSL("--version");
            result = result.Split("\r\n")[0].Split(" ")[2];
            return result;
        }
        public const string testedWSLVersion = "1.2.5.0";
        public static UACStatus GetUACStatus()
        {
            bool isAdmin = (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                 .IsInRole(WindowsBuiltInRole.Administrator);
            if (isAdmin) { return UACStatus.Elevated; }
            else { return UACStatus.Limited; }
        }
    }
}