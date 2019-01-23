using System;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting.Model.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Patient> Patients { get; }
        IRepository<Region> Regions { get; }
        IRepository<Settlement> Settlements { get; }
        IRepository<Street> Streets { get; }
        IRepository<Examination> Examinations { get; }
        IRepository<ExaminationType> ExaminationTypes { get; }
        IRepository<ExaminationData> ExaminationDatas { get; }
        IRepository<Doctor> Doctors { get; }
        void Save();
    }
}