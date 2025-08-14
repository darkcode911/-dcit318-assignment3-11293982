using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthcareManagementSystem
{
    // a. Define a Generic Repository for Entity Management
    public class Repository<T>
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
        }

        public List<T> GetAll()
        {
            return _items;
        }

        public T? GetById(Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public bool Remove(Func<T, bool> predicate)
        {
            var itemToRemove = _items.FirstOrDefault(predicate);
            if (itemToRemove != null)
            {
                _items.Remove(itemToRemove);
                return true;
            }
            return false;
        }
    }

    // b. Define the Patient Class
    public class Patient
    {
        public int Id { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Age: {Age}, Gender: {Gender}";
        }
    }

    // c. Define the Prescription Class
    public class Prescription
    {
        public int Id { get; }
        public int PatientId { get; }
        public string MedicationName { get; }
        public DateTime DateIssued { get; }

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
         public override string ToString()
        {
            return $"  - Med: {MedicationName}, Issued: {DateIssued:yyyy-MM-dd}";
        }
    }

    // g. Create a HealthSystemApp class
    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new Repository<Patient>();
        private readonly Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(101, "John Smith", 45, "Male"));
            _patientRepo.Add(new Patient(102, "Maria Garcia", 32, "Female"));
            _patientRepo.Add(new Patient(103, "Chen Wei", 51, "Male"));

            _prescriptionRepo.Add(new Prescription(1, 101, "Lisinopril", DateTime.Now.AddDays(-30)));
            _prescriptionRepo.Add(new Prescription(2, 102, "Metformin", DateTime.Now.AddDays(-15)));
            _prescriptionRepo.Add(new Prescription(3, 101, "Atorvastatin", DateTime.Now.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(4, 103, "Amlodipine", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(5, 102, "Amoxicillin", DateTime.Now.AddDays(-2)));
        }

        public void BuildPrescriptionMap()
        {
             _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("--- All Patients ---");
            List<Patient> patients = _patientRepo.GetAll();
            foreach (var patient in patients)
            {
                Console.WriteLine(patient);
            }
            Console.WriteLine();
        }
        
        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.GetValueOrDefault(patientId, new List<Prescription>());
        }

        public void PrintPrescriptionsForPatient(int patientId)
        {
            var patient = _patientRepo.GetById(p => p.Id == patientId);
            if (patient == null)
            {
                Console.WriteLine($"Patient with ID {patientId} not found.");
                return;
            }

            Console.WriteLine($"--- Prescriptions for {patient.Name} (ID: {patient.Id}) ---");
            List<Prescription> prescriptions = GetPrescriptionsByPatientId(patientId);

            if (prescriptions.Any())
            {
                foreach (var prescription in prescriptions)
                {
                    Console.WriteLine(prescription);
                }
            }
            else
            {
                Console.WriteLine("  No prescriptions found for this patient.");
            }
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            int patientIdToQuery = 102;
            app.PrintPrescriptionsForPatient(patientIdToQuery);
            
            int anotherPatientId = 101;
            app.PrintPrescriptionsForPatient(anotherPatientId);
        }
    }
}