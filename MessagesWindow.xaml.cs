using System.Windows;

namespace NimbusChat
{
    public partial class MessagesWindow : Window
    {
        public MessagesWindow()
        {
            InitializeComponent();
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                MessageBox.Show("Enter a message");
                return;
            }

            ChatList.Items.Add("You: " + MessageInput.Text);
            MessageInput.Clear();
        }
    }
}