using Microsoft.Win32;
//using System.Security.Principal;

namespace wslcontrol_gui
{
    class OsInfo
    {
        public OsInfo()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
            string buildstr = registryKey.GetValue("CurrentBuild")!.ToString()!;
            build = int.Parse(buildstr);
            //build = 30000;//basically disable all version checks
            //elevated = (new WindowsPrincipal(WindowsIdentity.GetCurrent()))//unused
            //     .IsInRole(WindowsBuiltInRole.Administrator);
        }
        public int build;
        //public bool elevated;
    }
}