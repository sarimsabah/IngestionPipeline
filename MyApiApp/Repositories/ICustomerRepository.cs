using MyApiApp.Entities;

namespace MyApiApp.Repositories;

public interface ICustomerRepository
{
    Task<CustomerEntity?> GetByCodeAsync(string customerCode);
    Task<List<CustomerEntity>> GetAllAsync();
    Task<CustomerEntity> CreateAsync(CustomerEntity customer);
    Task<CustomerEntity> UpdateAsync(CustomerEntity customer);
    Task<bool> ExistsAsync(string customerCode);
}
