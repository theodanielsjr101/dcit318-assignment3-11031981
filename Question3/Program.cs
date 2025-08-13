using System;
using System.Collections.Generic;
using System.Linq;

namespace Question3
{
    // a) Marker interface
    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

    // b) ElectronicItem
    public class ElectronicItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public string Brand { get; }
        public int WarrantyMonths { get; }

        public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
        {
            Id = id; Name = name; Quantity = quantity; Brand = brand; WarrantyMonths = warrantyMonths;
        }

        public override string ToString() =>
            $"Electronic {{ Id={Id}, Name={Name}, Qty={Quantity}, Brand={Brand}, Warranty={WarrantyMonths}m }}";
    }

    // c) GroceryItem
    public class GroceryItem : IInventoryItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; }

        public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
        {
            Id = id; Name = name; Quantity = quantity; ExpiryDate = expiryDate;
        }

        public override string ToString() =>
            $"Grocery {{ Id={Id}, Name={Name}, Qty={Quantity}, Expiry={ExpiryDate:d} }}";
    }

    // e) Custom exceptions
    public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
    public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
    public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

    // d) Generic inventory repo
    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
                throw new DuplicateItemException($"Item with Id {item.Id} already exists.");
            _items[item.Id] = item;
        }

        public T GetItemById(int id)
        {
            if (!_items.TryGetValue(id, out var item))
                throw new ItemNotFoundException($"Item with Id {id} was not found.");
            return item;
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
                throw new ItemNotFoundException($"Cannot remove. Item with Id {id} was not found.");
        }

        public List<T> GetAllItems() => _items.Values.ToList();

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
                throw new InvalidQuantityException("Quantity cannot be negative.");
            var item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    // f) Manager
    public class WareHouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics = new();
        private readonly InventoryRepository<GroceryItem> _groceries = new();

        public void SeedData()
        {
            try
            {
                _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
                _electronics.AddItem(new ElectronicItem(2, "Smartphone", 25, "Samsung", 12));
                _electronics.AddItem(new ElectronicItem(3, "Router", 15, "TP-Link", 18));

                _groceries.AddItem(new GroceryItem(101, "Rice 5kg", 50, DateTime.Today.AddMonths(12)));
                _groceries.AddItem(new GroceryItem(102, "Milk 1L", 80, DateTime.Today.AddDays(30)));
                _groceries.AddItem(new GroceryItem(103, "Eggs (tray)", 40, DateTime.Today.AddDays(14)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SeedData error: {ex.Message}");
            }
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            foreach (var item in repo.GetAllItems()) Console.WriteLine(item);
        }

        public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
        {
            try
            {
                var item = repo.GetItemById(id);
                repo.UpdateQuantity(id, item.Quantity + quantity);
                Console.WriteLine($"Stock increased: Id={id}, NewQty={item.Quantity}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IncreaseStock error: {ex.Message}");
            }
        }

        public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
        {
            try
            {
                repo.RemoveItem(id);
                Console.WriteLine($"Removed item Id={id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveItem error: {ex.Message}");
            }
        }

        // Expose repos for printing
        public InventoryRepository<ElectronicItem> Electronics => _electronics;
        public InventoryRepository<GroceryItem> Groceries => _groceries;
    }

    public class Program
    {
        public static void Main()
        {
            var mgr = new WareHouseManager();
            mgr.SeedData();

            Console.WriteLine("=== Groceries ===");
            mgr.PrintAllItems(mgr.Groceries);

            Console.WriteLine("\n=== Electronics ===");
            mgr.PrintAllItems(mgr.Electronics);

            Console.WriteLine("\n=== Exception demos ===");
            // Duplicate
            try { mgr.Electronics.AddItem(new ElectronicItem(1, "Duplicate Laptop", 5, "Dell", 24)); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            // Remove non-existent
            mgr.RemoveItemById(mgr.Groceries, 999);

            // Invalid quantity
            try { mgr.Electronics.UpdateQuantity(2, -5); }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
