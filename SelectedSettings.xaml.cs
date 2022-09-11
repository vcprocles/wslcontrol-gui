using System;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{
    public partial class SelectedSettings : Window
    {
        OsInfo os = new();
        IniParseWrapSpecific ini;
        private static Distro distro;
        public SelectedSettings(Distro selectedDistro, WSLInterface wsli)
        {
            distro = selectedDistro;
            ini = new(distro);
            InitializeComponent();
            DisableUnsupported();
            InitializeSetOrDefault();
        }
        private void DisableUnsupported()
        {
            if (os.build < 20348) //WS2022, W11, for boot settings
            { CommandOnBoot.IsEnabled = false; }
            if (os.build < 18980) //for user settings
            { DefaultUsername.IsEnabled = false; }
            if (os.build < 17713) //for interop settings
            {
                WindowsProcessesCreation.IsEnabled = false;
                AppendPath.IsEnabled = false;
            }
            if (os.build >= 22000) //for console window warning
            {
                ConsoleWindowWarning.Visibility = Visibility.Collapsed;
            }
        }
        private void InitializeSetOrDefault()
        {
            bool err;
            string section;
            #region automount
            section = "automount";
            //enable automount
            bool automount = ini.bReadParameter(section,"enabled",out err);
            if (err) automount = true;
            //process fstab
            bool processFstab = ini.bReadParameter(section,"mountFsTab",out err);
            if (err) processFstab = true;
            //windows fs path
            string WindowsPath = ini.sReadParameter(section,"root",out err);
            if (err) WindowsPath = "/mnt/";
            string mountOptions = ini.sReadParameter(section, "options", out err);//swap this for more complex configurator
            if (err) mountOptions = "";

            #endregion
            #region network
            //generate hosts
            section = "network";
            bool hostsGen = ini.bReadParameter(section, "generateHosts",out err);
            if (err) hostsGen = true;
            //generate resolv
            bool resolvGen = ini.bReadParameter(section, "generateResolvConf", out err);
            if (err) resolvGen = true;
            //hostname
            string hostname = ini.sReadParameter(section, "hostname", out err);
            if (err) hostname = "";//should use Windows hostname if empty
            #endregion
            #region interop
            section = "interop";
            //windows processes
            bool winProcessesStart = ini.bReadParameter(section, "enabled", out err);
            if (err) winProcessesStart = true;
            //append path
            bool appendPath = ini.bReadParameter(section, "appendWindowsPath", out err);
            if (err) appendPath = true;
            #endregion
            #region defaultuser
            section = "user";
            string defaultUser = ini.sReadParameter(section, "default",out err);
            if (err) defaultUser = "";
            #endregion
            #region commandonboot
            section = "boot";
            string commandOnBoot = ini.sReadParameter(section, "command", out err);
            if (err) commandOnBoot = "";
            #endregion
            #region setting gathered stuff

            #endregion
        }

        private void AutomountSwitch(bool state)
        {

        }
    }
}
