using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //List<Distro> items = new List<Distro>();
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
            //FilloutTestDistros();
        }
        //private void FilloutTestDistros()
        //{
        //    items.Add(new Distro() { Name = "Ubuntu", State = "Running", Version = 1 });
        //    items.Add(new Distro() { Name = "Debian", State = "Running", Version = 2 });
        //    items.Add(new Distro() { Name = "opensuse-tumbleweed", State = "Stopped", Version = 1 });
        //    DistroList.ItemsSource = items;
        //}
        private void SetInitialStatuses()
        {
            DeactivateAllButtons();
            switch (wsli.GetCurrentDefaultWSLVersion())
            {
                case 1:
                    WSLVersionSelected1.IsChecked = true;
                    break;
                case 2:
                    WSLVersionSelected2.IsChecked = true;
                    break;
                default:
                    throw new Exception("Is WSL installed?");
            }
            RefreshDistros();
        }
        private void DeactivateAllButtons()
        {
            InstallUninstallButton.IsEnabled = false;
            LaunchButton.IsEnabled = false;
            RunCommandButton.IsEnabled = false;
            TerminateButton.IsEnabled = false;
            SetDefaultButton.IsEnabled = false;
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

        private void WSLVersionSelected1_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(1);
        }
        private void WSLVersionSelected2_Checked(object sender, RoutedEventArgs e)
        {
            wsli.SetDefaultVersion(2);
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
                if (selectedDistro.State == "Running") TerminateButton.IsEnabled = true;
                if (selectedDistro.State == "Stopped") TerminateButton.IsEnabled = false;
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
            wsli.OpenDistro(((Distro)DistroList.SelectedItem).Name);
        }

        //private void NotImplementedException()
        //{
        //    throw new NotImplementedException();
        //}

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
            Thread.Sleep(500);
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
        //public string GetSelectedDistro()
        //{
        //    return ((Distro)DistroList.SelectedItem).Name;
        //}
    }

}