using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Question5
{
    // b) Marker interface
    public interface IInventoryEntity { int Id { get; } }

    // a) Immutable record implementing interface
    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    // c) Generic logger with file persistence
    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private readonly List<T> _log = new();
        private readonly string _filePath;

        public InventoryLogger(string filePath) => _filePath = filePath;

        public void Add(T item) => _log.Add(item);
        public List<T> GetAll() => new(_log);

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(_log, options);
                using var sw = new StreamWriter(_filePath);
                sw.Write(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveToFile error: {ex.Message}");
            }
        }

        public void LoadFromFile()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    Console.WriteLine("No file to load; starting fresh.");
                    return;
                }

                using var sr = new StreamReader(_filePath);
                var json = sr.ReadToEnd();
                var data = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                _log.Clear();
                _log.AddRange(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LoadFromFile error: {ex.Message}");
            }
        }
    }

    // f) Integration layer
    public class InventoryApp
    {
        private readonly InventoryLogger<InventoryItem> _logger;

        public InventoryApp(string path)
        {
            _logger = new InventoryLogger<InventoryItem>(path);
        }

        public void SeedSampleData()
        {
            _logger.Add(new InventoryItem(1, "Stapler", 12, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Printer Paper (A4)", 500, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Ink Cartridge", 36, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Whiteboard Markers", 20, DateTime.Now));
            _logger.Add(new InventoryItem(5, "Folders", 60, DateTime.Now));
        }

        public void SaveData() => _logger.SaveToFile();
        public void LoadData() => _logger.LoadFromFile();

        public void PrintAllItems()
        {
            Console.WriteLine("=== Inventory ===");
            foreach (var item in _logger.GetAll())
                Console.WriteLine(item);
        }
    }

    // g) Main flow
    public class Program
    {
        public static void Main()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "inventory.json");

            // Session 1: seed & save
            var app1 = new InventoryApp(filePath);
            app1.SeedSampleData();
            app1.SaveData();
            Console.WriteLine($"Data saved to {filePath}");

            // "Clear memory" by using a fresh app instance
            var app2 = new InventoryApp(filePath);
            app2.LoadData();
            app2.PrintAllItems();
        }
    }
}

