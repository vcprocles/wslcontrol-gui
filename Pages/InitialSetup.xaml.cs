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
    /// Interaction logic for InitialSetup.xaml
    /// </summary>
    public partial class InitialSetup : Window
    {
        private WSLInterface wsli;
        public InitialSetup(WSLInterface wslImport)
        {
            InitializeComponent();
            wsli = wslImport;
            OnlineDistroList.ItemsSource = wsli.GetOnlineDistros();
        }
    }
}
