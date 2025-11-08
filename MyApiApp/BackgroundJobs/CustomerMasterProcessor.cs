using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Entities;
using MyApiApp.Entities.MasterData;
using MyApiApp.Entities.TransactionData;
using MyApiApp.Models;

namespace MyApiApp.BackgroundJobs;

public class CustomerMasterProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CustomerMasterProcessor> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(10); // Process every 10 seconds

    public CustomerMasterProcessor(
        IServiceProvider serviceProvider,
        ILogger<CustomerMasterProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CustomerMasterProcessor background job started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingCustomers(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in CustomerMasterProcessor.");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("CustomerMasterProcessor background job stopped.");
    }

    private async Task ProcessPendingCustomers(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Get pending records: status = SUCCESS and process_status = PENDING
        var pendingLogs = await dbContext.CustomerIngestionLogs
            .Where(log => log.Status == "SUCCESS" && log.ProcessStatus == "PENDING")
            .OrderBy(log => log.RequestTime)
            .Take(10) // Process 10 at a time
            .ToListAsync(stoppingToken);

        if (!pendingLogs.Any())
        {
            return; // No pending records
        }

        _logger.LogInformation($"Processing {pendingLogs.Count} pending customer records.");

        foreach (var log in pendingLogs)
        {
            try
            {
                // Deserialize the raw payload
                var customerRequest = System.Text.Json.JsonSerializer.Deserialize<CustomerRequest>(log.RawPayload);

                if (customerRequest == null)
                {
                    throw new Exception("Failed to deserialize customer request.");
                }

                // Step 1: Upsert Master Data
                var regionId = await UpsertRegion(dbContext, customerRequest);
                var cityId = await UpsertCity(dbContext, customerRequest);
                var paymentTermId = await UpsertPaymentTerm(dbContext, customerRequest);
                var channelId = await UpsertChannel(dbContext, customerRequest);

                // Step 2: Upsert Transaction Customer Data
                await UpsertTransactionCustomer(dbContext, customerRequest, regionId, cityId, paymentTermId, channelId, log.ReferenceId);

                // Step 3: Mark as PROCESSED
                log.ProcessStatus = "PROCESSED";
                log.ProcessedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation($"Successfully processed customer: {customerRequest.CustomerCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing customer log ID: {log.LogId}");

                log.ProcessStatus = "ERROR";
                log.ErrorMessage = ex.Message.Length > 2000 ? ex.Message.Substring(0, 2000) : ex.Message;
                log.ProcessedAt = DateTime.UtcNow;

                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }

    private async Task<int?> UpsertRegion(AppDbContext dbContext, CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RegionCode))
            return null;

        var existing = await dbContext.MasterRegions
            .FirstOrDefaultAsync(r => r.RegionCode == request.RegionCode);

        if (existing != null)
        {
            // Update if needed
            existing.RegionName = request.RegionName ?? existing.RegionName;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        // Insert new
        var newRegion = new MRegionEntity
        {
            RegionCode = request.RegionCode,
            RegionName = request.RegionName ?? string.Empty
        };

        dbContext.MasterRegions.Add(newRegion);
        await dbContext.SaveChangesAsync();

        return newRegion.Id;
    }

    private async Task<int?> UpsertCity(AppDbContext dbContext, CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CityCode))
            return null;

        var existing = await dbContext.MasterCities
            .FirstOrDefaultAsync(c => c.CityCode == request.CityCode);

        if (existing != null)
        {
            existing.CityName = request.CityName ?? existing.CityName;
            existing.CityNameArabic = request.CityNameArabic ?? existing.CityNameArabic;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        var newCity = new MCityEntity
        {
            CityCode = request.CityCode,
            CityName = request.CityName ?? string.Empty,
            CityNameArabic = request.CityNameArabic
        };

        dbContext.MasterCities.Add(newCity);
        await dbContext.SaveChangesAsync();

        return newCity.Id;
    }

    private async Task<int?> UpsertPaymentTerm(AppDbContext dbContext, CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PaymentTermCode))
            return null;

        var existing = await dbContext.MasterPaymentTerms
            .FirstOrDefaultAsync(p => p.PaymentTermCode == request.PaymentTermCode);

        if (existing != null)
        {
            existing.PaymentTermName = request.PaymentTermName ?? existing.PaymentTermName;
            existing.CreditDays = request.CreditDays ?? existing.CreditDays;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        var newPaymentTerm = new MPaymentTermEntity
        {
            PaymentTermCode = request.PaymentTermCode,
            PaymentTermName = request.PaymentTermName ?? string.Empty,
            CreditDays = request.CreditDays
        };

        dbContext.MasterPaymentTerms.Add(newPaymentTerm);
        await dbContext.SaveChangesAsync();

        return newPaymentTerm.Id;
    }

    private async Task<int?> UpsertChannel(AppDbContext dbContext, CustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ChannelCode))
            return null;

        var existing = await dbContext.MasterChannels
            .FirstOrDefaultAsync(c => c.ChannelCode == request.ChannelCode);

        if (existing != null)
        {
            existing.ChannelName = request.ChannelName ?? existing.ChannelName;
            existing.SubChannelCode = request.SubChannelCode ?? existing.SubChannelCode;
            existing.SubChannelName = request.SubChannelName ?? existing.SubChannelName;
            existing.UpdatedAt = DateTime.UtcNow;
            return existing.Id;
        }

        var newChannel = new MChannelEntity
        {
            ChannelCode = request.ChannelCode,
            ChannelName = request.ChannelName ?? string.Empty,
            SubChannelCode = request.SubChannelCode,
            SubChannelName = request.SubChannelName
        };

        dbContext.MasterChannels.Add(newChannel);
        await dbContext.SaveChangesAsync();

        return newChannel.Id;
    }

    private async Task UpsertTransactionCustomer(
        AppDbContext dbContext,
        CustomerRequest request,
        int? regionId,
        int? cityId,
        int? paymentTermId,
        int? channelId,
        string? referenceId)
    {
        var existing = await dbContext.TransactionCustomers
            .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode);

        if (existing != null)
        {
            // Update existing
            existing.CustomerName = request.CustomerName ?? existing.CustomerName;
            existing.ArabicDescription = request.ArabicDescription ?? existing.ArabicDescription;
            existing.ParentCustomerCode = request.ParentCustomerCode ?? existing.ParentCustomerCode;
            existing.ParentCustomerName = request.ParentCustomerName ?? existing.ParentCustomerName;
            existing.ContactNo = request.ContactNo ?? existing.ContactNo;
            existing.Fax = request.Fax ?? existing.Fax;
            existing.Email = request.Email ?? existing.Email;
            existing.Address1 = request.Address1 ?? existing.Address1;
            existing.Address2 = request.Address2 ?? existing.Address2;
            existing.Address3 = request.Address3 ?? existing.Address3;
            existing.Address4 = request.Address4 ?? existing.Address4;
            existing.IsActive = request.IsActive ?? existing.IsActive;
            existing.Longitude = request.Longitude ?? existing.Longitude;
            existing.Latitude = request.Latitude ?? existing.Latitude;
            existing.ContactPersonName = request.ContactPersonName ?? existing.ContactPersonName;
            existing.RegionId = regionId ?? existing.RegionId;
            existing.CityId = cityId ?? existing.CityId;
            existing.PaymentTermId = paymentTermId ?? existing.PaymentTermId;
            existing.ChannelId = channelId ?? existing.ChannelId;
            existing.PriceListCode = request.PriceListCode ?? existing.PriceListCode;
            existing.CustomerGroupCode = request.CustomerGroupCode ?? existing.CustomerGroupCode;
            existing.CustomerGroupName = request.CustomerGroupName ?? existing.CustomerGroupName;
            existing.CreditLimit = request.CreditLimit ?? existing.CreditLimit;
            existing.CreditDays = request.CreditDays ?? existing.CreditDays;
            existing.CustomerType = request.CustomerType ?? existing.CustomerType;
            existing.IsBlocked = request.IsBlocked ?? existing.IsBlocked;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.ReferenceId = referenceId;
        }
        else
        {
            // Insert new
            var newCustomer = new TCustomerEntity
            {
                CustomerCode = request.CustomerCode ?? string.Empty,
                CustomerName = request.CustomerName ?? string.Empty,
                ArabicDescription = request.ArabicDescription,
                ParentCustomerCode = request.ParentCustomerCode,
                ParentCustomerName = request.ParentCustomerName,
                ContactNo = request.ContactNo,
                Fax = request.Fax,
                Email = request.Email,
                Address1 = request.Address1,
                Address2 = request.Address2,
                Address3 = request.Address3,
                Address4 = request.Address4,
                IsActive = request.IsActive ?? false,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                ContactPersonName = request.ContactPersonName,
                RegionId = regionId,
                CityId = cityId,
                PaymentTermId = paymentTermId,
                ChannelId = channelId,
                PriceListCode = request.PriceListCode,
                CustomerGroupCode = request.CustomerGroupCode,
                CustomerGroupName = request.CustomerGroupName,
                CreditLimit = request.CreditLimit,
                CreditDays = request.CreditDays,
                CustomerType = request.CustomerType,
                IsBlocked = request.IsBlocked ?? false,
                ReferenceId = referenceId
            };

            dbContext.TransactionCustomers.Add(newCustomer);
        }

        await dbContext.SaveChangesAsync();
    }
}
