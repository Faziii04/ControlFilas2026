using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlFilas2026
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeLogin();
        }

        private void InitializeLogin()
        {
            // Wire up login button
            LoginControl.LoginButton.Click += (s, e) => HandleLogin();
        }

        private void HandleLogin()
        {
            // Simple login validation (for now, accept any non-empty credentials)
            string username = LoginControl.UsernameTextBox.Text;
            string password = LoginControl.PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LoginControl.ErrorMessage.Text = "Por favor ingrese usuario y contraseña";
                LoginControl.ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            // On successful login, show all three sections
            LoginControl.Visibility = Visibility.Collapsed;
            CajasControl.Visibility = Visibility.Visible;
            AdminControl.Visibility = Visibility.Visible;
            ClienteControl.Visibility = Visibility.Visible;
        }
    }
}