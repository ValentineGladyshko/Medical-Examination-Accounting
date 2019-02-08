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
    /// Логика взаимодействия для ExaminationWindow.xaml
    /// </summary>
    public partial class ExaminationWindow : Window
    {
        private Examination LocalExamination { get; set; }
        private ActionType Action { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public ExaminationWindow()
        {
            InitializeComponent();

            LocalExamination = new Examination
            {
                Patient = new Patient(),
                Doctor = new Doctor(),
                ExaminationDatas = new List<ExaminationData>(),
                ExaminationType = new ExaminationType(),
                ExaminationDate = DateTime.Now
            };

            Action = ActionType.Create;
            SetContent();
            SetHandlers(); 
        }

        public ExaminationWindow(Examination examination, ActionType action)
        {
            InitializeComponent();

            LocalExamination = examination;

            LocalExamination.Patient = LocalExamination.Patient ?? new Patient();
            LocalExamination.Doctor = LocalExamination.Doctor ?? new Doctor();
            LocalExamination.ExaminationDatas = LocalExamination.ExaminationDatas ?? new List<ExaminationData>();
            LocalExamination.ExaminationType = LocalExamination.ExaminationType ?? new ExaminationType();

            Action = action;

            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування обстеження";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення обстеження";
                WorkButton.Content = "Створити";
            }

            var typeList = unitOfWork.ExaminationTypes.GetAll()
                .Where(item => item.DeletedDate == null).ToList();
            TypeBox.ItemsSource = typeList;

            if(TypeBox.Items.Contains(LocalExamination.ExaminationType))
            {
                TypeBox.SelectedIndex = TypeBox.Items.IndexOf(LocalExamination.ExaminationType);
            }

            else
            {
                if (typeList.Count != 0)
                    TypeBox.SelectedIndex = TypeBox.Items.IndexOf(typeList.First());
            }

            var patientList = unitOfWork.Patients.GetAll()
                .Where(item => item.DeletedDate == null).ToList();
            PatientBox.ItemsSource = patientList;

            if (PatientBox.Items.Contains(LocalExamination.Patient))
            {
                PatientBox.SelectedIndex = PatientBox.Items.IndexOf(LocalExamination.Patient);
            }

            else
            {
                if (patientList.Count != 0)
                    PatientBox.SelectedIndex = PatientBox.Items.IndexOf(patientList.First());
            }

            var doctorList = unitOfWork.Doctors.GetAll()
                .Where(item => item.DeletedDate == null).ToList();
            DoctorBox.ItemsSource = doctorList;

            if (DoctorBox.Items.Contains(LocalExamination.Doctor))
            {
                DoctorBox.SelectedIndex = DoctorBox.Items.IndexOf(LocalExamination.Doctor);
            }

            DiagnosisBox.Text = LocalExamination.Diagnosis;
            DescriptionBox.Text = LocalExamination.Descripton;
            DatePicker.SelectedDate = LocalExamination.ExaminationDate;
        }

        public void SetHandlers()
        {
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (TypeBox.SelectedIndex == -1)
                {
                    TypeBorder.BorderBrush = new SolidColorBrush(Colors.Red); //#FFACACAC
                }

                if (PatientBox.SelectedIndex == -1)
                {
                    PatientBorder.BorderBrush = new SolidColorBrush(Colors.Red); //#FFACACAC
                }

                if (TypeBox.SelectedIndex == -1 || PatientBox.SelectedIndex == -1)
                {
                    return;
                }

                LocalExamination.ExaminationType = TypeBox.SelectedItem as ExaminationType;
                LocalExamination.ExaminationTypeId = LocalExamination.ExaminationType.Id;
                LocalExamination.Patient = PatientBox.SelectedItem as Patient;
                LocalExamination.PatientId = LocalExamination.Patient.Id;
                if (DoctorBox.SelectedItem != null)
                {
                    LocalExamination.Doctor = DoctorBox.SelectedItem as Doctor;
                    LocalExamination.DoctorId = LocalExamination.Doctor.Id;
                }
                LocalExamination.Descripton = DescriptionBox.Text;
                LocalExamination.Diagnosis = DiagnosisBox.Text;
                LocalExamination.ExaminationDate = DatePicker.SelectedDate ?? DateTime.Now;

                if (Action == ActionType.Create)
                {
                    unitOfWork.Examinations.Create(LocalExamination);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Edit)
                {
                    unitOfWork.Examinations.Update(LocalExamination);
                    unitOfWork.Save();
                    Close();
                }
            };

            TypeBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                TypeBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            };

            PatientBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                PatientBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            };
        }
    }
}
