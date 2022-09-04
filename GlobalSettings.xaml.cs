using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        IniParseWrapGlobal ini;
        public GlobalSettings(WSLInterface wsli_extern)
        {
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
            ini = new IniParseWrapGlobal();
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

        }

        private void CoreCountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void LocalhostForward_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CustomKernel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void KernelArgs_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SwapSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SwapfilePath_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MemReclaimCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void WSLgCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DebugConsoleCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void NestedVirtTick_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ShutdownTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
