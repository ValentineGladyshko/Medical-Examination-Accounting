using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MedicalExaminationAccounting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EFUnitOfWork unitOfWork = new EFUnitOfWork("DataContext");

        private int MRIId;
        private int ECGId;

        public MainWindow()
        {
            InitializeComponent();

            MRIId = unitOfWork.ExaminationTypes.Find(item => item.TypeName == "МРТ").First().Id;
            ECGId = unitOfWork.ExaminationTypes.Find(item => item.TypeName == "ЕКГ").First().Id;

            SetPatientHandlers();
            SetMRIHandlers();
            SetECGHandlers();
            SetDoctorHandlers();
            SetRowHandlers();
        }

        public void SetPatientHandlers()
        {
            unitOfWork.Regions.GetAll().ToList();

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
                StreetBox.ItemsSource = list;
            };

            SearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var query = unitOfWork.Patients.GetAll()
                    .Where(patient =>
                        patient.DeletedDate == null
                        && patient.FirstName.Contains(FirstNameBox.Text)
                        && patient.MiddleName.Contains(MiddleNameBox.Text)
                        && patient.LastName.Contains(LastNameBox.Text)
                        && patient.Street.DeletedDate == null
                        && patient.Street.Settlement.SettlementName.Contains(SettlementBox.Text)
                        && patient.Street.Settlement.Region.RegionName.Contains(RegionBox.Text)
                        && patient.Street.StreetName.Contains(StreetBox.Text));

                if (StartDatePicker.SelectedDate != null)
                {
                    query = query.Where(patient => patient.BirthDate >= StartDatePicker.SelectedDate);
                }

                if (EndDatePicker.SelectedDate != null)
                {
                    query = query.Where(patient => patient.BirthDate <= EndDatePicker.SelectedDate);
                }

                var list = query.ToList();

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

            CreateButton.Click += (object sender, RoutedEventArgs e) =>
            {
                Region regionToCreate = new Region
                {
                    RegionName = RegionBox.Text
                };
                Settlement settlementToCreate = new Settlement
                {
                    Region = regionToCreate,
                    SettlementName = SettlementBox.Text
                };
                Street streetToCreate = new Street
                {
                    Settlement = settlementToCreate,
                    StreetName = StreetBox.Text
                };
                Patient patientToCreate = new Patient
                {
                    FirstName = FirstNameBox.Text,
                    LastName = LastNameBox.Text,
                    MiddleName = MiddleNameBox.Text,
                    BirthDate = StartDatePicker.SelectedDate ?? DateTime.Now,
                    Street = streetToCreate
                };

                PatientWindow patientWindow = new PatientWindow(patientToCreate, ActionType.Create);
                patientWindow.Owner = this;
                patientWindow.Show();
            };

            ListBoxPatient.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var patient = ListBoxPatient.SelectedItem as Patient;
                if (patient != null)
                {
                    PatientInfoWindow patientInfoWindow = new PatientInfoWindow(patient.Id);
                    patientInfoWindow.Owner = this;
                    patientInfoWindow.Closed += (object o, EventArgs eventArgs) =>
                    {
                        SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    };
                    patientInfoWindow.Show();                   
                }
                ListBoxPatient.SelectedIndex = -1;

            };
        }

        private void DeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var patientToDelete = unitOfWork.Patients.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити пацієнта " 
                             + patientToDelete.LastName + " " 
                             + patientToDelete.FirstName 
                             + " " + patientToDelete.MiddleName + " ?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            patientToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Patients.Update(patientToDelete);
            unitOfWork.Save();

            foreach (var examination in patientToDelete.Examinations)
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

            SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void EditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var patientToUpdate = unitOfWork.Patients.Get((int)button.Tag);

            PatientWindow patientWindow = new PatientWindow(patientToUpdate, ActionType.Edit);
            patientWindow.Owner = this;
            patientWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                SearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            patientWindow.Show();
        }

        private void MRIDeleteButtonOnClick(object sender, RoutedEventArgs e)
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

            MRISearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void MRIEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var examinationToUpdate = unitOfWork.Examinations.Get((int)button.Tag);

            ExaminationWindow examinationWindow = new ExaminationWindow(examinationToUpdate, ActionType.Edit);
            examinationWindow.Owner = this;
            examinationWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                MRISearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            examinationWindow.Show();
        }

        private void ECGDeleteButtonOnClick(object sender, RoutedEventArgs e)
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

            ECGSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void ECGEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var examinationToUpdate = unitOfWork.Examinations.Get((int)button.Tag);

            ExaminationWindow examinationWindow = new ExaminationWindow(examinationToUpdate, ActionType.Edit);
            examinationWindow.Owner = this;
            examinationWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                ECGSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            examinationWindow.Show();
        }

        private void DoctorDeleteButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var doctorToDelete = unitOfWork.Doctors.Get((int)button.Tag);

            string message = "Ви впевнені що хочете видалити лікаря "
                             + doctorToDelete.Position + " "
                             + doctorToDelete.LastName + " "
                             + doctorToDelete.FirstName
                             + " " + doctorToDelete.MiddleName + " ?";

            DialogWindow dialogWindow = new DialogWindow(message);
            bool? dialogResult = dialogWindow.ShowDialog();

            if (dialogResult != true)
                return;

            doctorToDelete.DeletedDate = DateTime.Now;
            unitOfWork.Doctors.Update(doctorToDelete);
            unitOfWork.Save();

            foreach (var examination in doctorToDelete.Examinations)
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

            DoctorSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void DoctorEditButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var doctorToUpdate = unitOfWork.Doctors.Get((int)button.Tag);

            DoctorWindow doctorWindow = new DoctorWindow(doctorToUpdate, ActionType.Edit);
            doctorWindow.Owner = this;
            doctorWindow.Closed += (object o, EventArgs eventArgs) =>
            {
                DoctorSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            };
            doctorWindow.Show();
        }

        private void RowRestoreButtonOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            var typeInfo = (TypeInfo) button.Tag;

            if (typeInfo.Type == RowType.Patient)
            {
                var item = unitOfWork.Patients.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.Patients.Update(item);
                foreach (var examination in item.Examinations)
                {
                    examination.DeletedDate = null;
                    unitOfWork.Examinations.Update(examination);

                    foreach (var data in examination.ExaminationDatas)
                    {
                        data.DeletedDate = null;
                        unitOfWork.ExaminationDatas.Update(data);
                    }
                }

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.Doctor)
            {
                var item = unitOfWork.Doctors.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.Doctors.Update(item);
                foreach (var examination in item.Examinations)
                {
                    examination.DeletedDate = null;
                    unitOfWork.Examinations.Update(examination);

                    foreach (var data in examination.ExaminationDatas)
                    {
                        data.DeletedDate = null;
                        unitOfWork.ExaminationDatas.Update(data);
                    }
                }

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.ExaminationType)
            {
                var item = unitOfWork.ExaminationTypes.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.ExaminationTypes.Update(item);

                var list = unitOfWork.Examinations.GetAll()
                    .Where(item1 => item1.ExaminationTypeId == item.Id);
                foreach (var examination in list)
                {
                    examination.DeletedDate = null;
                    unitOfWork.Examinations.Update(examination);

                    foreach (var data in examination.ExaminationDatas)
                    {
                        data.DeletedDate = null;
                        unitOfWork.ExaminationDatas.Update(data);
                    }
                }

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.Examination)
            {
                var item = unitOfWork.Examinations.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.Examinations.Update(item);

                foreach (var data in item.ExaminationDatas)
                {
                    data.DeletedDate = null;
                    unitOfWork.ExaminationDatas.Update(data);
                }

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.ExaminationData)
            {
                var item = unitOfWork.ExaminationDatas.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.ExaminationDatas.Update(item);

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.Region)
            {
                var item = unitOfWork.Regions.Get(typeInfo.Id);

                item.DeletedDate = null;
                unitOfWork.Regions.Update(item);

                var list = unitOfWork.Settlements.GetAll()
                    .Where(item1 => item1.RegionId == item.Id);
                foreach (var settlement in list)
                {
                    settlement.DeletedDate = null;
                    unitOfWork.Settlements.Update(settlement);

                    var list1 = unitOfWork.Streets.GetAll()
                        .Where(item1 => item1.SettlementId == settlement.Id);

                    foreach (var street in list1)
                    {
                        street.DeletedDate = null;
                        unitOfWork.Streets.Update(street);

                        var list2 = unitOfWork.Patients.GetAll()
                            .Where(item1 => item1.StreetId == street.Id);

                        foreach (var patient in list2)
                        {
                            patient.DeletedDate = null;
                            unitOfWork.Patients.Update(patient);

                            foreach (var examination in patient.Examinations)
                            {
                                examination.DeletedDate = null;
                                unitOfWork.Examinations.Update(examination);

                                foreach (var data in examination.ExaminationDatas)
                                {
                                    data.DeletedDate = null;
                                    unitOfWork.ExaminationDatas.Update(data);
                                }
                            }
                        }
                    }
                }


                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.Settlement)
            {
                var settlement = unitOfWork.Settlements.Get(typeInfo.Id);

                settlement.DeletedDate = null;
                unitOfWork.Settlements.Update(settlement);

                var list1 = unitOfWork.Streets.GetAll()
                    .Where(item1 => item1.SettlementId == settlement.Id);

                foreach (var street in list1)
                {
                    street.DeletedDate = null;
                    unitOfWork.Streets.Update(street);

                    var list2 = unitOfWork.Patients.GetAll()
                        .Where(item1 => item1.StreetId == street.Id);

                    foreach (var patient in list2)
                    {
                        patient.DeletedDate = null;
                        unitOfWork.Patients.Update(patient);

                        foreach (var examination in patient.Examinations)
                        {
                            examination.DeletedDate = null;
                            unitOfWork.Examinations.Update(examination);

                            foreach (var data in examination.ExaminationDatas)
                            {
                                data.DeletedDate = null;
                                unitOfWork.ExaminationDatas.Update(data);
                            }
                        }
                    }
                }

                unitOfWork.Save();
            }

            if (typeInfo.Type == RowType.Street)
            {
                var street = unitOfWork.Streets.Get(typeInfo.Id);


                street.DeletedDate = null;
                unitOfWork.Streets.Update(street);

                var list2 = unitOfWork.Patients.GetAll()
                    .Where(item1 => item1.StreetId == street.Id);

                foreach (var patient in list2)
                {
                    patient.DeletedDate = null;
                    unitOfWork.Patients.Update(patient);

                    foreach (var examination in patient.Examinations)
                    {
                        examination.DeletedDate = null;
                        unitOfWork.Examinations.Update(examination);

                        foreach (var data in examination.ExaminationDatas)
                        {
                            data.DeletedDate = null;
                            unitOfWork.ExaminationDatas.Update(data);
                        }
                    }

                }
                unitOfWork.Save();
            }
        }

        public void SetMRIHandlers()
        {
            var patientList = unitOfWork.Patients.GetAll()
                .Where(item => item.DeletedDate == null 
                               && item.Examinations
                                   .Any(exam => exam.ExaminationTypeId == MRIId))
                .ToList();

            if (patientList == null)
            {
                patientList = new List<Patient>();
            }
            else
            {
                
            
            patientList.Sort((a, b) =>
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
            }
            MRIPatientBox.ItemsSource = patientList;

            var doctorList = unitOfWork.Doctors.GetAll()
                .Where(item => item.DeletedDate == null 
                               && item.Examinations
                                   .Any(exam => exam.ExaminationTypeId == MRIId))
                .ToList();
            if (doctorList == null)
            {
                doctorList = new List<Doctor>();
            }
            else
            {


                doctorList.Sort((a, b) =>
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
            }

            MRIDoctorBox.ItemsSource = doctorList;

            MRIDoctorBoxDelete.Click += (object sender, RoutedEventArgs args) => { MRIDoctorBox.SelectedIndex = -1; };
            MRIPatientBoxDelete.Click += (object sender, RoutedEventArgs args) => { MRIPatientBox.SelectedIndex = -1; };
            MRISearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var query = unitOfWork.Examinations.GetAll().Where(item => item.ExaminationTypeId == MRIId && item.DeletedDate == null);

                if (MRIDoctorBox.SelectedIndex != -1)
                {
                    query = query.Where(item => item.DoctorId == ((Doctor) MRIDoctorBox.SelectedItem).Id);
                }

                if (MRIPatientBox.SelectedIndex != -1)
                {
                    query = query.Where(item => item.PatientId == ((Patient)MRIPatientBox.SelectedItem).Id);
                }

                if (MRIStartDatePicker.SelectedDate != null)
                {
                    query = query.Where(item => item.ExaminationDate >= MRIStartDatePicker.SelectedDate);
                }

                if (MRIEndDatePicker.SelectedDate != null)
                {
                    query = query.Where(item => item.ExaminationDate <= MRIEndDatePicker.SelectedDate);
                }

                var list = query.ToList();

                MRIListBox.ItemsSource = list;
            };

            MRIListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var examination = MRIListBox.SelectedItem as Examination;
                if (examination != null)
                {
                    ExaminationInfoWindow examinationInfoWindow = new ExaminationInfoWindow(examination.Id);
                    examinationInfoWindow.Owner = this;
                    examinationInfoWindow.Closed += (object o, EventArgs eventArgs) =>
                    {
                        MRISearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    };
                    examinationInfoWindow.Show();
                }
                MRIListBox.SelectedIndex = -1;
            };

            MRICreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Examination examination = new Examination();
                examination.ExaminationTypeId = MRIId;
                examination.ExaminationType = unitOfWork.ExaminationTypes.Get(MRIId);

                if (MRIDoctorBox.SelectedIndex != -1)
                {
                    examination.DoctorId = ((Doctor) MRIDoctorBox.SelectedItem).Id;
                    examination.Doctor = (Doctor) MRIDoctorBox.SelectedItem;
                }

                if (MRIPatientBox.SelectedIndex != -1)
                {
                    examination.PatientId = ((Patient) MRIPatientBox.SelectedItem).Id;
                    examination.Patient = (Patient) MRIPatientBox.SelectedItem;
                }

                ExaminationWindow examinationWindow = new ExaminationWindow(examination, ActionType.Create);
                examinationWindow.Owner = this;
                examinationWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    MRISearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                examinationWindow.Show();
            };
        }

        public void SetECGHandlers()
        {
            var patientList = unitOfWork.Patients.GetAll()
                .Where(item => item.DeletedDate == null
                               && item.Examinations
                                   .Any(exam => exam.ExaminationTypeId == ECGId))
                .ToList();
            if (patientList == null)
            {
                patientList = new List<Patient>();
            }
            else
            {

                patientList.Sort((a, b) =>
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
            }

            ECGPatientBox.ItemsSource = patientList;

            var doctorList = unitOfWork.Doctors.GetAll()
                .Where(item => item.DeletedDate == null
                               && item.Examinations
                                   .Any(exam => exam.ExaminationTypeId == ECGId))
                .ToList();
            if (doctorList == null)
            {
                doctorList = new List<Doctor>();
            }
            else
            {

                doctorList.Sort((a, b) =>
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
            }

            ECGDoctorBox.ItemsSource = doctorList;

            ECGDoctorBoxDelete.Click += (object sender, RoutedEventArgs args) => { ECGDoctorBox.SelectedIndex = -1; };
            ECGPatientBoxDelete.Click += (object sender, RoutedEventArgs args) => { ECGPatientBox.SelectedIndex = -1; };
            ECGSearchButton.Click += (object sender, RoutedEventArgs e) =>
            {
                var query = unitOfWork.Examinations.GetAll().Where(item => item.ExaminationTypeId == ECGId && item.DeletedDate == null);

                if (ECGDoctorBox.SelectedIndex != -1)
                {
                    query = query.Where(item => item.DoctorId == ((Doctor)ECGDoctorBox.SelectedItem).Id);
                }

                if (ECGPatientBox.SelectedIndex != -1)
                {
                    query = query.Where(item => item.PatientId == ((Patient)ECGPatientBox.SelectedItem).Id);
                }

                if (ECGStartDatePicker.SelectedDate != null)
                {
                    query = query.Where(item => item.ExaminationDate >= ECGStartDatePicker.SelectedDate);
                }

                if (ECGEndDatePicker.SelectedDate != null)
                {
                    query = query.Where(item => item.ExaminationDate <= ECGEndDatePicker.SelectedDate);
                }

                var list = query.ToList();

                ECGListBox.ItemsSource = list;
            };

            ECGListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {
                var examination = ECGListBox.SelectedItem as Examination;
                if (examination != null)
                {
                    ExaminationInfoWindow examinationInfoWindow = new ExaminationInfoWindow(examination.Id);
                    examinationInfoWindow.Owner = this;
                    examinationInfoWindow.Closed += (object o, EventArgs eventArgs) =>
                    {
                        ECGSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    };
                    examinationInfoWindow.Show();
                }
                ECGListBox.SelectedIndex = -1;
            };

            ECGCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Examination examination = new Examination();
                examination.ExaminationTypeId = ECGId;
                examination.ExaminationType = unitOfWork.ExaminationTypes.Get(ECGId);

                if (ECGDoctorBox.SelectedIndex != -1)
                {
                    examination.DoctorId = ((Doctor)ECGDoctorBox.SelectedItem).Id;
                    examination.Doctor = (Doctor)ECGDoctorBox.SelectedItem;
                }

                if (ECGPatientBox.SelectedIndex != -1)
                {
                    examination.PatientId = ((Patient)ECGPatientBox.SelectedItem).Id;
                    examination.Patient = (Patient)ECGPatientBox.SelectedItem;
                }

                ExaminationWindow examinationWindow = new ExaminationWindow(examination, ActionType.Create);
                examinationWindow.Owner = this;
                examinationWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    ECGSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                examinationWindow.Show();
            };
        }

        public void SetDoctorHandlers()
        {
            DoctorFirstNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Doctors.GetAll()
                    .Where(item =>
                        item.DeletedDate == null
                        && item.FirstName.Contains(DoctorFirstNameBox.Text))
                    .Select(item => item.FirstName).ToList().Distinct().ToList();
                list.Sort();
                DoctorFirstNameBox.ItemsSource = list;
            };

            DoctorMiddleNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Doctors.GetAll()
                    .Where(item =>
                        item.DeletedDate == null
                        && item.MiddleName.Contains(DoctorMiddleNameBox.Text))
                    .Select(item => item.MiddleName).ToList().Distinct().ToList();
                list.Sort();
                DoctorMiddleNameBox.ItemsSource = list;
            };

            DoctorLastNameBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Doctors.GetAll()
                    .Where(item =>
                        item.DeletedDate == null
                        && item.LastName.Contains(DoctorLastNameBox.Text))
                    .Select(item => item.LastName).ToList().Distinct().ToList();
                list.Sort();
                DoctorLastNameBox.ItemsSource = list;
            };

            DoctorPositionBox.DropDownOpened += (object sender, EventArgs e) =>
            {
                var list = unitOfWork.Doctors.GetAll()
                    .Where(item =>
                        item.DeletedDate == null
                        && item.Position.Contains(DoctorPositionBox.Text))
                    .Select(item => item.Position).Distinct().ToList();
                list.Sort();
                DoctorPositionBox.ItemsSource = list;
            };

            DoctorSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                var list = unitOfWork.Doctors.GetAll()
                    .Where(item =>
                        item.DeletedDate == null
                        && item.Position.Contains(DoctorPositionBox.Text)
                        && item.LastName.Contains(DoctorLastNameBox.Text)
                        && item.FirstName.Contains(DoctorFirstNameBox.Text)
                        && item.FirstName.Contains(DoctorFirstNameBox.Text)).ToList();
                DoctorListBox.ItemsSource = list;
            };

            DoctorCreateButton.Click += (object sender, RoutedEventArgs args) =>
            {
                Doctor doctor = new Doctor();
                doctor.FirstName = DoctorFirstNameBox.Text;
                doctor.MiddleName = DoctorMiddleNameBox.Text;
                doctor.LastName = DoctorLastNameBox.Text;
                doctor.Position = DoctorPositionBox.Text;

                DoctorWindow doctorWindow = new DoctorWindow(doctor, ActionType.Create);
                doctorWindow.Owner = this;
                doctorWindow.Closed += (object o, EventArgs eventArgs) =>
                {
                    DoctorSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                };

                doctorWindow.Show();
            };

            DoctorListBox.SelectionChanged += (object sender, SelectionChangedEventArgs args) =>
            {             
                var doctor = DoctorListBox.SelectedItem as Doctor;
                if (doctor != null)
                {
                    DoctorInfoWindow doctorInfoWindow = new DoctorInfoWindow(doctor.Id);
                    doctorInfoWindow.Owner = this;
                    doctorInfoWindow.Closed += (object o, EventArgs eventArgs) =>
                    {
                        DoctorSearchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    };
                    doctorInfoWindow.Show();
                }
                DoctorListBox.SelectedIndex = -1;
            };
        }

        public void SetRowHandlers()
        {
            string[] types =
            {
                "Пацієнти",
                "Обстеження",
                "Типи обстежень",
                "Зображення",
                "Лікарі",
                "Області",
                "Населені пункти",
                "Вулиці"
            };
            RowTypeBox.ItemsSource = types;

            RowTypeBoxDelete.Click += (object sender, RoutedEventArgs args) => { RowTypeBox.SelectedIndex = -1; };
            RowSearchButton.Click += (object sender, RoutedEventArgs args) =>
            {
                if (RowTypeBox.SelectedIndex == -1)
                {
                    var query1 = unitOfWork.Patients.GetAll().Where(item => item.DeletedDate != null);
                    var query2 = unitOfWork.Examinations.GetAll().Where(item => item.DeletedDate != null);
                    var query3 = unitOfWork.ExaminationTypes.GetAll().Where(item => item.DeletedDate != null);
                    var query4 = unitOfWork.ExaminationDatas.GetAll().Where(item => item.DeletedDate != null);
                    var query5 = unitOfWork.Doctors.GetAll().Where(item => item.DeletedDate != null);
                    var query6 = unitOfWork.Regions.GetAll().Where(item => item.DeletedDate != null);
                    var query7 = unitOfWork.Settlements.GetAll().Where(item => item.DeletedDate != null);
                    var query8 = unitOfWork.Streets.GetAll().Where(item => item.DeletedDate != null);


                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query1 = query1.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query2 = query2.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query3 = query3.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query4 = query4.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query5 = query5.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query6 = query6.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query7 = query7.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                        query8 = query8.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query1 = query1.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query2 = query2.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query3 = query3.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query4 = query4.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query5 = query5.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query6 = query6.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query7 = query7.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                        query8 = query8.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list1 = query1.ToList();
                    list1.Sort((a, b) =>
                        {
                            return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                        });
                    var source1 = list1.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Patient;
                        entity.Text = item.LastName + " " +
                            item.FirstName + " " +
                            item.MiddleName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list2 = query2.ToList();
                    list2.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source2 = list2.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Examination;
                        entity.Text = item.ExaminationType.TypeName + " " + 
                                      item.Diagnosis + 
                                      " " + item.DeletedDate;
                        return entity;
                    });

                    var list3 = query3.ToList();
                    list3.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source3 = list3.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.ExaminationType;
                        entity.Text = item.TypeName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list4 = query4.ToList();
                    list4.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source4 = list4.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.ExaminationData;
                        entity.Text = item.Examination.ExaminationType.TypeName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list5 = query5.ToList();
                    list5.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source5 = list5.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Doctor;
                        entity.Text = item.Position + " " + 
                                      item.LastName + " " +
                                      item.FirstName + " " +
                                      item.MiddleName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list6 = query6.ToList();
                    list6.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source6 = list6.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Region;
                        entity.Text = item.RegionName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list7 = query7.ToList();
                    list7.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source7 = list7.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Settlement;
                        entity.Text = item.SettlementName + " " + item.DeletedDate;
                        return entity;
                    });

                    var list8 = query8.ToList();
                    list8.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source8 = list8.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Street;
                        entity.Text = item.StreetName + " " + item.DeletedDate;
                        return entity;
                    });

                    var superSource = source1.Concat(source2).Concat(source3)
                        .Concat(source4).Concat(source5).Concat(source6)
                        .Concat(source7).Concat(source8);

                    RowListBox.ItemsSource = superSource;
                    return;
                }

                if (((string) RowTypeBox.SelectedValue) == "Пацієнти")
                {
                    var query = unitOfWork.Patients.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                       }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Patient;
                        entity.Text = item.LastName + " " +
                                      item.FirstName + " " +
                                      item.MiddleName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Обстеження")
                {
                    var query = unitOfWork.Examinations.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Examination;
                        entity.Text = item.ExaminationType.TypeName + " " +
                                      item.Diagnosis +
                                      " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Типи обстежень")
                {
                    var query = unitOfWork.ExaminationTypes.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.ExaminationType;
                        entity.Text = item.TypeName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Зображення")
                {
                    var query = unitOfWork.ExaminationDatas.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.ExaminationData;
                        entity.Text = item.Examination.ExaminationType.TypeName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Лікарі")
                {
                    var query = unitOfWork.Doctors.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Doctor;
                        entity.Text = item.Position + " " +
                                      item.LastName + " " +
                                      item.FirstName + " " +
                                      item.MiddleName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Області")
                {
                    var query = unitOfWork.Regions.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Region;
                        entity.Text = item.RegionName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Населені пункти")
                {
                    var query = unitOfWork.Settlements.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Settlement;
                        entity.Text = item.SettlementName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }

                if (((string)RowTypeBox.SelectedValue) == "Вулиці")
                {
                    var query = unitOfWork.Streets.GetAll().Where(item => item.DeletedDate != null);

                    if (RowStartDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate >= RowStartDatePicker.SelectedDate);
                    }

                    if (RowEndDatePicker.SelectedDate != null)
                    {
                        query = query.Where(item => item.DeletedDate <= RowEndDatePicker.SelectedDate);
                    }

                    var list = query.ToList();
                    list.Sort((a, b) =>
                    {
                        return ((DateTime)a.DeletedDate).CompareTo((DateTime)b.DeletedDate);
                    });
                    var source = list.ConvertAll(item =>
                    {
                        Entity entity = new Entity();
                        entity.Id = new TypeInfo();
                        entity.Id.Id = item.Id;
                        entity.Id.Type = RowType.Street;
                        entity.Text = item.StreetName + " " + item.DeletedDate;
                        return entity;
                    });

                    RowListBox.ItemsSource = source;
                    return;
                }
            };
        }
    }
}
