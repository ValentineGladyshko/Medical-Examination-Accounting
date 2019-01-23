using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Street
    {
        public int Id { get; set; }
        [Required]
        public string StreetName { get; set; }

        [Required]
        public int SettlementId { get; set; }
        public virtual Settlement Settlement { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}