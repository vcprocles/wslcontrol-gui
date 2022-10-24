using System.Windows;

namespace wslcontrol_gui.Pages
{
    /// <summary>
    /// Interaction logic for ImportVersionAlert.xaml
    /// </summary>
    public partial class ImportVersionAlert : Window
    {
        public ImportVersionAlert()
        {
            InitializeComponent();
            DistroName = DistroNameBox.Text;
            WSLVersion = 0;
        }
        public string DistroName { get; set; }
        public int WSLVersion { get; set; }

        private void WSL1Button_Click(object sender, RoutedEventArgs e)
        {
            WSLVersion = 1;
            DistroName = DistroNameBox.Text;
            this.Close();
        }

        private void WSL2Button_Click(object sender, RoutedEventArgs e)
        {
            WSLVersion = 2;
            DistroName = DistroNameBox.Text;
            this.Close();
        }
    }
}
