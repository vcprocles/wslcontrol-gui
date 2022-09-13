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
    /// Interaction logic for CommandInput.xaml
    /// </summary>
    public partial class CommandInput : Window
    {
        string g_selectedDistro;
        WSLInterface g_wsli;
        public CommandInput(string selectedDistro, WSLInterface wsli)
        {
            InitializeComponent();
            g_selectedDistro = selectedDistro;
            g_wsli = wsli;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            g_wsli.RunCustomCommand(g_selectedDistro, CommandBox.Text);
        }
    }
}
