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
            MessageBox.Show("Profile saved, " + UsernameBox.Text + "!");
        }
    }
}
