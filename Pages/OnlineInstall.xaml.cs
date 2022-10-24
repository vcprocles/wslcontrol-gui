using System.Windows;

namespace wslcontrol_gui
{
    /// <summary>
    /// Interaction logic for OnlineInstall.xaml
    /// </summary>
    public partial class OnlineInstall : Window
    {
        private WSLInterface wsli;
        public OnlineInstall(WSLInterface wslImport)
        {
            InitializeComponent();
            wsli = wslImport;
            OnlineDistroList.ItemsSource = wsli.GetOnlineDistros();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            wsli.InstallOnlineDistro(((Distro)OnlineDistroList.SelectedItem).Name);//long operation
            this.Close();
        }
    }
}
