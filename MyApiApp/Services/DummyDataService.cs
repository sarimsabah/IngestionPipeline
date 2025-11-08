using System.Text.Json;
using MyApiApp.Models;

namespace MyApiApp.Services;

public class DummyDataService
{
    private readonly List<CustomerRequest> _customers;
    private readonly List<ItemRequest> _items;
    private readonly string _dataPath;

    public DummyDataService()
    {
        _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "DummyData");
        _customers = LoadCustomers();
        _items = LoadItems();
    }

    private List<CustomerRequest> LoadCustomers()
    {
        var customers = new List<CustomerRequest>();

        if (!Directory.Exists(_dataPath))
        {
            return customers;
        }

        var customerFiles = Directory.GetFiles(_dataPath, "customer*.json");

        foreach (var file in customerFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var customer = JsonSerializer.Deserialize<CustomerRequest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (customer != null)
                {
                    customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading customer file {file}: {ex.Message}");
            }
        }

        return customers;
    }

    public CustomerRequest? GetCustomerByCode(string customerCode)
    {
        return _customers.FirstOrDefault(c =>
            c.CustomerCode?.Equals(customerCode, StringComparison.OrdinalIgnoreCase) == true);
    }

    public List<CustomerRequest> GetAllCustomers()
    {
        return _customers;
    }

    public CustomerRequest? GetRandomCustomer()
    {
        if (_customers.Count == 0)
            return null;

        var random = new Random();
        return _customers[random.Next(_customers.Count)];
    }

    private List<ItemRequest> LoadItems()
    {
        var items = new List<ItemRequest>();

        if (!Directory.Exists(_dataPath))
        {
            return items;
        }

        var itemFiles = Directory.GetFiles(_dataPath, "item*.json");

        foreach (var file in itemFiles)
        {
            try
            {
                var json = File.ReadAllText(file);
                var item = JsonSerializer.Deserialize<ItemRequest>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (item != null)
                {
                    items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading item file {file}: {ex.Message}");
            }
        }

        return items;
    }

    public ItemRequest? GetItemByCode(string itemCode)
    {
        return _items.FirstOrDefault(i =>
            i.Material?.ItemCode?.Equals(itemCode, StringComparison.OrdinalIgnoreCase) == true);
    }

    public List<ItemRequest> GetAllItems()
    {
        return _items;
    }

    public ItemRequest? GetRandomItem()
    {
        if (_items.Count == 0)
            return null;

        var random = new Random();
        return _items[random.Next(_items.Count)];
    }
}
