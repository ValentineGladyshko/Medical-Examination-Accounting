using System.Collections.Generic;

namespace MedicalExaminationAccounting.Model.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string RegionName { get; set; }

        public virtual ICollection<Settlement> Settlements { get; set; }
    }
}