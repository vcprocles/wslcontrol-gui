using System.Windows;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for SelectedSettings.xaml
    /// </summary>
    public partial class SelectedSettings : Window
    {
        public SelectedSettings(string selectedDistro, WSLInterface wsli)
        {
            InitializeComponent();
        }

    }
}
