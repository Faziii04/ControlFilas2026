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
using System;

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
            LoginControl.LoginButton.Click += (s, e) => HandleLogin();
        }

        private void HandleLogin()
        {
            string username = LoginControl.UsernameTextBox.Text;
            string password = LoginControl.PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                LoginControl.ErrorMessage.Text = "Por favor ingrese usuario y contraseña";
                LoginControl.ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            LoginControl.Visibility = Visibility.Collapsed;
            CajasControl.Visibility = Visibility.Visible;
            AdminControl.Visibility = Visibility.Visible;
            ClienteControl.Visibility = Visibility.Visible;
        }

        private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeComboBox == null || ThemeComboBox.SelectedIndex == -1)
                return;

            var selectedItem = (ComboBoxItem)ThemeComboBox.SelectedItem;
            if (selectedItem == null)
                return;

            string theme = selectedItem.Content.ToString();

            var appResources = Application.Current.Resources;
            appResources.MergedDictionaries.Clear();

            var newTheme = new ResourceDictionary();

            if (theme == "Oscuro")
            {
                newTheme.Source = new Uri("Resources/DarkTheme.xaml", UriKind.Relative);
            }
            else
            {
                newTheme.Source = new Uri("Resources/LightTheme.xaml", UriKind.Relative);
            }

            appResources.MergedDictionaries.Add(newTheme);

            this.InvalidateVisual();
            this.UpdateLayout();
        }
    }
}