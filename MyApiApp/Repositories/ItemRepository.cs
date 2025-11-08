using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Entities;

namespace MyApiApp.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ItemEntity?> GetByCodeAsync(string itemCode)
    {
        return await _context.Items
            .Include(i => i.UomList)
            .FirstOrDefaultAsync(i => i.ItemCode == itemCode);
    }

    public async Task<List<ItemEntity>> GetAllAsync()
    {
        return await _context.Items
            .Include(i => i.UomList)
            .ToListAsync();
    }

    public async Task<ItemEntity> CreateAsync(ItemEntity item)
    {
        _context.Items.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<ItemEntity> UpdateAsync(ItemEntity item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _context.Items.Update(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<bool> ExistsAsync(string itemCode)
    {
        return await _context.Items
            .AnyAsync(i => i.ItemCode == itemCode);
    }
}
