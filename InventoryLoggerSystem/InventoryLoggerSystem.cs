using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace InventoryLoggerSystem
{
    public interface IInventoryEntity
    {
        int Id { get; }
    }

    public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

    public class InventoryLogger<T> where T : IInventoryEntity
    {
        private List<T> _log = new List<T>();
        private readonly string _filePath;

        public InventoryLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Add(T item)
        {
            _log.Add(item);
        }

        public List<T> GetAll()
        {
            return _log;
        }

        public void ClearLog()
        {
            _log.Clear();
        }

        public void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_log, options);
            File.WriteAllText(_filePath, jsonString);
        }

        public void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                _log = new List<T>();
                return;
            }
            string jsonString = File.ReadAllText(_filePath);
            var loadedItems = JsonSerializer.Deserialize<List<T>>(jsonString);
            _log = loadedItems ?? new List<T>();
        }
    }

    public class InventoryApp
    {
        private InventoryLogger<InventoryItem> _logger;
        private readonly string _logFile = "inventory_log.json";

        public InventoryApp()
        {
            _logger = new InventoryLogger<InventoryItem>(_logFile);
        }

        public void SeedSampleData()
        {
            Console.WriteLine("Seeding sample data...");
            _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
            _logger.Add(new InventoryItem(2, "Mouse", 50, DateTime.Now));
            _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
            _logger.Add(new InventoryItem(4, "Monitor", 25, DateTime.Now));
        }

        public void SaveData()
        {
            Console.WriteLine("Saving data to disk...");
            try
            {
                _logger.SaveToFile();
                Console.WriteLine($"Data successfully saved to '{_logFile}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        public void LoadData()
        {
            Console.WriteLine("Loading data from disk...");
            try
            {
                _logger.LoadFromFile();
                 Console.WriteLine($"Data successfully loaded from '{_logFile}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }
        
        public void ClearDataInMemory()
        {
            Console.WriteLine("Clearing data from application memory...");
            _logger.ClearLog();
        }

        public void PrintAllItems()
        {
            Console.WriteLine("\n--- Current Inventory Items ---");
            var items = _logger.GetAll();
            if(items.Any())
            {
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("No items in inventory.");
            }
             Console.WriteLine("-----------------------------");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Session 1: Seeding and Saving ---");
            var app1 = new InventoryApp();
            app1.SeedSampleData();
            app1.PrintAllItems();
            app1.SaveData();

            Console.WriteLine("\n--- Session 2: Simulating App Restart and Loading ---");
            var app2 = new InventoryApp();
            Console.WriteLine("New app instance created. Current state:");
            app2.PrintAllItems();
            
            app2.LoadData();
            Console.WriteLine("Data reloaded. Current state:");
            app2.PrintAllItems();
        }
    }
}