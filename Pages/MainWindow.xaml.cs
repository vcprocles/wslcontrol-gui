using System;
using System.IO;
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
        readonly OsInfo os = new();
        //private bool RunInstaller=false;
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                SetInitialStatuses();
            }
            catch (System.IndexOutOfRangeException)
            {
                MessageBox.Show(
                    "Parsing error. WSL seems to not be installed or no distros are installed. " +
                    "\nWill now try running distro installer. If this fails, run \"wsl --install\" in the terminal and try again");
                try //basically the code from online install onclick function
                {
                    OnlineInstall installwindow = new(wsli);
                    installwindow.ShowDialog();
                    Close();
                }
                catch (Exception ex)//if even this fails
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
            ThemeResolver.SetTheme();
        }
        WindowLayoutChangeSaveData layoutSaveData;
        private void SetInitialStatuses()
        {
            DeactivateAllButtons();
            RefreshDistros();
        }
        private void CollapseWindow()
        {

            layoutSaveData.distroList = DistroList.Visibility;
            layoutSaveData.mainWinMinWidth = MinWidth;
            layoutSaveData.mainWinWidth = Width;
            layoutSaveData.mainWinResizeMode = ResizeMode;
            layoutSaveData.mainWinMaxWidth = MaxWidth;
            layoutSaveData.mainWinHeight = Height;
            layoutSaveData.mainWinMinHeight = MinHeight;
            layoutSaveData.distroListSelectedIndex = DistroList.SelectedIndex;
            layoutSaveData.selDistroLabelVisibility = SelectedDistroLabel.Visibility;
            layoutSaveData.allDistroLabelVisibility = AllDistroLabel.Visibility;

            if (DistroList.Items.Count == 1)
            {
                DistroList.Visibility = Visibility.Collapsed;
                MinWidth = 231;
                Width = 231;
                ResizeMode = ResizeMode.NoResize;
                MaxWidth = 231;
                Height = 516;
                MinHeight = 516;
                DistroList.SelectedIndex = 0;
                SelectedDistroLabel.Visibility = Visibility.Collapsed;
                AllDistroLabel.Visibility = Visibility.Collapsed;
                WSL2WarningLabel.Visibility = Visibility.Collapsed; //bc it will break the layout
            }
            else
            {
                DistroList.Visibility = layoutSaveData.distroList;
                MinWidth = layoutSaveData.mainWinMinWidth;
                Width = layoutSaveData.mainWinWidth;
                ResizeMode = layoutSaveData.mainWinResizeMode;
                MaxWidth = layoutSaveData.mainWinMaxWidth;
                Height = layoutSaveData.mainWinHeight;
                MinHeight = layoutSaveData.mainWinMinHeight;
                DistroList.SelectedIndex = layoutSaveData.distroListSelectedIndex;
                SelectedDistroLabel.Visibility = layoutSaveData.selDistroLabelVisibility;
                AllDistroLabel.Visibility = layoutSaveData.allDistroLabelVisibility;
            }
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
            CollapseWindow();
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
                if (os.build < 19041) { GlobalSettingsButton.IsEnabled = false; }
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
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx";
            openFile.Multiselect = false;
            openFile.Title = "Select file to import...";
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
            FolderBrowserDialog installLocationSelector = new();
            installLocationSelector.Description = "Select installation location";
            installLocationSelector.ShowNewFolderButton = true;
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
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "WSL .tar(*.tar)|*.tar|VM Images (*.vhdx)|*.vhdx";
            saveFile.Title = "Select path and filename to export...";
            saveFile.AddExtension = true;
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
            PerDistroPrefs inputwindow = new PerDistroPrefs((Distro)DistroList.SelectedItem, wsli);
            inputwindow.Owner = this;
            inputwindow.ShowDialog();
        }
    }
    struct WindowLayoutChangeSaveData
    {
        public Visibility distroList;
        public double mainWinMinWidth;
        public double mainWinWidth;
        public ResizeMode mainWinResizeMode;
        public double mainWinMaxWidth;
        public double mainWinHeight;
        public double mainWinMinHeight;
        public int distroListSelectedIndex;
        public Visibility selDistroLabelVisibility;
        public Visibility allDistroLabelVisibility;
    }
}
