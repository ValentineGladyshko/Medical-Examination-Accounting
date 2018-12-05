using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoMapper.QueryableExtensions;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataContext db = new DataContext("DataContext");

        public MainWindow()
        {
            InitializeComponent();

            db.Patients.Load();
            //ListBoxPatient.ItemsSource = db.Patients.Local;

            var asd = db.Patients.ToList();
            var firstNamesList = asd.Select(patient => patient.FirstName).Distinct().ToList();
            firstNamesList.Sort();

            var middleNamesList = asd.Select(patient => patient.MiddleName).Distinct().ToList();
            middleNamesList.Sort();

            var lastNamesList = asd.Select(patient => patient.LastName).Distinct().ToList();
            lastNamesList.Sort();

            FirstNameBox.ItemsSource = firstNamesList;
            MiddleNameBox.ItemsSource = middleNamesList;
            LastNameBox.ItemsSource = lastNamesList;

            FirstNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = firstNamesList.Where(item => item.Contains(FirstNameBox.Text));
                FirstNameBox.ItemsSource = list;
            };

            MiddleNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                middleNamesList = db.Patients.Local.Where(patient =>
                        patient.MiddleName.ToLower().Contains(MiddleNameBox.Text.ToLower()))
                    .Select(patient => patient.MiddleName).Distinct().ToList();
                middleNamesList.Sort();
                MiddleNameBox.ItemsSource = middleNamesList;
            };

            LastNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                lastNamesList = db.Patients.Local
                    .Where(patient => patient.LastName.ToLower().Contains(LastNameBox.Text.ToLower()))
                    .Select(patient => patient.LastName).Distinct().ToList();
                lastNamesList.Sort();
                LastNameBox.ItemsSource = lastNamesList;
            };

            SearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                ListBoxPatient.ItemsSource = db.Patients.Local.Where(patient =>
                    patient.FirstName.ToLower().Contains(FirstNameBox.Text.ToLower())
                    && patient.MiddleName.ToLower().Contains(MiddleNameBox.Text.ToLower())
                    && patient.LastName.ToLower().Contains(LastNameBox.Text.ToLower()));
            };
        }
    }
}
