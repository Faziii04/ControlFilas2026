using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlFilas2026.Helpers;
using ControlFilas2026.Models;

namespace ControlFilas2026;

public partial class MainWindow : Window
{
    private readonly LinkedList queue = new();

    private readonly bool[] boxEnabled = new bool[7];
    private readonly string?[] boxCurrentTicket = new string?[7];

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

        for (int i = 0; i < boxEnabled.Length; i++)
        {
            boxEnabled[i] = true;
        }

        InitializeLogin();
        InitializeDashboard();
    }

    private void InitializeLogin()
    {
        LoginControl.LoginButton.Click += (_, _) => HandleLogin();
        LoginControl.ThemeToggleButton.Click += (_, _) => ToggleTheme();
    }

    private void InitializeDashboard()
    {
        ClienteControl.GetTicketButton.Click += (_, _) => TakeTicket(isSpecial: false);
        ClienteControl.GetSpecialTicketButton.Click += (_, _) => TakeTicket(isSpecial: true);

        callButtons =
        [
            CajasControl.CallBox1Button,
            CajasControl.CallBox2Button,
            CajasControl.CallBox3Button,
            CajasControl.CallBox4Button,
            CajasControl.CallBox5Button,
            CajasControl.CallBox6Button,
            CajasControl.CallBox7Button
        ];

        freeButtons =
        [
            CajasControl.FreeBox1Button,
            CajasControl.FreeBox2Button,
            CajasControl.FreeBox3Button,
            CajasControl.FreeBox4Button,
            CajasControl.FreeBox5Button,
            CajasControl.FreeBox6Button,
            CajasControl.FreeBox7Button
        ];

        statusTexts =
        [
            CajasControl.Box1StatusText,
            CajasControl.Box2StatusText,
            CajasControl.Box3StatusText,
            CajasControl.Box4StatusText,
            CajasControl.Box5StatusText,
            CajasControl.Box6StatusText,
            CajasControl.Box7StatusText
        ];

        currentTexts =
        [
            CajasControl.Box1CurrentText,
            CajasControl.Box2CurrentText,
            CajasControl.Box3CurrentText,
            CajasControl.Box4CurrentText,
            CajasControl.Box5CurrentText,
            CajasControl.Box6CurrentText,
            CajasControl.Box7CurrentText
        ];

        for (int i = 0; i < 7; i++)
        {
            int boxIndex = i;
            int boxNumber = i + 1;

            callButtons[i].Click += (_, _) => CallNext(boxIndex, boxNumber);
            freeButtons[i].Click += (_, _) => FreeBox(boxIndex, boxNumber);
            AdminControl.BoxComboBox.Items.Add($"Caja {boxNumber}");
        }

        AdminControl.BoxComboBox.SelectedIndex = 0;
        AdminControl.EnableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(true);
        AdminControl.DisableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(false);
        AdminControl.EnableAllButton.Click += (_, _) => EnableAllBoxes();
        AdminControl.StartQueueButton.Click += (_, _) => StartQueue();
        AdminControl.RestartQueueButton.Click += (_, _) => RestartAll();

        RefreshBoxes();
        RefreshQueueCount();
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

    private void TakeTicket(bool isSpecial)
    {
        if (!queueStarted)
        {
            ShowInfo("La fila no está iniciada.");
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
        ShowInfo($"Turno generado: {ticket.Number}");
        RefreshQueueCount();
    }

    private void CallNext(int boxIndex, int boxNumber)
    {
        if (!queueStarted)
        {
            ShowInfo("Primero inicie la fila.");
            return;
        }

        if (!boxEnabled[boxIndex])
        {
            ShowInfo($"Caja {boxNumber} deshabilitada.");
            return;
        }

        if (!string.IsNullOrEmpty(boxCurrentTicket[boxIndex]))
        {
            ShowInfo($"Caja {boxNumber} está ocupada.");
            return;
        }

        if (!queue.TryDequeue(out Ticket? ticket) || ticket is null)
        {
            ShowInfo("No hay personas en espera.");
            return;
        }

        boxCurrentTicket[boxIndex] = ticket.Number;
        LogCall($"Caja {boxNumber} llama a {ticket.Number}");

        RefreshBoxes();
        RefreshQueueCount();
    }

    private void FreeBox(int boxIndex, int boxNumber)
    {
        if (string.IsNullOrEmpty(boxCurrentTicket[boxIndex]))
        {
            return;
        }

        LogCall($"Caja {boxNumber} finalizó {boxCurrentTicket[boxIndex]}");
        boxCurrentTicket[boxIndex] = null;

        RefreshBoxes();
    }

    private void SetSelectedBoxEnabled(bool enabled)
    {
        int index = AdminControl.BoxComboBox.SelectedIndex;

        if (index < 0 || index >= boxEnabled.Length)
        {
            ShowInfo("Seleccione una caja válida.");
            return;
        }

        boxEnabled[index] = enabled;

        if (!enabled)
        {
            boxCurrentTicket[index] = null;
        }

        RefreshBoxes();
        ShowInfo(enabled ? "Caja habilitada." : "Caja deshabilitada.");
    }

    private void EnableAllBoxes()
    {
        for (int i = 0; i < boxEnabled.Length; i++)
        {
            boxEnabled[i] = true;
        }

        RefreshBoxes();
        ShowInfo("Todas las cajas fueron habilitadas.");
    }

    private void StartQueue()
    {
        queueStarted = true;
        AdminControl.AdminStatusText.Text = "Estado: Iniciada";
        ShowInfo("Fila iniciada.");
    }

    private void RestartAll()
    {
        queueStarted = false;
        normalCounter = 0;
        specialCounter = 0;
        queue.Clear();

        for (int i = 0; i < boxEnabled.Length; i++)
        {
            boxEnabled[i] = true;
            boxCurrentTicket[i] = null;
        }

        ClienteControl.CallsListBox.Items.Clear();
        ClienteControl.TicketNumberDisplay.Text = "--";
        AdminControl.AdminStatusText.Text = "Estado: Detenida";

        RefreshBoxes();
        RefreshQueueCount();
        ShowInfo("Sistema reiniciado.");
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
            if (!boxEnabled[i])
            {
                statusTexts[i].Text = "Deshabilitada";
                statusTexts[i].Foreground = errorBrush;
                currentTexts[i].Text = "Sin cliente";
                continue;
            }

            if (string.IsNullOrEmpty(boxCurrentTicket[i]))
            {
                statusTexts[i].Text = "Disponible";
                statusTexts[i].Foreground = successBrush;
                currentTexts[i].Text = "Sin cliente";
            }
            else
            {
                statusTexts[i].Text = "Atendiendo";
                statusTexts[i].Foreground = warningBrush;
                currentTexts[i].Text = boxCurrentTicket[i];
            }
        }
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

    private void ShowInfo(string message)
    {
        MessageBox.Show(message, "Control de Filas", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void ToggleTheme()
    {
        darkTheme = !darkTheme;
        ThemeHelper.ApplyTheme(darkTheme);
        LoginControl.ThemeToggleButton.Content = darkTheme ? "Tema claro" : "Tema oscuro";
    }
}
