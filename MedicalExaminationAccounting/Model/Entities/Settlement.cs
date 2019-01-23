using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Settlement
    {
        public int Id { get; set; }
        [Required]
        public string SettlementName { get; set; }

        [Required]
        public int RegionId { get; set; }
        public virtual Region Region { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}