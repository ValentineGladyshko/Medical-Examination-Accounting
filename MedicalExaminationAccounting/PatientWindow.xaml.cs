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
    /// Логика взаимодействия для PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        public Patient LocalPatient { get; set; }
        public ActionType Action { get; set; }
        EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public PatientWindow()
        {
            InitializeComponent();

            LocalPatient = new Patient();
            Action = ActionType.Edit;

            SetBindings();
        }

        public PatientWindow(Patient patient, ActionType action)
        {
            InitializeComponent();

            LocalPatient = patient;
            Action = action;

            SetBindings();
            SetContent();
            SetListSources();
        }

        public void SetBindings()
        {
            Binding lastNameBinding = new Binding
            {
                Source = LocalPatient,
                Path = new PropertyPath("LastName"),
                Mode = BindingMode.TwoWay
            };

            LastNameBox.SetBinding(TextBox.TextProperty, lastNameBinding);

            Binding firstNameBinding = new Binding
            {
                Source = LocalPatient,
                Path = new PropertyPath("FirstName"),
                Mode = BindingMode.TwoWay
            };

            FirstNameBox.SetBinding(TextBox.TextProperty, firstNameBinding);

            Binding middleNameBinding = new Binding
            {
                Source = LocalPatient,
                Path = new PropertyPath("MiddleName"),
                Mode = BindingMode.TwoWay
            };

            MiddleNameBox.SetBinding(TextBox.TextProperty, middleNameBinding);

            
        }

        public void SetContent()
        {
            if (Action == ActionType.Edit)
            {
                Title = "Редагування пацієнта";
                WorkButton.Content = "Редагувати";
            }
            else if (Action == ActionType.Create)
            {
                Title = "Створення пацієнта";
                WorkButton.Content = "Створити";
            }
        }

        public void SetListSources()
        {
            RegionBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Regions.GetAll()
                    .Where(region =>
                        region.DeletedDate == null
                        && region.RegionName.Contains(RegionBox.Text))
                    .Select(region => region.RegionName).Distinct().ToList();
                list.Sort();
                RegionBox.ItemsSource = list;
                if(LocalPatient.Street != null)
                    RegionBox.Text = LocalPatient.Street.Settlement.Region.RegionName;
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
                if (LocalPatient.Street != null)
                    RegionBox.Text = LocalPatient.Street.Settlement.SettlementName;
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
                if (LocalPatient.Street != null)
                    RegionBox.Text = LocalPatient.Street.StreetName;
            };
        }
    }
}
