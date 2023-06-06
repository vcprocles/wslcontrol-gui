using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui.Pages
{
    public partial class InitialSetup : Window
    {
        private WSLInterface wsli;
        public InitialSetup(WSLInterface wslImport)
        {
            InitializeComponent();
            wsli = wslImport;
            OnlineDistroList.ItemsSource = wsli.GetOnlineDistros();
            SourceSelect.SelectedIndex = 0;
            RecheckElements();
        }
        private void RecheckElements()
        {
            if (SourceSelect.SelectedIndex == 0)
            {
                WSL1SupportCheckBox.IsEnabled = true;
            }
            else
            {
                WSL1SupportCheckBox.IsEnabled = false;
            }
            if (SpecificDistroCheckBox.IsChecked == true)
            {
                OnlineDistroList.IsEnabled = true;
            }
            else
            {
                OnlineDistroList.IsEnabled = false;
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SourceSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecheckElements();
        }

        private void SpecificDistroCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            RecheckElements();
        }
        private string GenerateCmdline()
        {
            string cmdline = "--install ";
            switch (SourceSelect.SelectedIndex)
            {
                case 1:
                    cmdline += "--inbox ";
                    break;
                case 2:
                    cmdline += "--web-download ";
                    break;
                default:
                    break;
            }
            if (WSL1SupportCheckBox.IsChecked == true && SourceSelect.SelectedIndex == 0)
            {
                cmdline += "--enable-wsl1 ";
            }
            if (SpecificDistroCheckBox.IsChecked == true)
            {
                if (OnlineDistroList.SelectedItem != null)
                {
                    var distro = OnlineDistroList.SelectedItem as Distro;
                    cmdline += "-d ";
                    if (distro != null)
                    {
                        cmdline += distro.Name;
                    }
                }
            }
            return cmdline;
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            var cmdline = GenerateCmdline();
            WSLInterface.PassToWSLWithWindow(cmdline);
            this.Close();
        }
    }
}
