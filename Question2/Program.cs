using System;
using System.Collections.Generic;
using System.Linq;

namespace Question2
{
    // a) Generic repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var toRemove = items.FirstOrDefault(predicate);
            if (toRemove is null) return false;
            items.Remove(toRemove);
            return true;
        }
    }

    // b) Patient
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    // c) Prescription
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() =>
            $"Prescription {{ Id={Id}, PatientId={PatientId}, Medication={MedicationName}, DateIssued={DateIssued:d} }}";
    }

    public class HealthSystemApp
    {
        // g) Fields
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new();

        // d/e) Build map by PatientId
        public void BuildPrescriptionMap()
        {
            _prescriptionMap = _prescriptionRepo.GetAll()
                .GroupBy(p => p.PatientId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        // f) Helper
        public List<Prescription> GetPrescriptionsByPatientId(int patientId) =>
            _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();

        // g) Methods
        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 28, "F"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 40, "M"));
            _patientRepo.Add(new Patient(3, "Cynthia Mensah", 33, "F"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Paracetamol", DateTime.Today.AddDays(-10)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Cetirizine", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(105, 2, "Omeprazole", DateTime.Today.AddDays(-3)));
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("=== Patients ===");
            foreach (var p in _patientRepo.GetAll())
                Console.WriteLine(p);
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            Console.WriteLine($"\n=== Prescriptions for PatientId={id} ===");
            var list = GetPrescriptionsByPatientId(id);
            if (list.Count == 0) Console.WriteLine("No prescriptions found.");
            foreach (var pr in list) Console.WriteLine(pr);
        }
    }

    public class Program
    {
        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(2); // choose any valid PatientId here
        }
    }
}
