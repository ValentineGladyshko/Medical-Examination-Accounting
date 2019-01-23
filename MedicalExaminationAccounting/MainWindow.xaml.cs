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
using MedicalExaminationAccounting.Model.Repositories;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");
        public MainWindow()
        {
            unitOfWork.Regions.GetAll().ToList();
            InitializeComponent();
            
            FirstNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient => patient.FirstName.Contains(FirstNameBox.Text))
                    .Select(patient => patient.FirstName).ToList().Distinct().ToList();
                list.Sort();
                FirstNameBox.ItemsSource = list;
            };

            MiddleNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient => patient.MiddleName.Contains(MiddleNameBox.Text))
                    .Select(patient => patient.MiddleName).ToList().Distinct().ToList();
                list.Sort();
                MiddleNameBox.ItemsSource = list;
            };

            LastNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient => patient.LastName.Contains(LastNameBox.Text))
                    .Select(patient => patient.LastName).ToList().Distinct().ToList();
                list.Sort();
                LastNameBox.ItemsSource = list;
            };

            RegionBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Regions.GetAll()
                    .Where(region => region.RegionName.Contains(RegionBox.Text))
                    .Select(region => region.RegionName).Distinct().ToList();
                list.Sort();
                RegionBox.ItemsSource = list;
            };

            SettlementBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Settlements.GetAll()
                    .Where(settlement => settlement.Region.RegionName.Contains(RegionBox.Text) 
                                         && settlement.SettlementName.Contains(SettlementBox.Text))
                    .Select(settlement => settlement.SettlementName).Distinct().ToList();
                list.Sort();
                SettlementBox.ItemsSource = list;
            };

            StreetBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Streets.GetAll()
                    .Where(street => street.Settlement.SettlementName.Contains(SettlementBox.Text)
                                         && street.Settlement.Region.RegionName.Contains(RegionBox.Text)
                                         && street.StreetName.Contains(StreetBox.Text))
                    .Select(street => street.StreetName).Distinct().ToList();
                list.Sort();
                SettlementBox.ItemsSource = list;
            };

            SearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll().Where(patient =>
                    patient.FirstName.Contains(FirstNameBox.Text)
                    && patient.MiddleName.Contains(MiddleNameBox.Text)
                    && patient.LastName.Contains(LastNameBox.Text)).ToList();
                ListBoxPatient.ItemsSource = list;
            };
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            int s = (int) button.Tag;
            var patientToDelete = unitOfWork.Patients.Get(s);
            patientToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Patients.Update(patientToDelete);
        }
    }
}
