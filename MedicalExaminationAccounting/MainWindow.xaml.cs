using MedicalExaminationAccounting.Model.Repositories;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
                    .Where(patient =>
                        patient.DeletedDate == null
                        && patient.FirstName.Contains(FirstNameBox.Text))
                    .Select(patient => patient.FirstName).ToList().Distinct().ToList();
                list.Sort();
                FirstNameBox.ItemsSource = list;
            };

            MiddleNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient =>
                        patient.DeletedDate == null
                        && patient.MiddleName.Contains(MiddleNameBox.Text))
                    .Select(patient => patient.MiddleName).ToList().Distinct().ToList();
                list.Sort();
                MiddleNameBox.ItemsSource = list;
            };

            LastNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient =>
                        patient.DeletedDate == null
                        && patient.LastName.Contains(LastNameBox.Text))
                    .Select(patient => patient.LastName).ToList().Distinct().ToList();
                list.Sort();
                LastNameBox.ItemsSource = list;
            };

            RegionBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Regions.GetAll()
                    .Where(region =>
                        region.DeletedDate == null
                        && region.RegionName.Contains(RegionBox.Text))
                    .Select(region => region.RegionName).Distinct().ToList();
                list.Sort();
                RegionBox.ItemsSource = list;
            };

            SettlementBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Settlements.GetAll()
                    .Where(settlement =>
                        settlement.DeletedDate == null
                        && settlement.Region.RegionName.Contains(RegionBox.Text)
                        && settlement.SettlementName.Contains(SettlementBox.Text))
                    .Select(settlement => settlement.SettlementName).Distinct().ToList();
                list.Sort();
                SettlementBox.ItemsSource = list;
            };

            StreetBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.Settlement.SettlementName.Contains(SettlementBox.Text)
                        && street.Settlement.Region.RegionName.Contains(RegionBox.Text)
                        && street.StreetName.Contains(StreetBox.Text))
                    .Select(street => street.StreetName).Distinct().ToList();
                list.Sort();
                SettlementBox.ItemsSource = list;
            };

            SearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var list = unitOfWork.Patients.GetAll()
                    .Where(patient =>
                        patient.DeletedDate == null
                        && patient.FirstName.Contains(FirstNameBox.Text)
                        && patient.MiddleName.Contains(MiddleNameBox.Text)
                        && patient.LastName.Contains(LastNameBox.Text)).ToList();

                list.Sort(
                    (a, b) =>
                    {
                        int result = a.LastName.CompareTo(b.LastName);
                        if (result != 0)
                            return result;
                        else
                        {
                            result = a.FirstName.CompareTo(b.FirstName);
                            if (result != 0)
                                return result;
                            else
                            {
                                return a.MiddleName.CompareTo(b.MiddleName);
                            }
                        }
                    });

                ListBoxPatient.ItemsSource = list;
            };
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var patientToDelete = unitOfWork.Patients.Get((int)button.Tag);
            patientToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Patients.Update(patientToDelete);
            unitOfWork.Save();

            SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var patientToDelete = unitOfWork.Patients.Get((int)button.Tag);
            patientToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Patients.Update(patientToDelete);
            unitOfWork.Save();

            SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }
}
