using System.Windows;
using System.Windows.Controls;

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
            var text = MessageInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            ChatList.Items.Add(text);
            MessageInput.Clear();
        }
    }
}