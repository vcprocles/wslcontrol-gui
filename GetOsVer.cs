using Microsoft.Win32;
using System;

class OsVer
{
    public OsVer()
    {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")!;
        string buildstr = registryKey.GetValue("UBR")!.ToString()!;
        build = int.Parse(buildstr);
    }
    public int build;
}