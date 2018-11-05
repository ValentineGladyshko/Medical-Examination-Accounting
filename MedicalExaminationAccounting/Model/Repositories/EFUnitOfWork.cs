using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private DataContext db;
        private PatientRepository patientRepository;
        private RegionRepository regionRepository;
        private SettlementRepository settlementRepository;
        private StreetRepository streetRepository;

        public EFUnitOfWork(string connectionString)
        {
            db = new DataContext(connectionString);
        }

        public IRepository<Patient> Patients 
            => patientRepository ?? (patientRepository = new PatientRepository(db));
        public IRepository<Region> Regions 
            => regionRepository ?? (regionRepository = new RegionRepository(db));
        public IRepository<Settlement> Settlements 
            => settlementRepository ?? (settlementRepository = new SettlementRepository(db));
        public IRepository<Street> Streets 
            => streetRepository ?? (streetRepository = new StreetRepository(db));

        public void Save()
        {
            db.SaveChanges();
        }

        public void Dispose()
        {
            db?.Dispose();
        }
    }
}