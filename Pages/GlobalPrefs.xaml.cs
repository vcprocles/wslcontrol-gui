using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{
    public partial class GlobalPrefs : Window
    {
        readonly WSLInterface wsli;
        readonly IniParseWrapGlobal ini;
        bool noWriteout = false;
        public GlobalPrefs(WSLInterface wsli_extern)
        {
            ini = new IniParseWrapGlobal();
            InitializeComponent();
            wsli = wsli_extern;
            DisableUnsupported();
            InitializeSetOrDefault();
        }
        private void DisableUnsupported()
        {
            OsInfo os = new();
            bool buildOk = false;
            if (os.build > 19041) buildOk = true;
            if (!buildOk)
            {
                WSLgCheckBox.IsEnabled = false;
                DebugConsoleCheckBox.IsEnabled = false;
                NestedVirtTick.IsEnabled = false;
                ShutdownTimeout.IsEnabled = false;
            }
        }
        private void InitializeSetOrDefault() //TODO: make a class/enum to make stuff easier to manage?
        {
            string section = "wsl2";
            //WSL version
            switch (wsli.GetCurrentDefaultWSLVersion())
            {
                case 1:
                    WSLVersionSelected1.IsChecked = true;
                    break;
                case 2:
                    WSLVersionSelected2.IsChecked = true;
                    break;
                default:
                    throw new Exception("Unexpected behaviour");
            }
            //ram limit
            string ramLimit = ini.ReadParameterString(section, "memory", out bool err);
            if (err) ramLimit = "";
            RamLimitTextBox.Text = ramLimit;
            //cpu cores
            string cpuCount = ini.ReadParameterString(section, "processors", out err);
            if (err) cpuCount = "";
            CoreCountTextBox.Text = cpuCount;
            //localhost forwarding
            bool localhostForward = ini.ReadParameterBoolean(section, "localhostForwarding", out err);
            if (err) localhostForward = true;
            LocalhostForward.IsChecked = localhostForward;
            //path to custom kernel
            string kernelPath = ini.ReadParameterString(section, "kernel", out err);
            if (err) kernelPath = "";
            CustomKernel.Text = kernelPath;
            //command line arguments
            string kernelArgs = ini.ReadParameterString(section, "kernelCommandLine", out err);
            if (err) kernelArgs = "";
            KernelArgs.Text = kernelArgs;
            //swap file size
            string swapSize = ini.ReadParameterString(section, "swap", out err);
            if (err) swapSize = "";
            SwapSizeTextBox.Text = swapSize;
            //custom swap
            string swapPath = ini.ReadParameterString(section, "swapFile", out err);
            if (err) swapPath = "";
            SwapfilePath.Text = swapPath;
            //memory reclaim
            bool memReclaim = ini.ReadParameterBoolean(section, "pageReporting", out err);
            if (err) memReclaim = true;
            MemReclaimCheckBox.IsChecked = memReclaim;
            //gui apps
            bool wslg = ini.ReadParameterBoolean(section, "guiApplications", out err);
            if (err) wslg = true;
            WSLgCheckBox.IsChecked = wslg;
            //debug console
            bool debugConsole = ini.ReadParameterBoolean(section, "debugConsole", out err);
            if (err) debugConsole = false;
            DebugConsoleCheckBox.IsChecked = debugConsole;
            //nested vm
            bool nestVm = ini.ReadParameterBoolean(section, "pageReporting", out err);
            if (err) nestVm = true;
            NestedVirtTick.IsChecked = nestVm;
            //timeout
            string shutdownTimeout = ini.ReadParameterString(section, "vmIdleTimeout", out err);
            if (err) shutdownTimeout = "6000";
            ShutdownTimeout.Text = shutdownTimeout;
        }
        private void WSLVersionSelected1_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(1);
        }
        private void WSLVersionSelected2_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(2);
        }

        private void RamSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("memory", RamLimitTextBox.Text);
        }

        private void CoreCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("processors", CoreCountTextBox.Text);
        }

        private void LocalhostForward_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("localhostForwarding", a.IsChecked.Value);
        }

        private void CustomKernel_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("kernel", CustomKernel.Text);
        }

        private void KernelArgs_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("kernelCommandLine", KernelArgs.Text);
        }

        private void SwapSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("swap", SwapSizeTextBox.Text);
        }

        private void SwapfilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("swapfile", SwapfilePath.Text);
        }

        private void MemReclaimCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("pageReporting", a.IsChecked.Value);
        }

        private void WSLgCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("guiApplications", a.IsChecked.Value);
        }

        private void DebugConsoleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("debugConsole", a.IsChecked.Value);
        }

        private void NestedVirtTick_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox a && a.IsChecked != null)
                ini.SetParameter("nestedVirtualization", a.IsChecked.Value);
        }

        private void ShutdownTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox a && a.Text != null)
                ini.SetParameter("vmIdleTimeout", a.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!noWriteout) ini.WriteOut();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            noWriteout = true;//tell to not save anything when window closes
            if (File.Exists(ini.path))
            {
                File.Delete(ini.path); //delete file
            }
            this.Close();
        }
    }
}
