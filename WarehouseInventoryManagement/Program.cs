using System;
using System.Collections.Generic;

// --- MARKER INTERFACE ---
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// --- ELECTRONIC ITEM ---
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }

    public override string ToString()
    {
        return $"{Id}: {Name} (Brand: {Brand}, Warranty: {WarrantyMonths} months, Qty: {Quantity})";
    }
}

// --- GROCERY ITEM ---
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public override string ToString()
    {
        return $"{Id}: {Name} (Expiry: {ExpiryDate:d}, Qty: {Quantity})";
    }
}

// --- CUSTOM EXCEPTIONS ---
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// --- GENERIC INVENTORY REPOSITORY ---
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.Remove(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
    }

    public List<T> GetAllItems() => new List<T>(_items.Values);

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items[id].Quantity = newQuantity;
    }
}

// --- WAREHOUSE MANAGER ---
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedDataInteractive()
    {
        Console.Write("How many electronic items to add? ");
        int eCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < eCount; i++)
        {
            Console.WriteLine($"\n--- Electronic Item {i + 1} ---");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Quantity: ");
            int qty = int.Parse(Console.ReadLine());
            Console.Write("Brand: ");
            string brand = Console.ReadLine();
            Console.Write("Warranty (months): ");
            int warranty = int.Parse(Console.ReadLine());

            try
            {
                _electronics.AddItem(new ElectronicItem(id, name, qty, brand, warranty));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        Console.Write("How many grocery items to add? ");
        int gCount = int.Parse(Console.ReadLine());
        for (int i = 0; i < gCount; i++)
        {
            Console.WriteLine($"\n--- Grocery Item {i + 1} ---");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Name: ");
            string name = Console.ReadLine();
            Console.Write("Quantity: ");
            int qty = int.Parse(Console.ReadLine());
            Console.Write("Expiry Date (yyyy-mm-dd): ");
            DateTime expiry = DateTime.Parse(Console.ReadLine());

            try
            {
                _groceries.AddItem(new GroceryItem(id, name, qty, expiry));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine(item);
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine("Stock updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine("Item removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }

    public void ShowAll()
    {
        Console.WriteLine("\n--- Electronics ---");
        PrintAllItems(_electronics);
        Console.WriteLine("\n--- Groceries ---");
        PrintAllItems(_groceries);
    }

    public InventoryRepository<ElectronicItem> GetElectronicsRepo() => _electronics;
    public InventoryRepository<GroceryItem> GetGroceriesRepo() => _groceries;
}

// --- MAIN PROGRAM ---
public class Program
{
    public static void Main()
    {
        WareHouseManager manager = new WareHouseManager();

        // Step 1: Add items interactively
        manager.SeedDataInteractive();

        // Step 2: Show all items
        manager.ShowAll();

        // Step 3: Test exception handling
        Console.Write("\nEnter Electronic Item ID to increase stock: ");
        int eId = int.Parse(Console.ReadLine());
        Console.Write("Enter quantity to add: ");
        int qty = int.Parse(Console.ReadLine());
        manager.IncreaseStock(manager.GetElectronicsRepo(), eId, qty);

        Console.Write("\nEnter Grocery Item ID to remove: ");
        int gId = int.Parse(Console.ReadLine());
        manager.RemoveItemById(manager.GetGroceriesRepo(), gId);

        // Step 4: Final list
        manager.ShowAll();
    }
}
