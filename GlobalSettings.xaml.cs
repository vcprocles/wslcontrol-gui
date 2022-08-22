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
        }
        private void WSLVersionSelected1_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(1);
        }
        private void WSLVersionSelected2_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(2);
        }
    }
}
