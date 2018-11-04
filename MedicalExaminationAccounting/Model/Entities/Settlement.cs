using System.Collections.Generic;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Settlement
    {
        public int Id { get; set; }
        public string SettlementName { get; set; }

        public int RegionId { get; set; }
        public virtual Region Region { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }
}