using System;
using System.Windows;
using System.Windows.Controls;

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
            if (os.build < 20348) //WS2022, W11, for boot settings
            { CommandOnBoot.IsEnabled = false; }
            if (os.build < 18980) //for user settings
            { DefaultUsername.IsEnabled = false; }
            if (os.build < 17713) //for interop settings
            {
                WindowsProcessesCreation.IsEnabled = false;
                AppendPath.IsEnabled = false;
            }
        }
    }
}
