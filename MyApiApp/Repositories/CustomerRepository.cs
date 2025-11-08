using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Entities;

namespace MyApiApp.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerEntity?> GetByCodeAsync(string customerCode)
    {
        return await _context.Customers
            .FirstOrDefaultAsync(c => c.CustomerCode == customerCode);
    }

    public async Task<List<CustomerEntity>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<CustomerEntity> CreateAsync(CustomerEntity customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<CustomerEntity> UpdateAsync(CustomerEntity customer)
    {
        customer.UpdatedAt = DateTime.UtcNow;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<bool> ExistsAsync(string customerCode)
    {
        return await _context.Customers
            .AnyAsync(c => c.CustomerCode == customerCode);
    }
}
