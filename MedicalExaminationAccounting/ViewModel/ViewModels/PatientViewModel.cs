using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting.ViewModel.ViewModels
{
    public class PatientViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string firstName;
        private string middleName;
        private string lastName;
        private DateTime birthDate;

        private int streetId;
        public virtual Street Street { get; set; }

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string MiddleName
        {
            get => middleName;
            set
            {
                middleName = value;
                OnPropertyChanged("MiddleName");
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public DateTime BirthDate
        {
            get => birthDate;
            set
            {
                birthDate = value;
                OnPropertyChanged("BirthDate");
            }
        }

        public int StreetId
        {
            get => streetId;
            set
            {
                streetId = value;
                OnPropertyChanged("StreetId");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}