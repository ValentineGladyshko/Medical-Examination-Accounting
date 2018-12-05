using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting.ViewModel.ViewModels
{
    public class RegionViewModel : INotifyPropertyChanged
    {
        private string regionName;

        public int Id { get; set; }

        public string RegionName
        {
            get => regionName;
            set
            {
                regionName = value;
                OnPropertyChanged("RegionName");
            }
        }

        public virtual ICollection<Settlement> Settlements { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}