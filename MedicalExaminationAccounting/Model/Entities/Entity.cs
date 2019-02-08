namespace MedicalExaminationAccounting.Model.Entities
{
    public class TypeInfo
    {
        public int Id { get; set; }
        public RowType Type { get; set; } 
    }
    public class Entity
    {
        public string Text { get; set; }
        public TypeInfo Id { get; set; }
    }
}