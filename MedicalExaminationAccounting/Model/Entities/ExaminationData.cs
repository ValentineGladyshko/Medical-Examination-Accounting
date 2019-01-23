using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class ExaminationData
    {
        public int Id { get; set; }
        [Required]
        public byte[] Data { get; set; }

        [Required]
        public int ExaminationId { get; set; }
        public virtual Examination Examination { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}
