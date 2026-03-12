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
    /// Interaction logic for CajasUC.xaml
    /// </summary>
    public partial class CajasUC : UserControl
    {
        private ObservableCollection<Box> boxes;

        public CajasUC()
        {
            InitializeComponent();
            InitializeBoxes();
        }

        private void InitializeBoxes()
        {
            boxes = new ObservableCollection<Box>();
            
            // Create 7 boxes
            for (int i = 1; i <= 7; i++)
            {
                boxes.Add(new Box(i));
            }

            BoxesItemsControl.ItemsSource = boxes;
        }

        public ObservableCollection<Box> Boxes
        {
            get { return boxes; }
        }
    }
}

