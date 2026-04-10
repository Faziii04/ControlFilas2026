using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlFilas2026.Models;

namespace ControlFilas2026;

public partial class MainWindow : Window
{
    private readonly LinkedList queue = new();
    private readonly Box[] boxes = new Box[7];

    private Button[] callButtons = null!;
    private Button[] freeButtons = null!;
    private TextBlock[] statusTexts = null!;
    private TextBlock[] currentTexts = null!;

    private bool darkTheme;
    private bool queueStarted;
    private int normalCounter;
    private int specialCounter;

    public MainWindow()
    {
        InitializeComponent();

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i] = new Box(i + 1);
        }

        InitializeLogin();
        InitializeDashboard();
        UpdateThemeButtonsText();
    }

    private void InitializeLogin()
    {
        LoginControl.LoginButton.Click += (_, _) => HandleLogin();
        LoginControl.ThemeToggleButton.Click += (_, _) => ToggleTheme();
    }

    private void InitializeDashboard()
    {
        ClienteControl.GetTicketButton.Click += (_, _) => TakeTicket(false);
        ClienteControl.GetSpecialTicketButton.Click += (_, _) => TakeTicket(true);

        callButtons = new Button[]
        {
            CajasControl.CallBox1Button,
            CajasControl.CallBox2Button,
            CajasControl.CallBox3Button,
            CajasControl.CallBox4Button,
            CajasControl.CallBox5Button,
            CajasControl.CallBox6Button,
            CajasControl.CallBox7Button
        };

        freeButtons = new Button[]
        {
            CajasControl.FreeBox1Button,
            CajasControl.FreeBox2Button,
            CajasControl.FreeBox3Button,
            CajasControl.FreeBox4Button,
            CajasControl.FreeBox5Button,
            CajasControl.FreeBox6Button,
            CajasControl.FreeBox7Button
        };

        statusTexts = new TextBlock[]
        {
            CajasControl.Box1StatusText,
            CajasControl.Box2StatusText,
            CajasControl.Box3StatusText,
            CajasControl.Box4StatusText,
            CajasControl.Box5StatusText,
            CajasControl.Box6StatusText,
            CajasControl.Box7StatusText
        };

        currentTexts = new TextBlock[]
        {
            CajasControl.Box1CurrentText,
            CajasControl.Box2CurrentText,
            CajasControl.Box3CurrentText,
            CajasControl.Box4CurrentText,
            CajasControl.Box5CurrentText,
            CajasControl.Box6CurrentText,
            CajasControl.Box7CurrentText
        };

        for (int i = 0; i < 7; i++)
        {
            int boxIndex = i;
            int boxNumber = i + 1;

            callButtons[i].Click += (_, _) => CallNext(boxIndex, boxNumber);
            freeButtons[i].Click += (_, _) => FreeBox(boxIndex);
            AdminControl.BoxComboBox.Items.Add($"Caja {boxNumber}");
        }

        AdminControl.BoxComboBox.SelectedIndex = 0;
        AdminControl.EnableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(true);
        AdminControl.DisableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(false);
        AdminControl.EnableAllButton.Click += (_, _) => EnableAllBoxes();
        AdminControl.StartQueueButton.Click += (_, _) => StartQueue();
        AdminControl.RestartQueueButton.Click += (_, _) => RestartAll();
        AdminControl.AdminThemeToggleButton.Click += (_, _) => ToggleTheme();
        AdminControl.BackToLoginButton.Click += (_, _) => BackToLogin();

        RefreshBoxes();
        RefreshQueueCount();
        RefreshRecentTickets();
    }

    private void HandleLogin()
    {
        string username = LoginControl.UsernameTextBox.Text;
        string password = LoginControl.PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Ingrese usuario y contraseña.");
            return;
        }

        if (username != "admin" || password != "123")
        {
            ShowError("Usuario o contraseña incorrectos.");
            return;
        }

        LoginControl.ErrorContainer.Visibility = Visibility.Collapsed;
        LoginControl.Visibility = Visibility.Collapsed;
        AdminControl.Visibility = Visibility.Visible;
        CajasControl.Visibility = Visibility.Visible;
        ClienteControl.Visibility = Visibility.Visible;
    }

    private void BackToLogin()
    {
        LoginControl.Visibility = Visibility.Visible;
        AdminControl.Visibility = Visibility.Collapsed;
        CajasControl.Visibility = Visibility.Collapsed;
        ClienteControl.Visibility = Visibility.Collapsed;
    }

    private void TakeTicket(bool isSpecial)
    {
        if (!queueStarted)
        {
            ShowError("La fila no está iniciada.");
            return;
        }

        Ticket ticket;

        if (isSpecial)
        {
            specialCounter++;
            ticket = new Ticket($"E-{specialCounter:000}", true);
            queue.EnqueueSpecial(ticket);
        }
        else
        {
            normalCounter++;
            ticket = new Ticket($"N-{normalCounter:000}", false);
            queue.EnqueueNormal(ticket);
        }

        ClienteControl.TicketNumberDisplay.Text = ticket.Number;
        RefreshQueueCount();
        RefreshRecentTickets();
    }

    private void CallNext(int boxIndex, int boxNumber)
    {
        if (!queueStarted)
        {
            ShowError("Primero inicie la fila.");
            return;
        }

        if (!boxes[boxIndex].IsEnabled)
        {
            ShowError($"Caja {boxNumber} deshabilitada.");
            return;
        }

        if (!string.IsNullOrEmpty(boxes[boxIndex].CurrentTicket))
        {
            ShowError($"Caja {boxNumber} está ocupada.");
            return;
        }

        if (!queue.TryDequeue(out Ticket? ticket) || ticket == null)
        {
            ShowError("No hay personas en espera.");
            return;
        }

        boxes[boxIndex].CurrentTicket = ticket.Number;
        LogCall($"Caja {boxNumber} llama a {ticket.Number}");

        RefreshBoxes();
        RefreshQueueCount();
        RefreshRecentTickets();
    }

    private void FreeBox(int boxIndex)
    {
        if (string.IsNullOrEmpty(boxes[boxIndex].CurrentTicket))
        {
            return;
        }

        boxes[boxIndex].CurrentTicket = null;
        RefreshBoxes();
    }

    private void SetSelectedBoxEnabled(bool enabled)
    {
        int index = AdminControl.BoxComboBox.SelectedIndex;

        if (index < 0 || index >= boxes.Length)
        {
            ShowError("Seleccione una caja válida.");
            return;
        }

        boxes[index].IsEnabled = enabled;

        if (!enabled)
        {
            boxes[index].CurrentTicket = null;
        }

        RefreshBoxes();
    }

    private void EnableAllBoxes()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].IsEnabled = true;
        }

        RefreshBoxes();
    }

    private void StartQueue()
    {
        queueStarted = true;
        AdminControl.AdminStatusText.Text = "Estado: Iniciada";
    }

    private void RestartAll()
    {
        queueStarted = false;
        normalCounter = 0;
        specialCounter = 0;
        queue.Clear();

        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].IsEnabled = true;
            boxes[i].CurrentTicket = null;
        }

        ClienteControl.CallsListBox.Items.Clear();
        ClienteControl.TicketNumberDisplay.Text = "--";
        AdminControl.AdminStatusText.Text = "Estado: Detenida";

        RefreshBoxes();
        RefreshQueueCount();
        RefreshRecentTickets();
    }

    private void RefreshQueueCount()
    {
        ClienteControl.QueuePositionText.Text = queue.Count.ToString();
    }

    private void RefreshBoxes()
    {
        SolidColorBrush successBrush = (SolidColorBrush)FindResource("BrushSuccess");
        SolidColorBrush warningBrush = (SolidColorBrush)FindResource("BrushWarning");
        SolidColorBrush errorBrush = (SolidColorBrush)FindResource("BrushError");

        for (int i = 0; i < 7; i++)
        {
            if (!boxes[i].IsEnabled)
            {
                statusTexts[i].Text = "Deshabilitada";
                statusTexts[i].Foreground = errorBrush;
                currentTexts[i].Text = "Sin cliente";
                continue;
            }

            if (string.IsNullOrEmpty(boxes[i].CurrentTicket))
            {
                statusTexts[i].Text = "Disponible";
                statusTexts[i].Foreground = successBrush;
                currentTexts[i].Text = "Sin cliente";
            }
            else
            {
                statusTexts[i].Text = "Atendiendo";
                statusTexts[i].Foreground = warningBrush;
                currentTexts[i].Text = boxes[i].CurrentTicket;
            }
        }
    }

    private void RefreshRecentTickets()
    {
        ObservableCollection<string> orderedTickets = new ObservableCollection<string>();
        LinkedList.Node? current = queue.Head;
        int position = 1;

        while (current != null)
        {
            orderedTickets.Add($"{position}. {current.Value.Number}");
            current = current.Next;
            position++;
        }

        ClienteControl.RecentTicketsListBox.ItemsSource = orderedTickets;
    }

    private void LogCall(string message)
    {
        ClienteControl.CallsListBox.Items.Insert(0, message);

        if (ClienteControl.CallsListBox.Items.Count > 40)
        {
            ClienteControl.CallsListBox.Items.RemoveAt(40);
        }
    }

    private void ShowError(string message)
    {
        MessageBox.Show(message, "Control de Filas", MessageBoxButton.OK, MessageBoxImage.Warning);
        LoginControl.ErrorContainer.Visibility = Visibility.Collapsed;
    }

    private void ToggleTheme()
    {
        darkTheme = !darkTheme;

        var appResources = Application.Current.Resources;
        var dictionaries = appResources.MergedDictionaries;
        dictionaries.Clear();

        var source = darkTheme
            ? new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative)
            : new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative);

        dictionaries.Add(new ResourceDictionary { Source = source });

        UpdateThemeButtonsText();
    }

    private void UpdateThemeButtonsText()
    {
        string text = darkTheme ? "Tema claro" : "Tema oscuro";
        LoginControl.ThemeToggleButton.Content = text;
        AdminControl.AdminThemeToggleButton.Content = text;
    }
}
