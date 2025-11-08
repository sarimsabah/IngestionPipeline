using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Entities;
using MyApiApp.Entities.MasterData;
using MyApiApp.Entities.TransactionData;
using MyApiApp.Models;

namespace MyApiApp.BackgroundJobs;

public class ItemMasterProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ItemMasterProcessor> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(10); // Process every 10 seconds

    public ItemMasterProcessor(
        IServiceProvider serviceProvider,
        ILogger<ItemMasterProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ItemMasterProcessor background job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingItems(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ItemMasterProcessor.");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("ItemMasterProcessor background job stopped.");
    }

    private async Task ProcessPendingItems(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Get pending records: status = SUCCESS and process_status = PENDING
        var pendingLogs = await dbContext.ItemIngestionLogs
            .Where(log => log.Status == "SUCCESS" && log.ProcessStatus == "PENDING")
            .OrderBy(log => log.RequestTime)
            .Take(10) // Process 10 at a time
            .ToListAsync(stoppingToken);

        if (!pendingLogs.Any())
        {
            return; // No pending records
        }

        _logger.LogInformation($"Processing {pendingLogs.Count} pending item records.");

        foreach (var log in pendingLogs)
        {
            try
            {
                // Deserialize the raw payload
                var itemRequest = System.Text.Json.JsonSerializer.Deserialize<ItemRequest>(log.RawPayload);

                if (itemRequest?.Material == null)
                {
                    throw new Exception("Failed to deserialize item request or material is null.");
                }

                // Step 1: Upsert Master Data
                var brandId = await UpsertBrand(dbContext, itemRequest.Material);
                var categoryId = await UpsertCategory(dbContext, itemRequest.Material);
                var uomIds = await UpsertUoms(dbContext, itemRequest.Material);

                // Step 2: Upsert Transaction Item Data
                await UpsertTransactionItem(dbContext, itemRequest.Material, brandId, categoryId, log.ReferenceId);

                // Step 3: Upsert Item-UOM Conversions
                await UpsertItemUomConversions(dbContext, itemRequest.Material, uomIds);

                // Step 4: Mark as PROCESSED
                log.ProcessStatus = "PROCESSED";
                log.ProcessedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation($"Successfully processed item: {itemRequest.Material.ItemCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing item log ID: {log.LogId}");

                log.ProcessStatus = "ERROR";
                log.ErrorMessage = ex.Message.Length > 2000 ? ex.Message.Substring(0, 2000) : ex.Message;
                log.ProcessedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }

    private async Task<int?> UpsertBrand(AppDbContext dbContext, MaterialData material)
    {
        if (string.IsNullOrWhiteSpace(material.BrandCode))
            return null;

        var existing = await dbContext.MasterBrands
            .FirstOrDefaultAsync(b => b.BrandCode == material.BrandCode);

        if (existing != null)
        {
            existing.BrandName = material.BrandName ?? existing.BrandName;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        var newBrand = new MBrandEntity
        {
            BrandCode = material.BrandCode,
            BrandName = material.BrandName ?? string.Empty
        };

        dbContext.MasterBrands.Add(newBrand);
        await dbContext.SaveChangesAsync();

        return newBrand.Id;
    }

    private async Task<int?> UpsertCategory(AppDbContext dbContext, MaterialData material)
    {
        if (string.IsNullOrWhiteSpace(material.CategoryCode))
            return null;

        var existing = await dbContext.MasterCategories
            .FirstOrDefaultAsync(c => c.CategoryCode == material.CategoryCode);

        if (existing != null)
        {
            existing.CategoryName = material.CategoryName ?? existing.CategoryName;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        var newCategory = new MCategoryEntity
        {
            CategoryCode = material.CategoryCode,
            CategoryName = material.CategoryName ?? string.Empty
        };

        dbContext.MasterCategories.Add(newCategory);
        await dbContext.SaveChangesAsync();

        return newCategory.Id;
    }

    private async Task<Dictionary<string, int>> UpsertUoms(AppDbContext dbContext, MaterialData material)
    {
        var uomIds = new Dictionary<string, int>();

        if (material.UomList == null || !material.UomList.Any())
            return uomIds;

        foreach (var uomData in material.UomList)
        {
            if (string.IsNullOrWhiteSpace(uomData.Uom))
                continue;

            var existing = await dbContext.MasterUoms
                .FirstOrDefaultAsync(u => u.UomCode == uomData.Uom);

            if (existing != null)
            {
                uomIds[uomData.Uom] = existing.Id;
            }
            else
            {
                var newUom = new MUomEntity
                {
                    UomCode = uomData.Uom,
                    UomDescription = uomData.Uom
                };

                dbContext.MasterUoms.Add(newUom);
                await dbContext.SaveChangesAsync();

                uomIds[uomData.Uom] = newUom.Id;
            }
        }

        return uomIds;
    }

    private async Task UpsertTransactionItem(
        AppDbContext dbContext,
        MaterialData material,
        int? brandId,
        int? categoryId,
        string? referenceId)
    {
        var existing = await dbContext.TransactionItems
            .FirstOrDefaultAsync(i => i.ItemCode == material.ItemCode);

        if (existing != null)
        {
            // Update existing
            existing.ItemName = material.ItemName ?? existing.ItemName;
            existing.ArabicDescription = material.ArabicDescription ?? existing.ArabicDescription;
            existing.SalesOrgCode = material.SalesOrgCode ?? existing.SalesOrgCode;
            existing.BaseUOM = material.BaseUOM ?? existing.BaseUOM;
            existing.BrandId = brandId ?? existing.BrandId;
            existing.CategoryId = categoryId ?? existing.CategoryId;
            existing.IsActive = material.IsActive ?? existing.IsActive;
            existing.IsBatchEnabled = material.IsBatchEnabled ?? existing.IsBatchEnabled;
            existing.BusinessType = material.BusinessType ?? existing.BusinessType;
            existing.BusinessTypeDescription = material.BusinessTypeDescription ?? existing.BusinessTypeDescription;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.ReferenceId = referenceId;
        }
        else
        {
            // Insert new
            var newItem = new TItemEntity
            {
                ItemCode = material.ItemCode ?? string.Empty,
                ItemName = material.ItemName ?? string.Empty,
                ArabicDescription = material.ArabicDescription,
                SalesOrgCode = material.SalesOrgCode,
                BaseUOM = material.BaseUOM,
                BrandId = brandId,
                CategoryId = categoryId,
                IsActive = material.IsActive,
                IsBatchEnabled = material.IsBatchEnabled ?? false,
                BusinessType = material.BusinessType,
                BusinessTypeDescription = material.BusinessTypeDescription,
                ReferenceId = referenceId
            };

            dbContext.TransactionItems.Add(newItem);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task UpsertItemUomConversions(
        AppDbContext dbContext,
        MaterialData material,
        Dictionary<string, int> uomIds)
    {
        if (material.UomList == null || !material.UomList.Any())
            return;

        // Get the item ID
        var item = await dbContext.TransactionItems
            .FirstOrDefaultAsync(i => i.ItemCode == material.ItemCode);

        if (item == null)
            return;

        // Remove existing conversions for this item (to handle updates cleanly)
        var existingConversions = await dbContext.TransactionItemUomConversions
            .Where(c => c.ItemId == item.Id)
            .ToListAsync();

        dbContext.TransactionItemUomConversions.RemoveRange(existingConversions);
        await dbContext.SaveChangesAsync();

        // Add new conversions
        foreach (var uomData in material.UomList)
        {
            if (string.IsNullOrWhiteSpace(uomData.Uom) || !uomIds.ContainsKey(uomData.Uom))
                continue;

            var conversion = new TItemUomConversionEntity
            {
                ItemId = item.Id,
                UomId = uomIds[uomData.Uom],
                ConversionFactor = uomData.ConversionFactor ?? 1
            };

            dbContext.TransactionItemUomConversions.Add(conversion);
        }

        await dbContext.SaveChangesAsync();
    }
}
