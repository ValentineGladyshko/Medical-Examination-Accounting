using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для ExaminationInfoWindow.xaml
    /// </summary>
    public partial class ExaminationInfoWindow : Window
    {
        private Examination Examination { get; set; }
        private EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public ExaminationInfoWindow()
        {
            InitializeComponent();

            Examination = new Examination
            {
                Patient = new Patient(),
                Doctor = new Doctor(),
                ExaminationDatas = new List<ExaminationData>()
            };

            SetContent();
            SetHandlers();
        }

        public ExaminationInfoWindow(int id)
        {
            InitializeComponent();

            Examination = unitOfWork.Examinations.Get(id);

            SetContent();
            SetHandlers();
        }

        public void SetContent()
        {
            TypeBox.Text = "Обстеження " + Examination.ExaminationType.TypeName;
            ExaminationDateBox.Text = "Дата обстеження: "
                                      + Examination.ExaminationDate.ToString("dd.MM.yyyy");
            DiagnosisBox.Text = "Діагноз: " + Examination.Diagnosis;
            DescriptionBox.Text = "Додаткова інформація: " + Examination.Descripton;
            PatientBox.Text = "Пацієнт: " + Examination.Patient.LastName + " "
                                          + Examination.Patient.FirstName + " "
                                          + Examination.Patient.MiddleName;
            if (Examination.Doctor != null)
            {
                DoctorBox.Text = "Лікар: " + Examination.Doctor.LastName + " "
                                           + Examination.Doctor.FirstName + " "
                                           + Examination.Doctor.MiddleName;
            }

            DataBox.ItemsSource = Examination.ExaminationDatas.Where(item => item.DeletedDate == null);
        }

        public void SetHandlers()
        {
            DataBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var data = DataBox.SelectedItem as ExaminationData;
                if (data != null)
                {
                    ImageWindow imageWindow = new ImageWindow(data.Id);
                    imageWindow.Owner = this;
                    imageWindow.Show();
                }
                DataBox.SelectedIndex = -1;
            };

            DeleteButton.Click += (object sender, RoutedEventArgs e) =>
            {
                string message = "Ви впевнені що хочете видалити "
                                 + Examination.ExaminationType.TypeName + " "
                                 + Examination.Diagnosis
                                 + " " + Examination.ExaminationDate.ToString("dd.MM.yyyy") + " ?";

                DialogWindow dialogWindow = new DialogWindow(message);
                bool? dialogResult = dialogWindow.ShowDialog();

                if (dialogResult != true)
                    return;

                Examination.DeletedDate = DateTime.Now;
                unitOfWork.Examinations.Update(Examination);
                unitOfWork.Save();

                foreach (var data in Examination.ExaminationDatas)
                {
                    data.DeletedDate = DateTime.Now;
                    unitOfWork.ExaminationDatas.Update(data);
                }

                unitOfWork.Save();

                Close();
            };

            EditButton.Click += (object sender, RoutedEventArgs e) =>
            {
                ExaminationWindow examinationWindow = new ExaminationWindow(Examination, ActionType.Edit);
                examinationWindow.ShowDialog();

                SetContent();
            };

            CreateButton.Click += (object sender, RoutedEventArgs e) =>
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == true)
                {
                    string fileExt =
                        System.IO.Path.GetExtension(openFileDialog.FileName);

                    if (fileExt == ".jpeg" || fileExt == ".jpg" || fileExt == ".png")
                    {
                        ExaminationData data = new ExaminationData();
                        data.ExaminationId = Examination.Id;
                        data.Examination = Examination;
                        data.Data = File.ReadAllBytes(openFileDialog.FileName);
                        unitOfWork.ExaminationDatas.Create(data);
                    }
                    else
                    {         
                        MessageBox.Show("Ви вибрали файл з непідртимуваним розширенням" +
                                        ", будь-ласка виберіть зображення"
                            ,"Неправильний файл", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }

                    SetContent();
                }
            };
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var data = unitOfWork.ExaminationDatas.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити зображення?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            data.DeletedDate = DateTime.Now;
            unitOfWork.ExaminationDatas.Update(data);
            unitOfWork.Save();

            SetContent();
        }
    }
}
