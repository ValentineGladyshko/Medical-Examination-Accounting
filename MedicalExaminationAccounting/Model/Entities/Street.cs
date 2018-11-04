using System.Collections.Generic;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Street
    {
        public int Id { get; set; }
        public string StreetName { get; set; }

        public int SettlementId { get; set; }
        public virtual Settlement Settlement { get; set; }
        public virtual ICollection<Patient> Patients { get; set; }
    }
}