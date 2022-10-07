using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{

    public partial class MainWindow : Window
    {
        readonly WSLInterface wsli = new();
        readonly OsInfo os = new();
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SetInitialStatuses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }
        private void SetInitialStatuses()
        {
            DeactivateAllButtons();
            RefreshDistros();
        }
        private void DeactivateAllButtons()
        {
            InstallUninstallButton.IsEnabled = false;
            LaunchButton.IsEnabled = false;
            RunCommandButton.IsEnabled = false;
            TerminateButton.IsEnabled = false;
            SetDefaultButton.IsEnabled = false;
            OpenInExplorerButton.IsEnabled = false;
            //ThisDistroSettingsButton.IsEnabled=false;
        }
        private void RefreshDistros()
        {
            DistroList.ItemsSource = wsli.GetDistros();
            DistroList.SelectedItem = null;
            WSL2WarningLabel.Visibility = Visibility.Collapsed;
        }
        private void ShutdownButton_Click(object sender, RoutedEventArgs e)
        {
            wsli.ShutdownWSL();
            WaitAndRefresh();
        }
        private void RefreshButtons()
        {
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            if (selectedDistro == null)
            {
                DeactivateAllButtons();
                return;
            }
            else
            {
                if (selectedDistro.Version == 2) WSL2WarningLabel.Visibility = Visibility.Visible;
                if (selectedDistro.Version == 1) WSL2WarningLabel.Visibility = Visibility.Collapsed;
                OpenInExplorerButton.IsEnabled = true;
                if (selectedDistro.State == "Running") { TerminateButton.IsEnabled = true; }
                if (selectedDistro.State == "Stopped") { TerminateButton.IsEnabled = false; }
                if (os.build < 19041) { GlobalSettingsButton.IsEnabled = false; }
                RunCommandButton.IsEnabled = true;
                LaunchButton.IsEnabled = true;
                InstallUninstallButton.IsEnabled = true;
                SetDefaultButton.IsEnabled = true;
                if (selectedDistro.State == "Installing")
                {
                    TerminateButton.IsEnabled = false;
                    RunCommandButton.IsEnabled = false;
                    LaunchButton.IsEnabled = false;
                    InstallUninstallButton.IsEnabled = false;
                    SetDefaultButton.IsEnabled = false;
                }
            }
        }
        private void DistroList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshButtons();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            WSLInterface.OpenDistro(((Distro)DistroList.SelectedItem).Name);
        }

        private void RunCommandButton_Click(object sender, RoutedEventArgs e)
        {
            CommandInput inputwindow = new(((Distro)DistroList.SelectedItem).Name, wsli)
            {
                Owner = this
            };
            inputwindow.Show();
        }

        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            wsli.TerminateDistro(selectedDistro.Name);
            WaitAndRefresh();
        }
        private void WaitAndRefresh()
        {
            Thread.Sleep(250);
            RefreshDistros();
        }

        private void InstallUninstallButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult q = MessageBox.Show(GetWindow(InstallUninstallButton), "Are you sure you want to delete this distro?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (q == MessageBoxResult.Yes) wsli.UnregisterDistro(((Distro)DistroList.SelectedItem).Name);
            WaitAndRefresh();
        }

        private void SetDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultDistro(((Distro)DistroList.SelectedItem).Name);
            WaitAndRefresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshDistros();
        }
        private void OpenThisInExplorer_Click(object sender, RoutedEventArgs e)
        {
            int selectedDistroNumber = ((Distro)DistroList.SelectedItem).Number;
            RefreshDistros();
            DistroList.SelectedIndex = selectedDistroNumber - 1;
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            if ((selectedDistro != null) && (selectedDistro.State == "Running"))
            {
                var _ = new ShellExecuteBp(selectedDistro.Name);
            }
            else if ((selectedDistro != null) && os.build > 22000) //W11 doesn't need the distro running
            {
                var _ = new ShellExecuteBp(selectedDistro.Name);
            }
            else
            {
                var _ = new ShellExecuteBp();
            }
        }
        private void OpenAllInExplorer_Click(object sender, RoutedEventArgs e)
        {
            var _ = new ShellExecuteBp();
        }

        private void GlobalSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalPrefs inputwindow = new(wsli)
            {
                Owner = this
            };
            inputwindow.Show();
        }

        private void OnlineInstall_Click(object sender, RoutedEventArgs e)
        {
            OnlineInstall installwindow = new(wsli)
            {
                Owner = this
            };
            installwindow.Show();
        }

        private void ImportTar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx";
            openFile.Multiselect = false;
            openFile.Title = "Select file to import...";
            openFile.ShowDialog(this);
            string importFile=openFile.FileName;
            //TODO: write backend to pass this to wsli
        }

        private void ExportTar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx";
            saveFile.Title = "Select path and filename to export...";
            saveFile.AddExtension = true;
            saveFile.ShowDialog(this);
            string exportFile = saveFile.FileName;
            //TODO: write backend to pass this to wsli
        }

        //private void ThisDistroSettings_Click(object sender, RoutedEventArgs e)
        //{
        //    PerDistroPrefs inputwindow = new PerDistroPrefs((Distro)DistroList.SelectedItem, wsli);
        //    inputwindow.Owner = this;
        //    inputwindow.Show();
        //}
    }

}