using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();
            SetHandlers();
        }

        public DialogWindow(string message)
        {
            InitializeComponent();
            SetHandlers();
            MessageBlock.Text = message;
        }

        public void SetHandlers()
        {
            CancelButton.Click += (object sender, RoutedEventArgs e) =>
            {
                DialogResult = false;
                Close();           
            };

            OkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                DialogResult = true;
                Close();
            };
        }
    }
}
