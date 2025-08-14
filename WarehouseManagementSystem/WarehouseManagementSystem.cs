using System;
using System.Collections.Generic;
using System.Linq;

namespace WarehouseManagementSystem
{
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

    public interface IInventoryItem
    {
        int Id { get; }
        string Name { get; }
        int Quantity { get; set; }
    }

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
            return $"ID: {Id}, Name: {Name}, Brand: {Brand}, Quantity: {Quantity}, Warranty: {WarrantyMonths} months";
        }
    }

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
            return $"ID: {Id}, Name: {Name}, Quantity: {Quantity}, Expires: {ExpiryDate:yyyy-MM-dd}";
        }
    }

    public class InventoryRepository<T> where T : IInventoryItem
    {
        private readonly Dictionary<int, T> _items = new Dictionary<int, T>();

        public void AddItem(T item)
        {
            if (_items.ContainsKey(item.Id))
            {
                throw new DuplicateItemException($"Error: Item with ID {item.Id} already exists.");
            }
            _items.Add(item.Id, item);
        }

        public T GetItemById(int id)
        {
            if (_items.TryGetValue(id, out T item))
            {
                return item;
            }
            throw new ItemNotFoundException($"Error: Item with ID {id} not found.");
        }

        public void RemoveItem(int id)
        {
            if (!_items.Remove(id))
            {
                throw new ItemNotFoundException($"Error: Could not remove. Item with ID {id} not found.");
            }
        }

        public List<T> GetAllItems()
        {
            return _items.Values.ToList();
        }

        public void UpdateQuantity(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                throw new InvalidQuantityException("Error: Quantity cannot be negative.");
            }
            T item = GetItemById(id);
            item.Quantity = newQuantity;
        }
    }

    public class WarehouseManager
    {
        private readonly InventoryRepository<ElectronicItem> _electronics;
        private readonly InventoryRepository<GroceryItem> _groceries;

        public WarehouseManager()
        {
            _electronics = new InventoryRepository<ElectronicItem>();
            _groceries = new InventoryRepository<GroceryItem>();
        }

        public void SeedData()
        {
            _electronics.AddItem(new ElectronicItem(101, "Laptop", 15, "TechBrand", 24));
            _electronics.AddItem(new ElectronicItem(102, "Smartphone", 50, "MobileCorp", 12));
            _groceries.AddItem(new GroceryItem(201, "Milk", 100, DateTime.Now.AddDays(7)));
            _groceries.AddItem(new GroceryItem(202, "Bread", 150, DateTime.Now.AddDays(3)));
        }

        public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
        {
            Console.WriteLine($"--- Listing All {typeof(T).Name}s ---");
            var items = repo.GetAllItems();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("No items in this category.");
            }
            Console.WriteLine();
        }
        
        public InventoryRepository<ElectronicItem> ElectronicsRepo => _electronics;
        public InventoryRepository<GroceryItem> GroceriesRepo => _groceries;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var manager = new WarehouseManager();
            manager.SeedData();

            manager.PrintAllItems(manager.GroceriesRepo);
            manager.PrintAllItems(manager.ElectronicsRepo);

            Console.WriteLine("--- Testing Exception Handling ---");

            try
            {
                Console.WriteLine("\nAttempting to add a duplicate item (ID 101)...");
                manager.ElectronicsRepo.AddItem(new ElectronicItem(101, "Duplicate Laptop", 5, "AnotherBrand", 12));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine(ex.Message);
            }

            try
            {
                Console.WriteLine("\nAttempting to remove a non-existent item (ID 999)...");
                manager.ElectronicsRepo.RemoveItem(999);
            }
            catch (ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            try
            {
                Console.WriteLine("\nAttempting to update an item with a negative quantity...");
                manager.GroceriesRepo.UpdateQuantity(201, -10);
            }
            catch (InvalidQuantityException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(ItemNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}