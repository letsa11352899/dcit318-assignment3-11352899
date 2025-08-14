using System;
using System.Collections.Generic;
using System.Linq;

// --- GENERIC REPOSITORY ---
public class Repository<T>
{
    private List<T> items = new List<T>();

    public void Add(T item) => items.Add(item);

    public List<T> GetAll() => new List<T>(items);

    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);

    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

// --- PATIENT CLASS ---
public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }

    public override string ToString()
    {
        return $"{Id}: {Name}, Age {Age}, Gender {Gender}";
    }
}

// --- PRESCRIPTION CLASS ---
public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }

    public override string ToString()
    {
        return $"{Id}: {MedicationName} (Patient ID: {PatientId}, Date: {DateIssued:d})";
    }
}

// --- HEALTH SYSTEM APP ---
public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new Repository<Patient>();
    private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

    public void SeedDataInteractive()
    {
        Console.Write("How many patients to enter? ");
        int patientCount = int.Parse(Console.ReadLine());

        for (int i = 0; i < patientCount; i++)
        {
            Console.WriteLine($"\n--- Patient {i + 1} ---");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Age: ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("Gender: ");
            string gender = Console.ReadLine();

            _patientRepo.Add(new Patient(id, name, age, gender));
        }

        Console.Write("\nHow many prescriptions to enter? ");
        int prescriptionCount = int.Parse(Console.ReadLine());

        for (int i = 0; i < prescriptionCount; i++)
        {
            Console.WriteLine($"\n--- Prescription {i + 1} ---");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Patient ID: ");
            int patientId = int.Parse(Console.ReadLine());
            Console.Write("Medication Name: ");
            string medName = Console.ReadLine();
            Console.Write("Date Issued (yyyy-mm-dd): ");
            DateTime dateIssued = DateTime.Parse(Console.ReadLine());

            _prescriptionRepo.Add(new Prescription(id, patientId, medName, dateIssued));
        }
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap.Clear();
        foreach (var prescription in _prescriptionRepo.GetAll())
        {
            if (!_prescriptionMap.ContainsKey(prescription.PatientId))
            {
                _prescriptionMap[prescription.PatientId] = new List<Prescription>();
            }
            _prescriptionMap[prescription.PatientId].Add(prescription);
        }
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\n--- All Patients ---");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine(patient);
        }
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        if (_prescriptionMap.ContainsKey(id))
        {
            Console.WriteLine($"\nPrescriptions for Patient ID {id}:");
            foreach (var p in _prescriptionMap[id])
            {
                Console.WriteLine(p);
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

// --- MAIN PROGRAM ---
public class Program
{
    public static void Main()
    {
        HealthSystemApp app = new HealthSystemApp();

        // Step 1: Enter data from user
        app.SeedDataInteractive();

        // Step 2: Build prescription map
        app.BuildPrescriptionMap();

        // Step 3: Print all patients
        app.PrintAllPatients();

        // Step 4: Ask user for a patient ID and print prescriptions
        Console.Write("\nEnter Patient ID to view prescriptions: ");
        int searchId = int.Parse(Console.ReadLine());
        app.PrintPrescriptionsForPatient(searchId);
    }
}
