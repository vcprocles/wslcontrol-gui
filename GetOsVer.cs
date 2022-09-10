using Microsoft.Win32;
using System;
using System.Security.Principal;

class OsVer
{
    public OsVer()
    {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
        string buildstr = registryKey.GetValue("CurrentBuild")!.ToString()!;
        build = int.Parse(buildstr);
        elevated = (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
             .IsInRole(WindowsBuiltInRole.Administrator);
    }
    public int build;
    public bool elevated;
}