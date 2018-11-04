using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting.Model.Interfaces
{
    public interface IUnitOfWork
    {
        IRepository<Patient> Patients { get; }
        IRepository<Region> Regions { get; }
        IRepository<Settlement> Settlements { get; }
        IRepository<Street> Streets { get; }
        void Save();
    }
}