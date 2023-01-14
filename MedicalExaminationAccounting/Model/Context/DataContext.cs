using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MedicalExaminationAccounting.Model.Entities;
using MedicalExaminationAccounting.Tools;
using Region = MedicalExaminationAccounting.Model.Entities.Region;

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

        private static DataContext dataContext;
        private DataContext(string connectionString) : base(connectionString)
        {
        }

        public static DataContext GetDataContext(string connectionString)
        {
            if (dataContext == null)
            {
                dataContext = new DataContext(connectionString);
            }

            return dataContext;
        }
    }

    public class StoreDbInitializer : DropCreateDatabaseIfModelChanges<DataContext>
    {
        protected override void Seed(DataContext db)
        {
            RegionInit(db);
            ExaminationTypeInit(db);
            PatientInit(db);
            DoctorInit(db);
            ExaminationInit(db);
        }

        private void RegionInit(DataContext db)
        {
            string path = "../../Strings";
            string[] regions = File.ReadAllLines(path + "/regions.txt");
            var regionsList = new List<Region>();
            foreach (string region in regions)
            {
                regionsList.Add(new Region
                {
                    RegionName = region,
                    DeletedDate = null
                });
            }
            db.Regions.AddRange(regionsList);
            db.SaveChanges();
        }

        private void ExaminationTypeInit(DataContext db)
        {
            db.ExaminationTypes.Add(new ExaminationType
            {
                TypeName = "МРТ",
                DeletedDate = null
            });

            db.ExaminationTypes.Add(new ExaminationType
            {
                TypeName = "ЕКГ",
                DeletedDate = null
            });

            db.SaveChanges();
        }

        private void PatientInit(DataContext db)
        {
            string path = "../../Strings";
            string[] womanFirstNames = File.ReadAllLines(path + "/womanfirstnames.txt");
            string[] womanMiddleNames = File.ReadAllLines(path + "/womanmiddlenames.txt");
            string[] womanLastNames = File.ReadAllLines(path + "/womanlastnames.txt");
            string[] manFirstNames = File.ReadAllLines(path + "/manfirstnames.txt");
            string[] manMiddleNames = File.ReadAllLines(path + "/manmiddlenames.txt");
            string[] manLastNames = File.ReadAllLines(path + "/manlastnames.txt");

            db.Settlements.Add(new Settlement
            {
                SettlementName = "Київ",
                RegionId = db.Regions.Where(region => region.RegionName == "м. Київ").First().Id,
                DeletedDate = null
            });

            db.SaveChanges();

            db.Streets.Add(new Street
            {
                StreetName = "Політехнічний провулок",
                SettlementId = db.Settlements.First().Id,
                DeletedDate = null
            });

            db.SaveChanges();

            Random rand = new Random();
            DateTime RandomDate()
            {
                DateTime start = new DateTime(1950, 1, 1);
                int range = (DateTime.Today - start).Days;
                return start.AddDays(rand.Next(range) - 7000);
            }

            var patientslist = new List<Patient>();
            int streertId = db.Streets.First().Id;
            for (int i = 0; i < 1000; i++)
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

                patient.BirthDate = RandomDate();
                patient.StreetId = streertId;
                patient.DeletedDate = null;
                patientslist.Add(patient);
            }

            db.Patients.AddRange(patientslist);
            db.SaveChanges();
        }

        private void DoctorInit(DataContext db)
        {
            string path = "../../Strings";
            string[] womanFirstNames = File.ReadAllLines(path + "/womanfirstnames.txt");
            string[] womanMiddleNames = File.ReadAllLines(path + "/womanmiddlenames.txt");
            string[] womanLastNames = File.ReadAllLines(path + "/womanlastnames.txt");
            string[] manFirstNames = File.ReadAllLines(path + "/manfirstnames.txt");
            string[] manMiddleNames = File.ReadAllLines(path + "/manmiddlenames.txt");
            string[] manLastNames = File.ReadAllLines(path + "/manlastnames.txt");

            Random rand = new Random();
            var doctorslist = new List<Doctor>();
            for (int i = 0; i < 50; i++)
            {
                var doctor = new Doctor();
                if (rand.Next(0, 2) == 0)
                {
                    doctor.FirstName = womanFirstNames[rand.Next(0, womanFirstNames.Length)];
                    doctor.MiddleName = womanMiddleNames[rand.Next(0, womanMiddleNames.Length)];
                    doctor.LastName = womanLastNames[rand.Next(0, womanLastNames.Length)];
                }
                else
                {
                    doctor.FirstName = manFirstNames[rand.Next(0, manFirstNames.Length)];
                    doctor.MiddleName = manMiddleNames[rand.Next(0, manMiddleNames.Length)];
                    doctor.LastName = manLastNames[rand.Next(0, manLastNames.Length)];
                }

                doctor.DeletedDate = null;
                doctor.Position = "Рентгенолог";
                doctorslist.Add(doctor);
            }

            db.Doctors.AddRange(doctorslist);
            db.SaveChanges();
        }

        private void ExaminationInit(DataContext db)
        {
            Random rand = new Random();
            DateTime RandomDate()
            {
                DateTime start = new DateTime(2015, 1, 1);
                int range = (DateTime.Today - start).Days;
                return start.AddDays(rand.Next(range));
            }

            IImageConverter imageConverter = new JpgQualityConverter();

            byte[] newBytes1 = imageConverter.ConvertImage(File.ReadAllBytes("../../Images/forest.jpg"));
            byte[] newBytes2 = imageConverter.ConvertImage(File.ReadAllBytes("../../Images/mountain.jpg"));
            byte[] newBytes3 = imageConverter.ConvertImage(File.ReadAllBytes("../../Images/blueberries.jpg"));

            //File.WriteAllBytes("../../Images/forest1.jpg", newBytes1);

            var patientsList = db.Patients.ToList();
            int patientsCount = patientsList.Count;
            var doctorList = db.Doctors.ToList();
            int doctorsCount = doctorList.Count;
            var examinationTypesList = db.ExaminationTypes.ToList();
            int examinationTypesCount = examinationTypesList.Count;

            for (int i = 0; i < 1000; i++)
            {                
                Examination examination = new Examination
                {
                    Diagnosis = "Some Diagnosis",
                    PatientId = patientsList[rand.Next(0, patientsCount)].Id,
                    DoctorId = doctorList[rand.Next(0, doctorsCount)].Id,
                    ExaminationTypeId = examinationTypesList[rand.Next(0, examinationTypesCount)].Id,
                    ExaminationDate = RandomDate(),
                    Descripton = "Some Descripton"
                };

                db.Examinations.Add(examination);
                db.SaveChanges();

                if (rand.Next(0, 2) == 0)
                {
                    ExaminationData data = new ExaminationData
                    {
                        ExaminationId = examination.Id,
                        Data = newBytes1
                    };
                    db.ExaminationDatas.Add(data);
                }

                if (rand.Next(0, 2) == 0)
                {
                    ExaminationData data = new ExaminationData
                    {
                        ExaminationId = examination.Id,
                        Data = newBytes2
                    };
                    db.ExaminationDatas.Add(data);
                }

                if (rand.Next(0, 2) == 0)
                {
                    ExaminationData data = new ExaminationData
                    {
                        ExaminationId = examination.Id,
                        Data = newBytes3
                    };
                    db.ExaminationDatas.Add(data);
                }

                db.SaveChanges();
            }
        }
    }
}