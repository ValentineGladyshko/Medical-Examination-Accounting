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
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Repositories;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для DoctorInfoWindow.xaml
    /// </summary>
    public partial class DoctorInfoWindow : Window
    {
        private Doctor Doctor { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public DoctorInfoWindow()
        {
            InitializeComponent();

            Doctor = new Doctor
            {
                Examinations = new List<Examination>()
            };
            SetContent();
            SetHandlers();
        }

        public DoctorInfoWindow(int id)
        {
            InitializeComponent();

            Doctor = unitOfWork.Doctors.Get(id);
            SetContent();
            SetHandlers();
        }

        private void SetContent()
        {
            LastNameBox.Text = "Прізвище: " + Doctor.LastName;
            FirstNameBox.Text = "Ім'я: " + Doctor.FirstName;
            MiddleNameBox.Text = "По-батькові: " + Doctor.MiddleName;
            PositionBox.Text = "Посада: " + Doctor.Position;
            
            ExaminationListBox.ItemsSource = Doctor.Examinations.Where(item => item.DeletedDate == null);
        }

        private void SetHandlers()
        {
            DeleteButton.Click += (object sender, RoutedEventArgs e) =>
            {
                string message = "Ви впевнені що хочете видалити лікаря "
                                 + Doctor.Position + " "
                                 + Doctor.LastName + " "
                                 + Doctor.FirstName
                                 + " " + Doctor.MiddleName + " ?";

                DialogWindow dialogWindow = new DialogWindow(message);
                bool? dialogResult = dialogWindow.ShowDialog();

                if (dialogResult != true)
                    return;

                Doctor.DeletedDate = DateTime.Now;
                unitOfWork.Doctors.Update(Doctor);
                unitOfWork.Save();

                foreach (var examination in Doctor.Examinations)
                {
                    examination.DeletedDate = DateTime.Now;
                    unitOfWork.Examinations.Update(examination);
                    foreach (var data in examination.ExaminationDatas)
                    {
                        data.DeletedDate = DateTime.Now;
                        unitOfWork.ExaminationDatas.Update(data);
                    }
                }
                unitOfWork.Save();

                Close();
            };

            EditButton.Click += (object sender, RoutedEventArgs e) =>
            {
                DoctorWindow doctorWindow = new DoctorWindow(Doctor, ActionType.Edit);
                doctorWindow.ShowDialog();

                SetContent();
            };

            ExaminationListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var examination = ExaminationListBox.SelectedItem as Examination;
                if (examination != null)
                {
                    ExaminationInfoWindow examinationInfoWindow = new ExaminationInfoWindow(examination.Id);
                    examinationInfoWindow.Owner = this;
                    examinationInfoWindow.Closed += (object o, EventArgs eventArgs) =>
                    {
                        SetContent();
                    };
                    examinationInfoWindow.Show();
                }
                ExaminationListBox.SelectedIndex = -1;

            };

            CreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var examinationToCreate = new Examination();
                examinationToCreate.Doctor = Doctor;
                examinationToCreate.DoctorId = Doctor.Id;
                examinationToCreate.ExaminationDate = DateTime.Now;
                ExaminationWindow examinationWindow = new ExaminationWindow(examinationToCreate, ActionType.Create);
                examinationWindow.Owner = this;
                examinationWindow.Closed += (object o, EventArgs args1) =>
                {
                    SetContent();
                };

                examinationWindow.Show();
            };
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var examinationToDelete = unitOfWork.Examinations.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити "
                             + examinationToDelete.ExaminationType.TypeName + " "
                             + examinationToDelete.Diagnosis
                             + " " + examinationToDelete.ExaminationDate.ToString("dd.MM.yyyy") + " ?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            examinationToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Examinations.Update(examinationToDelete);
            unitOfWork.Save();

            foreach (var data in examinationToDelete.ExaminationDatas)
            {
                data.DeletedDate = DateTime.Now;
                unitOfWork.ExaminationDatas.Update(data);
            }
            unitOfWork.Save();

            SetContent();
        }

        private void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var examinationToUpdate = unitOfWork.Examinations.Get((int)button.Tag);
            ExaminationWindow examinationWindow = new ExaminationWindow(examinationToUpdate, ActionType.Edit);
            examinationWindow.Owner = this;
            examinationWindow.Closed += (object o, EventArgs args) =>
            {
                SetContent();
            };

            examinationWindow.Show();
        }
    }
}
