using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Region
    {
        public int Id { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string RegionName { get; set; }

        public DateTime? DeletedDate { get; set; }
    }
}