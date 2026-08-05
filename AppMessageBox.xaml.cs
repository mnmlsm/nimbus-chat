using System.Windows;

namespace NimbusChat
{
    public enum AppMessageBoxIcon
    {
        Info,
        Success,
        Error
    }

    public partial class AppMessageBox : Window
    {
        public AppMessageBox(string message, string title, AppMessageBoxIcon icon)
        {
            InitializeComponent();

            Title = title;
            TitleBlock.Text = title;
            MessageBlock.Text = message;

            switch (icon)
            {
                case AppMessageBoxIcon.Success:
                    IconElement.Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckCircleOutline;
                    break;

                case AppMessageBoxIcon.Error:
                    IconElement.Kind = MaterialDesignThemes.Wpf.PackIconKind.AlertCircleOutline;
                    break;

                default:
                    IconElement.Kind = MaterialDesignThemes.Wpf.PackIconKind.InformationOutline;
                    break;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void Show(string message, string title = "NimbusChat", AppMessageBoxIcon icon = AppMessageBoxIcon.Info, Window owner = null)
        {
            var box = new AppMessageBox(message, title, icon);

            if (owner != null)
                box.Owner = owner;

            box.ShowDialog();
        }
    }
}
