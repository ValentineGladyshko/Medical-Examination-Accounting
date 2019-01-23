using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using MedicalExaminationAccounting.Model.Entities;

namespace MedicalExaminationAccounting.Model.Context
{
    public class DataContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Examination> Examinations { get; set; }
        public DbSet<ExaminationType> ExaminationTypes { get; set; }
        public DbSet<ExaminationData> ExaminationDatas { get; set; }

        static DataContext()
        {
            Database.SetInitializer<DataContext>(new StoreDbInitializer());
        }
        public DataContext(string connectionString)
            : base(connectionString)
        {
        }
    }

    public class StoreDbInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext db)
        {
            PaietntInit(db);
        }

        private void RegionInit(DataContext db)
        {
            string path = @"C:\Users\Kappi\Source\Repos\Medical-Examination-Accounting\MedicalExaminationAccounting\Strings";
            string[] regionNames = File.ReadAllLines(path + @"\regions.txt");
            foreach (var name in regionNames)
            {
                db.Regions.Add(new Region
                {
                    RegionName = name
                });
            }
            
            db.SaveChanges();
        }

        private void PaietntInit(DataContext db)
        {
            string path = @"C:\Users\Kappi\Source\Repos\Medical-Examination-Accounting\MedicalExaminationAccounting\Strings";
            string[] womanFirstNames = File.ReadAllLines(path + @"\womanfirstnames.txt");
            string[] womanMiddleNames = File.ReadAllLines(path + @"\womanmiddlenames.txt");
            string[] womanLastNames = File.ReadAllLines(path + @"\womanlastnames.txt");
            string[] manFirstNames = File.ReadAllLines(path + @"\manfirstnames.txt");
            string[] manMiddleNames = File.ReadAllLines(path + @"\manmiddlenames.txt");
            string[] manLastNames = File.ReadAllLines(path + @"\manlastnames.txt");

            db.Regions.Add(new Region
            {
                RegionName = "Київська"
            });

            db.SaveChanges();

            db.Settlements.Add(new Settlement
            {
                SettlementName = "Київ",
                RegionId = db.Regions.First().Id
            });

            db.SaveChanges();

            db.Streets.Add(new Street
            {
                StreetName = "Політехнічний провулок",
                SettlementId = db.Settlements.First().Id
            });

            db.SaveChanges();

            Random rand = new Random();
            var list = new List<Patient>();
            int id = db.Streets.First().Id;
            for (int i = 0; i < 100; i++)
            {
                var patient = new Patient();
                if (rand.Next(0, 2) == 0)
                {
                    patient.FirstName = womanFirstNames[rand.Next(0, womanFirstNames.Length)];
                    patient.MiddleName = womanMiddleNames[rand.Next(0, womanMiddleNames.Length)];
                    patient.LastName = womanLastNames[rand.Next(0, womanLastNames.Length)];
                }
                else
                {
                    patient.FirstName = manFirstNames[rand.Next(0, manFirstNames.Length)];
                    patient.MiddleName = manMiddleNames[rand.Next(0, manMiddleNames.Length)];
                    patient.LastName = manLastNames[rand.Next(0, manLastNames.Length)];
                }

                patient.BirthDate = DateTime.Today;
                patient.StreetId = id;
                list.Add(patient);
            }

            db.Patients.AddRange(list);
            db.SaveChanges();
        }
    }
}