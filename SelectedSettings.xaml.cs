using System.Windows;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for SelectedSettings.xaml
    /// </summary>
    public partial class SelectedSettings : Window
    {
        OsInfo os = new();
        public SelectedSettings(string selectedDistro, WSLInterface wsli)
        {
            InitializeComponent();
            DisableUnsupported();
        }
        private void DisableUnsupported()
        {

        }
    }
}
