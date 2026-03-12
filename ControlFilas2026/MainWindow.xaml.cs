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
        private bool isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeLogin();
        }

        private void InitializeLogin()
        {
            // Wire up login button
            LoginControl.LoginButton.Click += (s, e) => HandleLogin();
            
            // Wire up theme toggle button
            LoginControl.ThemeToggleButton.Click += (s, e) => ToggleTheme();
        }

        private void HandleLogin()
        {
            // Simple login validation
            string username = LoginControl.UsernameTextBox.Text;
            string password = LoginControl.PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Por favor ingrese usuario y contraseña");
                return;
            }

            if (username == "admin" && password == "123")
            {
                // Hide error and login successful
                HideError();
                
                // Show all three sections
                LoginControl.Visibility = Visibility.Collapsed;
                CajasControl.Visibility = Visibility.Visible;
                AdminControl.Visibility = Visibility.Visible;
                ClienteControl.Visibility = Visibility.Visible;
            }
            else
            {
                ShowError("Usuario o contraseña incorrectos");
            }
        }

        private void ShowError(string message)
        {
            LoginControl.ErrorMessage.Text = message;
            LoginControl.ErrorContainer.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            LoginControl.ErrorContainer.Visibility = Visibility.Collapsed;
        }

        private void ToggleTheme()
        {
            isDarkTheme = !isDarkTheme;

            if (isDarkTheme)
            {
                ApplyDarkTheme();
                LoginControl.ThemeToggleButton.Content = "☀️ Tema Claro";
            }
            else
            {
                ApplyLightTheme();
                LoginControl.ThemeToggleButton.Content = "🌙 Tema Oscuro";
            }
        }

        private void ApplyDarkTheme()
        {
            // Update colors to dark theme
            Application.Current.Resources["ColorBackground"] = (Color)ColorConverter.ConvertFromString("#0F172A");
            Application.Current.Resources["ColorSurface"] = (Color)ColorConverter.ConvertFromString("#1E293B");
            Application.Current.Resources["ColorCard"] = (Color)ColorConverter.ConvertFromString("#334155");
            Application.Current.Resources["ColorText"] = (Color)ColorConverter.ConvertFromString("#F8FAFC");
            Application.Current.Resources["ColorTextSecondary"] = (Color)ColorConverter.ConvertFromString("#CBD5E1");
            Application.Current.Resources["ColorBorder"] = (Color)ColorConverter.ConvertFromString("#475569");
        }

        private void ApplyLightTheme()
        {
            // Restore light theme colors
            Application.Current.Resources["ColorBackground"] = (Color)ColorConverter.ConvertFromString("#F8FAFC");
            Application.Current.Resources["ColorSurface"] = (Color)ColorConverter.ConvertFromString("#FFFFFF");
            Application.Current.Resources["ColorCard"] = (Color)ColorConverter.ConvertFromString("#F1F5F9");
            Application.Current.Resources["ColorText"] = (Color)ColorConverter.ConvertFromString("#0F172A");
            Application.Current.Resources["ColorTextSecondary"] = (Color)ColorConverter.ConvertFromString("#64748B");
            Application.Current.Resources["ColorBorder"] = (Color)ColorConverter.ConvertFromString("#E2E8F0");
        }
    }
}