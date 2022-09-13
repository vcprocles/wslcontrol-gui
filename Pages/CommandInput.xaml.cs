using System.Windows;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for CommandInput.xaml
    /// </summary>
    public partial class CommandInput : Window
    {
        readonly string g_selectedDistro;
        public CommandInput(string selectedDistro, WSLInterface _)
        {
            InitializeComponent();
            g_selectedDistro = selectedDistro;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            WSLInterface.RunCustomCommand(g_selectedDistro, CommandBox.Text);
        }
    }
}
