using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class PatientRepository : IRepository<Patient>
    {
        private DataContext db;

        public PatientRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Patient> GetAll()
        {
            return db.Patients.Include(item => item.Street.Settlement.Region);
        }

        public Patient Get(int id)
        {
            return db.Patients.Find(id);
        }

        public IEnumerable<Patient> Find(Func<Patient, bool> predicate)
        {
            return db.Patients.Include(item => item.Street.Settlement.Region).Where(predicate);
        }

        public void Create(Patient item)
        {
            db.Patients.Add(item);
        }

        public void Update(Patient item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Patients.Find(id);
            if (item != null)
                db.Patients.Remove(item);
        }
    }
}