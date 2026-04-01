using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ControlFilas2026.Models;

namespace ControlFilas2026.UserControls
{
    /// <summary>
    /// Interaction logic for AdministracionUC.xaml
    /// </summary>
    public partial class AdministracionUC : UserControl
    {
        private ObservableCollection<Box> boxes;

        public AdministracionUC()
        {
            InitializeComponent();
            InitializeBoxes();
        }

        private void InitializeBoxes()
        {
            boxes = new ObservableCollection<Box>();
            
            for (int i = 1; i <= 7; i++)
            {
                boxes.Add(new Box(i));
            }

            BoxComboBox.ItemsSource = boxes;
            BoxComboBox.DisplayMemberPath = "Name";
            if (boxes.Count > 0)
            {
                BoxComboBox.SelectedIndex = 0;
            }
        }

        public ObservableCollection<Box> Boxes
        {
            get { return boxes; }
        }

        private void EnableAllButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

