using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// --- MARKER INTERFACE ---
public interface IInventoryEntity
{
    int Id { get; }
}

// --- INVENTORY RECORD ---
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// --- GENERIC INVENTORY LOGGER ---
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item) => _log.Add(item);

    public List<T> GetAll() => new List<T>(_log);

    public void SaveToFile()
    {
        try
        {
            string json = JsonSerializer.Serialize(_log, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
            Console.WriteLine($"Data saved to {_filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving file: " + ex.Message);
        }
    }

    public void LoadFromFile()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("No saved file found.");
                return;
            }

            string json = File.ReadAllText(_filePath);
            _log = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            Console.WriteLine("Data loaded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading file: " + ex.Message);
        }
    }
}

// --- INVENTORY APP ---
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger;

    public InventoryApp(string filePath)
    {
        _logger = new InventoryLogger<InventoryItem>(filePath);
    }

    public void SeedSampleDataInteractive()
    {
        Console.Write("How many items do you want to add? ");
        int count = int.Parse(Console.ReadLine());

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\n--- Item {i + 1} ---");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Quantity: ");
            int qty = int.Parse(Console.ReadLine());

            var item = new InventoryItem(id, name, qty, DateTime.Now);
            _logger.Add(item);
        }
    }

    public void SaveData() => _logger.SaveToFile();

    public void LoadData() => _logger.LoadFromFile();

    public void PrintAllItems()
    {
        Console.WriteLine("\n--- Inventory Items ---");
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"{item.Id}: {item.Name} - Qty: {item.Quantity}, Added: {item.DateAdded}");
        }
    }
}

// --- MAIN PROGRAM ---
public class Program
{
    public static void Main()
    {
        Console.Write("Enter file path for inventory data (e.g., inventory.json): ");
        string filePath = Console.ReadLine();

        InventoryApp app = new InventoryApp(filePath);

        // Step 1: Add items interactively
        app.SeedSampleDataInteractive();

        // Step 2: Save to file
        app.SaveData();

        // Step 3: Clear memory & reload (simulating a new session)
        Console.WriteLine("\nReloading data from file...");
        app.LoadData();

        // Step 4: Print all items
        app.PrintAllItems();
    }
}
