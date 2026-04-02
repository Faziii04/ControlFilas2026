using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ControlFilas2026.Helpers;
using ControlFilas2026.Models;

namespace ControlFilas2026;

public partial class MainWindow : Window
{
    private readonly DoublyLinkedList queue = new DoublyLinkedList();
    private readonly Box[] boxes = new Box[7];

    private Button[] callButtons = null!;
    private Button[] freeButtons = null!;
    private TextBlock[] statusTexts = null!;
    private TextBlock[] currentTexts = null!;

    private bool isDarkTheme;
    private bool queueRunning;
    private int normalCounter;
    private int specialCounter;

    public MainWindow()
    {
        InitializeComponent();
        for (int i = 0; i < boxes.Length; i++) boxes[i] = new Box(i + 1);
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
        ClienteControl.GetTicketButton.Click += (_, _) => EnqueueCustomer(false);
        ClienteControl.GetSpecialTicketButton.Click += (_, _) => EnqueueCustomer(true);

        callButtons = new Button[]
        {
            CajasControl.CallBox1Button, CajasControl.CallBox2Button, CajasControl.CallBox3Button, CajasControl.CallBox4Button,
            CajasControl.CallBox5Button, CajasControl.CallBox6Button, CajasControl.CallBox7Button
        };
        freeButtons = new Button[]
        {
            CajasControl.FreeBox1Button, CajasControl.FreeBox2Button, CajasControl.FreeBox3Button, CajasControl.FreeBox4Button,
            CajasControl.FreeBox5Button, CajasControl.FreeBox6Button, CajasControl.FreeBox7Button
        };
        statusTexts = new TextBlock[]
        {
            CajasControl.Box1StatusText, CajasControl.Box2StatusText, CajasControl.Box3StatusText, CajasControl.Box4StatusText,
            CajasControl.Box5StatusText, CajasControl.Box6StatusText, CajasControl.Box7StatusText
        };
        currentTexts = new TextBlock[]
        {
            CajasControl.Box1CurrentText, CajasControl.Box2CurrentText, CajasControl.Box3CurrentText, CajasControl.Box4CurrentText,
            CajasControl.Box5CurrentText, CajasControl.Box6CurrentText, CajasControl.Box7CurrentText
        };

        for (int i = 0; i < 7; i++)
        {
            int boxNumber = i + 1;
            callButtons[i].Click += (_, _) => CallNextForBox(boxNumber);
            freeButtons[i].Click += (_, _) => FreeBox(boxNumber);
            AdminControl.BoxComboBox.Items.Add($"Caja {boxNumber}");
        }

        AdminControl.BoxComboBox.SelectedIndex = 0;
        AdminControl.EnableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(true);
        AdminControl.DisableBoxButton.Click += (_, _) => SetSelectedBoxEnabled(false);
        AdminControl.EnableAllButton.Click += (_, _) => { foreach (var b in boxes) b.IsEnabled = true; RefreshBoxes(); };
        AdminControl.StartQueueButton.Click += (_, _) => { queueRunning = true; AdminControl.AdminStatusText.Text = "Estado: Iniciada"; AddCallMessage("Fila iniciada."); };
        AdminControl.RestartQueueButton.Click += (_, _) => RestartQueue();

        RefreshBoxes();
        RefreshQueueCount();
    }

    private void HandleLogin()
    {
        var username = LoginControl.UsernameTextBox.Text;
        var password = LoginControl.PasswordBox.Password;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Por favor ingrese usuario y contraseña");
            return;
        }

        if (username != "admin" || password != "123")
        {
            ShowError("Usuario o contraseña incorrectos");
            return;
        }

