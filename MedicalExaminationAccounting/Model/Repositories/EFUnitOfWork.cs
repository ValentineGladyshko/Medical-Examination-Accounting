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
        private ExaminationRepository examinationRepository;
        private ExaminationDataRepository examinationDataRepository;
        private ExaminationTypeRepository examinationTypeRepository;
        private DoctorRepository doctorRepository;

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
        public IRepository<Examination> Examinations
            => examinationRepository ?? (examinationRepository = new ExaminationRepository(db));
        public IRepository<ExaminationData> ExaminationDatas
            => examinationDataRepository ?? (examinationDataRepository = new ExaminationDataRepository(db));
        public IRepository<ExaminationType> ExaminationTypes
            => examinationTypeRepository ?? (examinationTypeRepository = new ExaminationTypeRepository(db));
        public IRepository<Doctor> Doctors
            => doctorRepository ?? (doctorRepository = new DoctorRepository(db));

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