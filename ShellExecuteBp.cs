using System.Runtime.InteropServices;
using System.Windows;
using System;

public partial class ShellExecuteBp
{
    OsInfo os = new();
    public ShellExecuteBp(String Distro)
    {
        if (os.build < 22000)
        {
            ShellExecute(IntPtr.Zero, "open", "explorer.exe", "\\\\wsl$\\" + Distro, "", ShowCommands.SW_NORMAL);//checked and working for Windows 10
        }
        else
        {
            ShellExecute(IntPtr.Zero, "open", "explorer.exe", "\\\\wsl.localhost\\" + Distro, "", ShowCommands.SW_NORMAL);//for Windows 11
        }
    }
    public ShellExecuteBp()
    {
        if (os.build <22000)
        {
            ShellExecute(IntPtr.Zero, "open", "explorer.exe", "\\\\wsl$\\", "", ShowCommands.SW_NORMAL);
        }
        else
        {
            ShellExecute(IntPtr.Zero, "open", "explorer.exe", "\\\\wsl.localhost\\", "", ShowCommands.SW_NORMAL); //for Windows 11
        }
    }

    public enum ShowCommands : int
    {
        SW_HIDE = 0,
        SW_SHOWNORMAL = 1,
        SW_NORMAL = 1,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMAXIMIZED = 3,
        SW_MAXIMIZE = 3,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOW = 5,
        SW_MINIMIZE = 6,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_RESTORE = 9,
        SW_SHOWDEFAULT = 10,
        SW_FORCEMINIMIZE = 11,
        SW_MAX = 11
    }

    [DllImport("shell32.dll")]
    static extern IntPtr ShellExecute(
        IntPtr hwnd,
        string lpOperation,
        string lpFile,
        string lpParameters,
        string lpDirectory,
        ShowCommands nShowCmd);
}
