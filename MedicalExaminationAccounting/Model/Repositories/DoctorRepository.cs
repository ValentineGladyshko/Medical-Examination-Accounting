using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MedicalExaminationAccounting.Model.Context;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Model.Interfaces;

namespace MedicalExaminationAccounting.Model.Repositories
{
    public class DoctorRepository : IRepository<Doctor>
    {
        private DataContext db;

        public DoctorRepository(DataContext context)
        {
            db = context;
        }

        public IQueryable<Doctor> GetAll()
        {
            return db.Doctors.Include(item => item.Examinations);
        }

        public Doctor Get(int id)
        {
            return db.Doctors.Find(id);
        }

        public IEnumerable<Doctor> Find(Func<Doctor, bool> predicate)
        {
            return db.Doctors.Include(item => item.Examinations).Where(predicate);
        }

        public void Create(Doctor item)
        {
            db.Doctors.Add(item);
        }

        public void Update(Doctor item)
        {
            db.Entry(item).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var item = db.Doctors.Find(id);
            if (item != null)
                db.Doctors.Remove(item);
        }
    }
}