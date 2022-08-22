using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace wslcontrol_gui
{
    public partial class MainWindow : Window
    {
        WSLInterface wsli = new();
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
            ThisDistroSettingsButton.IsEnabled=false;
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
                if (selectedDistro.State == "Running") { TerminateButton.IsEnabled = true; OpenInExplorerButton.IsEnabled = true; }
                if (selectedDistro.State == "Stopped") { TerminateButton.IsEnabled = false; OpenInExplorerButton.IsEnabled = false; }
                RunCommandButton.IsEnabled = true;
                LaunchButton.IsEnabled = true;
                InstallUninstallButton.IsEnabled = true;
                SetDefaultButton.IsEnabled = true;
                ThisDistroSettingsButton.IsEnabled = true;
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
            wsli.OpenDistro(((Distro)DistroList.SelectedItem).Name);
        }

        private void RunCommandButton_Click(object sender, RoutedEventArgs e)
        {
            CommandInput inputwindow = new CommandInput(((Distro)DistroList.SelectedItem).Name,wsli);
            inputwindow.Owner = this;
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
            DistroList.SelectedIndex = selectedDistroNumber-1;
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            if ((selectedDistro != null) && (selectedDistro.State=="Running"))
            {
                ShellExecuteBp a = new(selectedDistro.Name);
            }
            else
            {
                ShellExecuteBp a = new();
            }
        }
        private void OpenAllInExplorer_Click(object sender, RoutedEventArgs e)
        {
            ShellExecuteBp a = new();
        }

        private void GlobalSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            GlobalSettings inputwindow = new GlobalSettings(wsli);
            inputwindow.Owner = this;
            inputwindow.Show();
        }

        private void ThisDistroSettings_Click(object sender, RoutedEventArgs e)
        {
            SelectedSettings inputwindow = new SelectedSettings(((Distro)DistroList.SelectedItem).Name, wsli);
            inputwindow.Owner = this;
            inputwindow.Show();
        }
    }

}