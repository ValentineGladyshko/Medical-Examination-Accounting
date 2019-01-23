using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Examination
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; }
        public string Descripton { get; set; }

        [Required]
        public int ExaminationTypeId { get; set; }
        public virtual ExaminationType ExaminationType { get; set; }

        [Required]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }

        public DateTime DeletedDate { get; set; }

        public virtual ICollection<ExaminationData> ExaminationDatas { get; set; }
    }
}
