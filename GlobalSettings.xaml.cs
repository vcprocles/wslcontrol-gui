using System;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{
    public partial class GlobalSettings : Window
    {
        WSLInterface wsli;
        OsInfo os = new();
        IniParseWrapGlobal ini;
        public GlobalSettings(WSLInterface wsli_extern)
        {
            ini = new IniParseWrapGlobal();
            InitializeComponent();
            wsli = wsli_extern;
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
            if (os.build<22000)
            {
                WSLgCheckBox.IsEnabled = false;
                DebugConsoleCheckBox.IsEnabled = false;
                NestedVirtTick.IsEnabled = false;
                ShutdownTimeout.IsEnabled = false;
            }
            InitializeSetOrDefault();
        }
        private void InitializeSetOrDefault() //TODO: make a class/enum to make stuff easier to manage?
        {
            //WSL version is marked independently from other stuff
            //ram limit
            RamLimitTextBox.Text = ini.ReadParameter("memory");
            //cpu cores
            CoreCountTextBox.Text = ini.ReadParameter("processors");
            //localhost forwarding
           // LocalhostForward.IsChecked = bool.Parse(ini.ReadParameter("localhostForwarding"));
            bool check_localhostForward;
            if (bool.TryParse(ini.ReadParameter("localhostForwarding"), out check_localhostForward))
            {
                LocalhostForward.IsChecked = check_localhostForward;
            }
            else LocalhostForward.IsChecked = true; //as default
            //path to custom kernel
            CustomKernel.Text = ini.ReadParameter("kernel");
            //command line arguments
            KernelArgs.Text = ini.ReadParameter("kernelCommandLine");
            //swap file size
            SwapSizeTextBox.Text = ini.ReadParameter("swap");
            //custom swap
            SwapfilePath.Text = ini.ReadParameter("swapFile");
            //memory reclaim
            bool memReclaim;
            if (bool.TryParse(ini.ReadParameter("pageReporting"), out memReclaim))
            {
                MemReclaimCheckBox.IsChecked = memReclaim; 
            }
            else MemReclaimCheckBox.IsChecked = true; //as default
            //gui apps
            //WSLgCheckBox.IsChecked = bool.Parse(ini.ReadParameter("guiApplications"));
            bool wslg;
            if (bool.TryParse(ini.ReadParameter("guiApplications"),out wslg))
            {
                WSLgCheckBox.IsChecked = wslg;
            }   
            else WSLgCheckBox.IsChecked = true;
            //debug console
            //DebugConsoleCheckBox.IsChecked = bool.Parse(ini.ReadParameter("debugConsole"));
            bool debugConsole;
            if (bool.TryParse(ini.ReadParameter("debugConsole"), out debugConsole))
            {
                DebugConsoleCheckBox.IsChecked = debugConsole;
            }
            else DebugConsoleCheckBox.IsChecked = false; //as default
            //nested vm
            //NestedVirtTick.IsChecked = bool.Parse(ini.ReadParameter("nestedVirtualization"));
            bool nestVm = true;
            if (bool.TryParse(ini.ReadParameter("pageReporting"), out nestVm))
            {
                NestedVirtTick.IsChecked = nestVm;
            }
            else NestedVirtTick.IsChecked = true; //as default
            //timeout
            ShutdownTimeout.Text = ini.ReadParameter("vmIdleTimeout");
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
            ini.SetParameter("memory",RamLimitTextBox.Text);
        }

        private void CoreCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("processors",CoreCountTextBox.Text);
        }

        private void LocalhostForward_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = sender as CheckBox;
            ini.SetParameter("localhostForwarding", a.IsChecked.Value);
        }

        private void CustomKernel_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("kernel", CustomKernel.Text);
        }

        private void KernelArgs_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("kernelCommandLine",KernelArgs.Text);
        }

        private void SwapSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("swap",SwapSizeTextBox.Text);
        }

        private void SwapfilePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            ini.SetParameter("swapfile", SwapfilePath.Text);
        }

        private void MemReclaimCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = sender as CheckBox;
            ini.SetParameter("pageReporting", a.IsChecked.Value);
        }

        private void WSLgCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = sender as CheckBox;
            ini.SetParameter("guiApplications",a.IsChecked.Value);
        }

        private void DebugConsoleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a = sender as CheckBox;
            ini.SetParameter("debugConsole", a.IsChecked.Value);
        }

        private void NestedVirtTick_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox a=sender as CheckBox;
            ini.SetParameter("nestedVirtualization",a.IsChecked.Value);
        }

        private void ShutdownTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox a = sender as TextBox;
            ini.SetParameter("vmIdleTimeout", a.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ini.WriteOut();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
