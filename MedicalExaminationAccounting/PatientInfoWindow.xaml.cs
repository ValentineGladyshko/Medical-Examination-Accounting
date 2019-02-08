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
    /// Логика взаимодействия для PatientInfoWindow.xaml
    /// </summary>
    public partial class PatientInfoWindow : Window
    {
        private Patient Patient { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public PatientInfoWindow()
        {
            InitializeComponent();

            Patient = new Patient
            {
                Street = new Street
                {
                    Settlement = new Settlement
                    {
                        Region = new Region()
                    }
                },
                Examinations = new List<Examination>()
            };          
            SetContent();
            SetHandlers();
        }

        public PatientInfoWindow(int id)
        {
            InitializeComponent();

            Patient = unitOfWork.Patients.Get(id);
            SetContent();
            SetHandlers();
        }

        private void SetContent()
        {
            LastNameBox.Text = "Прізвище: " + Patient.LastName;
            FirstNameBox.Text = "Ім'я: " + Patient.FirstName;
            MiddleNameBox.Text = "По-батькові: " + Patient.MiddleName;
            RegionBox.Text = "Область: " + Patient.Street.Settlement.Region.RegionName;
            SettlementBox.Text = "Населений пункт: " + Patient.Street.Settlement.SettlementName;
            StreetBox.Text = "Вулиця: " + Patient.Street.StreetName;
            BirthDateBox.Text = "Дата народження: " + Patient.BirthDate.ToString("dd.MM.yyyy");

            ExaminationListBox.ItemsSource = Patient.Examinations.Where(item => item.DeletedDate == null);
        }

        private void SetHandlers()
        {
            DeleteButton.Click += (object sender, RoutedEventArgs e) =>
            {
                string message = "Ви впевнені що хочете видалити пацієнта "
                                 + Patient.LastName + " "
                                 + Patient.FirstName
                                 + " " + Patient.MiddleName + " ?";

                DialogWindow dialogWindow = new DialogWindow(message);
                bool? dialogResult = dialogWindow.ShowDialog();

                if (dialogResult != true)
                    return;

                Patient.DeletedDate = DateTime.Now;
                unitOfWork.Patients.Update(Patient);
                unitOfWork.Save();

                foreach (var examination in Patient.Examinations)
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
                PatientWindow patientWindow = new PatientWindow(Patient, ActionType.Edit);
                patientWindow.ShowDialog();

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
                examinationToCreate.Patient = Patient;
                examinationToCreate.PatientId = Patient.Id;
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

            var examinationToUpdate = unitOfWork.Examinations.Get((int) button.Tag);
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
