using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using wslcontrol_gui.Pages;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;




namespace wslcontrol_gui
{

    public partial class MainWindow : Window
    {
        readonly WSLInterface wsli = new();
        public MainWindow()
        {
            InitializeComponent();
            if (!wsli.CheckResponseIfInstalled())
            {
                wslcontrol_gui.Pages.InitialSetup firstsetupguide = new(wsli);
                firstsetupguide.ShowDialog();//is my job done in this class? idk
                Environment.Exit(0);//here exit after finishing
            }
            try
            {
                SetInitialStatuses();
            }
            catch (System.IndexOutOfRangeException)
            {
                //The code below is old routines for handling moments when WSL is not set up or some other error is encountered
                MessageBox.Show(
                    "Parsing error. WSL seems to not be installed or no distros are installed. " +
                    "\nIf you just ran the initial setup wizard or install the WSL yourself, \n" +
                    "You should try rebooting the system 2 times, run your first installed distribution, and try again.");
                //try //basically the code from online install onclick function
                //{
                //    OnlineInstall installwindow = new(wsli);
                //    installwindow.ShowDialog();
                //    Close();
                //}
                //catch (Exception ex)//if even this fails
                //{
                //    MessageBox.Show(ex.Message);
                //    Close();
                //}
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
            ThemeResolver.SetTheme();
            CheckVersion();
        }
        private void CheckVersion()
        {
            AssemblyInfo assemblyInfo = new(Assembly.GetExecutingAssembly());
            var appver = assemblyInfo.Version.ToString();
            var wslver = OsInfo.WSLVersion();
            if (wslver==OsInfo.testedWSLVersion)
            {
                WSLVersionWarningLabel.Text = "v" + appver+ " WSL Version: " + wslver;
            }
            else
            {
                WSLVersionWarningLabel.Text = "v" + appver + " This version was tested with WSL v" + OsInfo.testedWSLVersion + ", current version: " + wslver;
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
            ThisDistroSettingsButton.IsEnabled = false;
            ExportTar.IsEnabled = false;
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
                ExportTar.IsEnabled = true;
                if (selectedDistro.State == "Running") { TerminateButton.IsEnabled = true; }
                if (selectedDistro.State == "Stopped") { TerminateButton.IsEnabled = false; }
                if (OsInfo.GetOsBuild() < 19041) { GlobalSettingsButton.IsEnabled = false; }
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
                    ThisDistroSettingsButton.IsEnabled = false;
                }
                if ((selectedDistro.Name == "docker-desktop") || (selectedDistro.Name == "docker-desktop-data"))
                //disable some options for docker
                {
                    ThisDistroSettingsButton.IsEnabled = false;
                    LaunchButton.IsEnabled = false;
                    RunCommandButton.IsEnabled = false;
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
            inputwindow.ShowDialog();
        }

        private void TerminateButton_Click(object sender, RoutedEventArgs e)
        {
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            wsli.TerminateDistro(selectedDistro.Name);
            WaitAndRefresh();
        }
        private void WaitAndRefresh()
        {
            Thread.Sleep(250);//long operation
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
            try
            {
                RefreshDistros();
            }
            catch { Close(); };

        }
        private void OpenThisInExplorer_Click(object sender, RoutedEventArgs e)
        {
            int selectedDistroNumber = ((Distro)DistroList.SelectedItem).Number;
            try
            {
                RefreshDistros();
            }
            catch { Close(); }
            DistroList.SelectedIndex = selectedDistroNumber - 1;
            Distro selectedDistro = (Distro)DistroList.SelectedItem;
            if ((selectedDistro != null) && (selectedDistro.State == "Running"))
            {
                var _ = new ShellExecuteBp(selectedDistro.Name);
            }
            else if ((selectedDistro != null) && OsInfo.GetOsBuild() > 22000) //W11 doesn't need the distro running
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
            inputwindow.ShowDialog();
        }

        private void OnlineInstall_Click(object sender, RoutedEventArgs e)
        {
            OnlineInstall installwindow = new(wsli)
            {
                Owner = this
            };
            installwindow.ShowDialog();
        }

        private void ImportTar_Click(object sender, RoutedEventArgs e)
        {
            #region import file selection
            OpenFileDialog openFile = new()
            {
                Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx",
                Multiselect = false,
                Title = "Select file to import..."
            };
            openFile.ShowDialog(this);
            string importFile = openFile.FileName;
            #endregion
            #region distro name and wsl version selection
            ImportVersionAlert importDialog = new()
            {
                Owner = this
            };
            importDialog.ShowDialog();
            if (importDialog.WSLVersion == 0) return;
            string distroName = importDialog.DistroName;
            int wslVersion = importDialog.WSLVersion;
            #endregion
            #region install location selection
            FolderBrowserDialog installLocationSelector = new()
            {
                Description = "Select installation location",
                ShowNewFolderButton = true
            };
            installLocationSelector.ShowDialog();
            string installLocation = installLocationSelector.SelectedPath;
            #endregion
            #region pass to backend
            if (Path.GetExtension(importFile) == ".tar")
            {
                wsli.ImportDistro(distroName, importFile, installLocation, DistType.tar, wslVersion);
            }
            else if (Path.GetExtension(importFile) == ".vhdx")
            {
                wsli.ImportDistro(distroName, importFile, installLocation, DistType.vhdx, wslVersion);
            }
            #endregion
        }

        private void ExportTar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new()
            {
                Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx",
                Title = "Select path and filename to export...",
                AddExtension = true
            };
            saveFile.ShowDialog(this);
            string exportFile = saveFile.FileName;
            string distroName = ((Distro)DistroList.SelectedItem).Name;
            if (Path.GetExtension(exportFile) == ".tar")
            {
                wsli.ExportDistro(distroName, exportFile, DistType.tar);//long operation
            }
            else if (Path.GetExtension(exportFile) == ".vhdx")
            {
                wsli.ExportDistro(distroName, exportFile, DistType.vhdx);//long operation
            }
        }



        private void ThisDistroSettings_Click(object sender, RoutedEventArgs e)
        {
            PerDistroPrefs inputwindow = new((Distro)DistroList.SelectedItem, wsli)
            {
                Owner = this
            };
            inputwindow.ShowDialog();
        }

        private void MountDriveButton_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
