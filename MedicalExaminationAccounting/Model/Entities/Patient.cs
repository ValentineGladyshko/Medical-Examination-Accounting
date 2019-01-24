using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Patient
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public int? StreetId { get; set; }
        public virtual Street Street { get; set; }

        public virtual ICollection<Examination> Examinations { get; set; }
    }
}