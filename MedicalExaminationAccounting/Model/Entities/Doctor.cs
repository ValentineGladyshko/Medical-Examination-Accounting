using System;
using System.Collections.Generic;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Doctor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }

        public DateTime? DeletedDate { get; set; }

        public virtual ICollection<Examination> Examinations { get; set; }
    }
}