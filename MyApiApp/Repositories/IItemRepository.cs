using MyApiApp.Entities;

namespace MyApiApp.Repositories;

public interface IItemRepository
{
    Task<ItemEntity?> GetByCodeAsync(string itemCode);
    Task<List<ItemEntity>> GetAllAsync();
    Task<ItemEntity> CreateAsync(ItemEntity item);
    Task<ItemEntity> UpdateAsync(ItemEntity item);
    Task<bool> ExistsAsync(string itemCode);
}
