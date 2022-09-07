using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class GlobalSettings : Window
    {
        WSLInterface wsli;
        OsVer os = new();
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
        private void InitializeSetOrDefault() //TODO
        {

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
    }
}
