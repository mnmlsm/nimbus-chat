using System.Windows;

namespace NimbusChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmailBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Please enter email and password");
            }
            else
            {
                MessageBox.Show("Welcome!");
            }
        }
    }
}
