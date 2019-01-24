using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Repositories;
using MedicalExaminationAccounting.Rules;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для PatientWindow.xaml
    /// </summary>
    public partial class PatientWindow : Window
    {
        public Patient LocalPatient { get; set; }
        public Patient NewLocalPatient { get; set; }
        public NameRule NameRule = new NameRule();
        public ActionType Action { get; set; }
        EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        public PatientWindow()
        {
            InitializeComponent();

            LocalPatient = new Patient();
            NewLocalPatient = new Patient();
            DataContext = NewLocalPatient;
            Action = ActionType.Create;

            SetContent();
            SetListSources();
            SetButtonHandlers();
            SetHandlers();
        }

        public PatientWindow(Patient patient, ActionType action)
        {
            InitializeComponent();

            LocalPatient = patient;
            NewLocalPatient = new Patient();         
            DataContext = NewLocalPatient;
            Action = action;

            SetContent();
            SetListSources();
            SetButtonHandlers();
            SetHandlers();
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

            NewLocalPatient.LastName = LocalPatient.LastName;
            NewLocalPatient.FirstName = LocalPatient.FirstName;
            NewLocalPatient.MiddleName = LocalPatient.MiddleName;
            BirthDatePicker.SelectedDate = LocalPatient.BirthDate;

            if (LocalPatient.Street != null)
            {
                RegionBox.Text = LocalPatient.Street.Settlement.Region.RegionName;
                SettlementBox.Text = LocalPatient.Street.Settlement.SettlementName;
                StreetBox.Text = LocalPatient.Street.StreetName;
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
                StreetBox.ItemsSource = list;
            };
        }

        public void SetButtonHandlers()
        {
            CancelButton.Click += (object sender, RoutedEventArgs e) => { Close(); };

            WorkButton.Click += (object sender, RoutedEventArgs e) =>
            {
                if (NameRule.Validate(LastNameBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult
                    || NameRule.Validate(FirstNameBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult
                    || NameRule.Validate(MiddleNameBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult)
                {
                    return;
                }
                LocalPatient.FirstName = FirstNameBox.Text;
                LocalPatient.MiddleName = MiddleNameBox.Text;
                LocalPatient.LastName = LastNameBox.Text;
                LocalPatient.BirthDate = BirthDatePicker.DisplayDate;

                if (NameRule.Validate(RegionBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult
                    || NameRule.Validate(SettlementBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult
                    || NameRule.Validate(StreetBox.Text, CultureInfo.CurrentCulture) != ValidationResult.ValidResult)
                {
                    LocalPatient.Street = null;
                    LocalPatient.StreetId = null;

                    if (Action == ActionType.Edit)
                    {
                        unitOfWork.Patients.Update(LocalPatient);
                        unitOfWork.Save();
                        Close();
                    }
                    else if (Action == ActionType.Create)
                    {
                        unitOfWork.Patients.Create(LocalPatient);
                        unitOfWork.Save();
                        Close();
                    }
                }

                var streets = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.Settlement.SettlementName.CompareTo(SettlementBox.Text) == 0
                        && street.Settlement.Region.RegionName.CompareTo(RegionBox.Text) == 0
                        && street.StreetName.CompareTo(StreetBox.Text) == 0).ToList();

                Region newRegion = new Region();
                Settlement newSettlement = new Settlement();
                Street newStreet = new Street();

                if (streets.Count == 0)
                {
                    string message = "Для того щоб " + ActionText(Action) +
                                     " пацієнта також потрібно створити вулицю: \"" + StreetBox.Text +
                                     "\". Ви впевнені що хочете " +
                                     ActionText(Action) +
                                     " пацієнта?";

                    var settlements = unitOfWork.Settlements.GetAll()
                        .Where(settlement =>
                            settlement.DeletedDate == null
                            && settlement.SettlementName.CompareTo(SettlementBox.Text) == 0
                            && settlement.Region.RegionName.CompareTo(RegionBox.Text) == 0).ToList();

                    if (settlements.Count == 0)
                    {
                        message = "Для того щоб " + ActionText(Action) +
                                  " пацієнта також потрібно створити населений пункт: \"" + SettlementBox.Text +
                                  "\" та вулицю: \"" + StreetBox.Text +
                                  "\" в цьому населеному пункті. Ви впевнені що хочете " +
                                  ActionText(Action) +
                                  " пацієнта?";

                        var regions = unitOfWork.Regions.GetAll()
                            .Where(region =>
                                region.DeletedDate == null
                                && region.RegionName.CompareTo(RegionBox.Text) == 0).ToList();

                        if (regions.Count == 0)
                        {
                            message = "Для того щоб " + ActionText(Action) +
                                             " пацієнта також потрібно створити область: \"" +
                                             RegionBox.Text + "\", населений пункт: \"" + SettlementBox.Text +
                                             "\" в цій області та вулицю: \"" + StreetBox.Text +
                                             "\" в цьому населеному пункті. Ви впевнені що хочете " +
                                             ActionText(Action) +
                                             " пацієнта?";

                            DialogWindow dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();

                            if (dialogResult != true)
                                return;
                            newRegion = new Region
                            {
                                RegionName = RegionBox.Text,
                                DeletedDate = null
                            };

                            unitOfWork.Regions.Create(newRegion);
                            unitOfWork.Save();


                        }
                        else
                        {
                            DialogWindow dialogWindow = new DialogWindow(message);
                            bool? dialogResult = dialogWindow.ShowDialog();

                            if (dialogResult != true)
                                return;
                            newRegion = regions.First();
                        }

                        newSettlement = new Settlement
                        {
                            RegionId = newRegion.Id,
                            SettlementName = SettlementBox.Text,
                            DeletedDate = null
                        };

                        unitOfWork.Settlements.Create(newSettlement);
                        unitOfWork.Save();
                    }
                    else
                    {
                        DialogWindow dialogWindow = new DialogWindow(message);
                        bool? dialogResult = dialogWindow.ShowDialog();

                        if (dialogResult != true)
                            return;

                        newSettlement = settlements.First();
                    }

                    newStreet = new Street
                    {
                        StreetName = StreetBox.Text,
                        SettlementId = newSettlement.Id,
                        DeletedDate = null
                    };

                    unitOfWork.Streets.Create(newStreet);
                    unitOfWork.Save();

                    LocalPatient.Street = newStreet;
                    LocalPatient.StreetId = newStreet.Id;
                }
                else
                {
                    LocalPatient.Street = streets.First();
                    LocalPatient.StreetId = streets.First().Id;
                }

                if (Action == ActionType.Edit)
                {
                    unitOfWork.Patients.Update(LocalPatient);
                    unitOfWork.Save();
                    Close();
                }
                else if (Action == ActionType.Create)
                {
                    unitOfWork.Patients.Create(LocalPatient);
                    unitOfWork.Save();
                    Close();
                }
            };
        }

        public void SetHandlers()
        {
            StreetBox.LostFocus += (object sender, RoutedEventArgs args) =>
            {
                var streets = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.Settlement.SettlementName.CompareTo(SettlementBox.Text) == 0
                        && street.Settlement.Region.RegionName.CompareTo(RegionBox.Text) == 0
                        && street.StreetName.CompareTo(StreetBox.Text) == 0).ToList();

                if (streets.Count > 0)
                    return;

                streets = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.Settlement.SettlementName.CompareTo(SettlementBox.Text) == 0
                        && street.StreetName.CompareTo(StreetBox.Text) == 0).ToList();

                if (streets.Count > 0)
                {
                    RegionBox.Text = streets.First().Settlement.Region.RegionName;
                    return;
                }

                streets = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.Settlement.Region.RegionName.CompareTo(RegionBox.Text) == 0
                        && street.StreetName.CompareTo(StreetBox.Text) == 0).ToList();

                if (streets.Count > 0)
                {
                    SettlementBox.Text = streets.First().Settlement.SettlementName;
                    return;
                }

                streets = unitOfWork.Streets.GetAll()
                    .Where(street =>
                        street.DeletedDate == null
                        && street.StreetName.CompareTo(StreetBox.Text) == 0).ToList();

                if (streets.Count > 0)
                {
                    RegionBox.Text = streets.First().Settlement.Region.RegionName;
                    SettlementBox.Text = streets.First().Settlement.SettlementName;
                    return;
                }
            };

            SettlementBox.LostFocus += (object sender, RoutedEventArgs args) =>
            {
                var settlements = unitOfWork.Settlements.GetAll()
                    .Where(settlement =>
                        settlement.DeletedDate == null
                        && settlement.SettlementName.CompareTo(SettlementBox.Text) == 0
                        && settlement.Region.RegionName.CompareTo(RegionBox.Text) == 0).ToList();

                if (settlements.Count > 0)
                    return;

                settlements = unitOfWork.Settlements.GetAll()
                    .Where(settlement =>
                        settlement.DeletedDate == null
                        && settlement.SettlementName.CompareTo(SettlementBox.Text) == 0).ToList();

                if (settlements.Count > 0)
                {
                    RegionBox.Text = settlements.First().Region.RegionName;
                    return;
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
