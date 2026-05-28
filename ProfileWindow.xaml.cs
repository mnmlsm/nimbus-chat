using System.Windows;

namespace NimbusChat
{
    public partial class ProfileWindow : Window
    {
        public ProfileWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Hier kannst du später Daten speichern, z. B. in Settings oder DB
            MessageBox.Show("Profile saved!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
    }
}