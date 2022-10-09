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

namespace wslcontrol_gui.Pages
{
    /// <summary>
    /// Interaction logic for ImportVersionAlert.xaml
    /// </summary>
    public partial class ImportVersionAlert : Window
    {
        public ImportVersionAlert()
        {
            InitializeComponent();
            DistroName=DistroNameBox.Text;
            WSLVersion = 0;
        }
        public string DistroName { get; set; }
        public int WSLVersion { get; set; }

        private void WSL1Button_Click(object sender, RoutedEventArgs e)
        {
            WSLVersion = 1; 
            DistroName=DistroNameBox.Text;
            this.Close();
        }

        private void WSL2Button_Click(object sender, RoutedEventArgs e)
        {
            WSLVersion = 2;
            DistroName = DistroNameBox.Text;
            this.Close();
        }
    }
}
