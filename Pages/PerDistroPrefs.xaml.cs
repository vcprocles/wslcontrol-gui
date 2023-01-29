using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{
    public partial class PerDistroPrefs : Window
    {
        readonly OsInfo os = new();
        readonly IniParseWrapSpecific ini;
        private bool removeConfig = false;
        enum SwitchStates
        {
            Enable,
            Disable
        }
        private static Distro distro;
        public PerDistroPrefs(Distro selectedDistro, WSLInterface wsli)
        {
            IniParseWrapSpecific.GetConfig(selectedDistro.Name);
            Thread.Sleep(500);
            distro = selectedDistro;
            ini = new(distro);
            InitializeComponent();
            DisableUnsupported();
            InitializeSetOrDefault();
        }
        private void DisableUnsupported() 
        {
            bool buildNotOk = true;
            if (os.build > 19041) buildNotOk = false;
            if (buildNotOk) //WS2022, W11, for boot settings
            { CommandOnBoot.IsEnabled = false; }
            if (buildNotOk) //for user settings
            { DefaultUsername.IsEnabled = false; }
            if (buildNotOk) //for interop settings
            {
                WindowsProcessesCreation.IsEnabled = false;
                AppendPath.IsEnabled = false;
            }
            if (buildNotOk) //for console window warning
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
            bool automount = ini.ReadParameterBoolean(section, "enabled", out err);
            if (err) automount = true;
            //process fstab
            bool processFstab = ini.ReadParameterBoolean(section, "mountFsTab", out err);
            if (err) processFstab = true;
            //windows fs path
            string windowsPath = ini.ReadParameterString(section, "root", out err);
            if (err) windowsPath = "/mnt/";
            string mountOptions = ini.ReadParameterString(section, "options", out err);//swap this for more complex configurator
            if (err) mountOptions = "";

            #endregion
            #region network
            //generate hosts
            section = "network";
            bool hostsGen = ini.ReadParameterBoolean(section, "generateHosts", out err);
            if (err) hostsGen = true;
            //generate resolv
            bool resolvGen = ini.ReadParameterBoolean(section, "generateResolvConf", out err);
            if (err) resolvGen = true;
            //hostname
            string hostname = ini.ReadParameterString(section, "hostname", out err);
            if (err) hostname = "";//should use Windows hostname if empty
            #endregion
            #region interop
            section = "interop";
            //windows processes
            bool winProcessesStart = ini.ReadParameterBoolean(section, "enabled", out err);
            if (err) winProcessesStart = true;
            //append path
            bool appendPath = ini.ReadParameterBoolean(section, "appendWindowsPath", out err);
            if (err) appendPath = true;
            #endregion
            #region defaultuser
            section = "user";
            string defaultUser = ini.ReadParameterString(section, "default", out err);
            if (err) defaultUser = "";
            #endregion
            #region commandonboot
            section = "boot";
            string commandOnBoot = ini.ReadParameterString(section, "command", out err);
            if (err) commandOnBoot = "";
            bool enableSystemd = ini.ReadParameterBoolean(section, "systemd", out err);
            if (err) enableSystemd = false;
            #endregion
            #region setting gathered stuff
            EnableAutomountCheckMark.IsChecked = automount;
            if (!automount) AutomountSwitch(SwitchStates.Disable);
            ProcessFstabCheckmark.IsChecked = processFstab;
            WinFsPathBox.Text = windowsPath;
            MountOptionsTextBox.Text = mountOptions;
            HostsTick.IsChecked = hostsGen;
            ResolvTick.IsChecked = resolvGen;
            CustomHostnameBox.Text = hostname;
            WindowsProcessesCreation.IsChecked = winProcessesStart;
            AppendPath.IsChecked = appendPath;
            DefaultUsername.Text = defaultUser;
            CommandOnBoot.Text = commandOnBoot;
            SystemdEnableCheckBox.IsChecked = enableSystemd;
            #endregion
        }

        private void AutomountSwitch(SwitchStates a)
        {
            if (ProcessFstabCheckmark == null) return;
            if (a == SwitchStates.Enable)
            {
                ProcessFstabCheckmark.IsEnabled = true;
                WinFsPathBox.IsEnabled = true;
                WinFsPathLabel.IsEnabled = true;
                MountOptionsLabel.IsEnabled = true;
                MountOptionsTextBox.IsEnabled = true;
            }
            else if (a == SwitchStates.Disable)
            {
                ProcessFstabCheckmark.IsEnabled = false;
                WinFsPathBox.IsEnabled = false;
                WinFsPathLabel.IsEnabled = false;
                MountOptionsLabel.IsEnabled = false;
                MountOptionsTextBox.IsEnabled = false;
            }
            else throw new Exception("Unexpected behaviour");
        }

        private void EnableAutomountCheckMark_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = sender as CheckBox;
            if (sender is CheckBox && a.IsChecked == null) return;
            if (a.IsChecked.Value == true)
            {
                AutomountSwitch(SwitchStates.Enable);
                ini.SetParameter("automount", "enabled", a.IsChecked.Value);
            }
            else
            {
                AutomountSwitch(SwitchStates.Disable);
                ini.SetParameter("automount", "enabled", a.IsChecked.Value);
            }
        }

        private void ProcessFstabCheckmark_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("automount","mountFsTab", a.IsChecked.Value);
        }

        private void WinFsPathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            if (a != null)
                ini.SetParameter("automount", "root", a.Text);
        }

        private void MountOptionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            if (a!= null)
            ini.SetParameter("automount", "options", a.Text);
        }

        private void HostsTick_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("network", "generateHosts", a.IsChecked.Value);
        }

        private void ResolvTick_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("network", "generateResolvConf", a.IsChecked.Value);
        }

        private void CustomHostnameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            if (a != null)
                ini.SetParameter("network", "hostname", a.Text);
        }

        private void WindowsProcessesCreation_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("interop", "enabled", a.IsChecked.Value);
        }

        private void AppendPath_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("interop", "appendWindowsPath", a.IsChecked.Value);
        }

        private void DefaultUsername_TextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            if (a != null)
                ini.SetParameter("user", "default", a.Text);
        }

        private void CommandOnBoot_TextChanged(object sender, TextChangedEventArgs e)
        {
            var a = sender as TextBox;
            if (a != null)
                ini.SetParameter("boot", "command", a.Text);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            ini.WriteOut();
            Thread.Sleep(100);
            if (removeConfig)
            {
                ini.ResetConfig();
            }
            else 
            {
                ini.SetConfig();
            }
        }

        private void SystemdEnableCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("boot", "systemd", a.IsChecked.Value);
        }

        private void ResetToDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            removeConfig = true;
            this.Close();
        }
    }
}
