using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class ExaminationType
    {
        public int Id { get; set; }
        [Required]
        public string TypeName { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
