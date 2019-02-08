using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Repositories;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для DoctorWindow.xaml
    /// </summary>
    public partial class DoctorWindow : Window
    {
        private Doctor LocalDoctor { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public DoctorWindow()
        {
            InitializeComponent();

            LocalDoctor = new Doctor();
            Action = ActionType.Create;

            SetContent();
            SetButtonHandlers();
        }

        public DoctorWindow(Doctor doctor, ActionType action)
        {
            InitializeComponent();

            LocalDoctor = doctor;
            Action = action;

            SetContent();
            SetButtonHandlers();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування лікаря";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення лікаря";
                WorkButton.Content = "Створити";
            }

            FirstNameBox.Text = LocalDoctor.FirstName;
            MiddleNameBox.Text = LocalDoctor.MiddleName;
            LastNameBox.Text = LocalDoctor.LastName;
            PositionBox.Text = LocalDoctor.Position;
        }

        public void SetButtonHandlers()
        {
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                LocalDoctor.FirstName = FirstNameBox.Text;
                LocalDoctor.MiddleName = MiddleNameBox.Text;
                LocalDoctor.LastName = LastNameBox.Text;
                LocalDoctor.Position = PositionBox.Text;

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Doctors.Update(LocalDoctor);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Doctors.Create(LocalDoctor);
                    unitOfWork.Save();
                    Close();
                }
            };
        }

        public string ActionText(ActionType action)
        {
            if (action == ActionType.Create)
                return "створити";
            else if (action == ActionType.Edit)
                return "редагувати";
            return String.Empty;
        }
    }
}