        LoginControl.ErrorContainer.Visibility = Visibility.Collapsed;
        LoginControl.Visibility = Visibility.Collapsed;
        AdminControl.Visibility = Visibility.Visible;
        CajasControl.Visibility = Visibility.Visible;
        ClienteControl.Visibility = Visibility.Visible;
    }

    private void EnqueueCustomer(bool isSpecial)
    {
        if (!queueRunning) { AddCallMessage("La fila no está iniciada."); return; }

        var ticket = isSpecial
            ? new Ticket($"E-{++specialCounter:000}", true)
            : new Ticket($"N-{++normalCounter:000}", false);

        if (isSpecial) queue.InsertAt(queue.FindInsertIndexForSpecial(), ticket);
        else queue.AddLast(ticket);

        ClienteControl.TicketNumberDisplay.Text = ticket.Number;
        AddCallMessage($"Turno tomado: {ticket.Number}");
        RefreshQueueCount();
    }

    private void CallNextForBox(int boxNumber)
    {
        var box = boxes[boxNumber - 1];

        if (!queueRunning) { AddCallMessage("Inicie la fila para llamar clientes."); return; }
        if (!box.IsEnabled) { AddCallMessage($"Caja {boxNumber} está deshabilitada."); return; }
        if (!string.IsNullOrEmpty(box.CurrentTicket)) { AddCallMessage($"Caja {boxNumber} ya está atendiendo {box.CurrentTicket}."); return; }
        if (!queue.TryRemoveFirst(out Ticket? ticket) || ticket is null) { AddCallMessage("No hay clientes en espera."); return; }

        box.CurrentTicket = ticket.Number;
        AddCallMessage($"Caja {boxNumber} llama a {ticket.Number}");
        RefreshBoxes();
        RefreshQueueCount();
    }

    private void FreeBox(int boxNumber)
    {
        var box = boxes[boxNumber - 1];
        if (string.IsNullOrEmpty(box.CurrentTicket)) return;

        AddCallMessage($"Caja {boxNumber} finalizó {box.CurrentTicket}");
        box.CurrentTicket = null;
        RefreshBoxes();
    }

    private void SetSelectedBoxEnabled(bool enabled)
    {
        int idx = AdminControl.BoxComboBox.SelectedIndex;
        if (idx < 0 || idx >= boxes.Length) return;

        boxes[idx].IsEnabled = enabled;
        if (!enabled) boxes[idx].CurrentTicket = null;
        RefreshBoxes();
    }

    private void RestartQueue()
    {
        queueRunning = false;
        normalCounter = 0;
        specialCounter = 0;
        while (queue.TryRemoveFirst(out _)) { }

        foreach (var box in boxes)
        {
            box.IsEnabled = true;
            box.CurrentTicket = null;
        }

        ClienteControl.CallsListBox.Items.Clear();
        ClienteControl.TicketNumberDisplay.Text = "--";
        AdminControl.AdminStatusText.Text = "Estado: Detenida";
        RefreshBoxes();
        RefreshQueueCount();
        AddCallMessage("Fila reiniciada.");
    }

    private void RefreshQueueCount() => ClienteControl.QueuePositionText.Text = queue.Count.ToString();

    private void RefreshBoxes()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            if (!box.IsEnabled)
            {
                statusTexts[i].Text = "Deshabilitada";
                statusTexts[i].Foreground = (SolidColorBrush)FindResource("BrushError");
                currentTexts[i].Text = "Sin cliente";
                continue;
            }

            if (string.IsNullOrEmpty(box.CurrentTicket))
            {
                statusTexts[i].Text = "Disponible";
                statusTexts[i].Foreground = (SolidColorBrush)FindResource("BrushSuccess");
                currentTexts[i].Text = "Sin cliente";
            }
            else
            {
                statusTexts[i].Text = "Atendiendo";
                statusTexts[i].Foreground = (SolidColorBrush)FindResource("BrushWarning");
                currentTexts[i].Text = box.CurrentTicket;
            }
        }
    }

    private void AddCallMessage(string message)
    {
        ClienteControl.CallsListBox.Items.Insert(0, message);
        if (ClienteControl.CallsListBox.Items.Count > 30) ClienteControl.CallsListBox.Items.RemoveAt(30);
    }

    private void ShowError(string message)
    {
        LoginControl.ErrorMessage.Text = message;
        LoginControl.ErrorContainer.Visibility = Visibility.Visible;
    }

    private void ToggleTheme()
    {
        isDarkTheme = !isDarkTheme;
        ThemeHelper.ApplyTheme(isDarkTheme);
        LoginControl.ThemeToggleButton.Content = isDarkTheme ? "☀️ Tema Claro" : "🌙 Tema Oscuro";
    }
}