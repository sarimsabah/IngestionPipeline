using MyApiApp.Models;
using MyApiApp.Services;
using MyApiApp.Data;
using MyApiApp.Repositories;
using MyApiApp.Entities;
using MyApiApp.BackgroundJobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSingleton<DummyDataService>();

// Add Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();

// Add Services
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Add Background Services
builder.Services.AddHostedService<CustomerMasterProcessor>();
builder.Services.AddHostedService<ItemMasterProcessor>();

// Add CORS for dashboard
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add authentication
builder.Services.AddAuthentication("Bearer")
    .AddScheme<AuthenticationSchemeOptions, BearerAuthenticationHandler>("Bearer", null);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowDashboard");
app.UseAuthentication();
app.UseAuthorization();

// Customer Ingest Endpoint
app.MapPost("/api/v1/customers/ingest", [Authorize] async (
    DummyDataService dummyDataService,
    ICustomerRepository customerRepo,
    AppDbContext dbContext,
    CustomerRequest? request) =>
{
    // If no request body provided, use random dummy data
    if (request == null || string.IsNullOrEmpty(request.CustomerCode))
    {
        request = dummyDataService.GetRandomCustomer();
        if (request == null)
        {
            return Results.BadRequest(new FailureResponse
            {
                Status = "Failure",
                Message = "No dummy data available and no valid request provided.",
                ValidationErrors = new List<ValidationError>()
            });
        }
    }

    // Step 1: Log to staging table immediately
    var logId = Guid.NewGuid();
    var rawPayload = System.Text.Json.JsonSerializer.Serialize(request);
    var ingestionLog = new CustomerIngestionLogEntity
    {
        LogId = logId,
        RawPayload = rawPayload,
        HttpStatus = 202, // Will be updated based on validation
        Status = "PENDING",
        ProcessStatus = "PENDING"
    };

    // Step 2: Validate request
    var validationErrors = ValidationService.ValidateCustomer(request);

    if (validationErrors.Any())
    {
        ingestionLog.HttpStatus = 400;
        ingestionLog.Status = "VALIDATION_FAILED";
        ingestionLog.ValidationDetails = System.Text.Json.JsonSerializer.Serialize(validationErrors);
        await dbContext.CustomerIngestionLogs.AddAsync(ingestionLog);
        await dbContext.SaveChangesAsync();

        return Results.BadRequest(new FailureResponse
        {
            Status = "Failure",
            Message = "Validation failed for one or more fields.",
            ValidationErrors = validationErrors
        });
    }

    // Validation passed
    var referenceId = logId.ToString(); // Use log_id as referenceId
    ingestionLog.Status = "SUCCESS";
    ingestionLog.HttpStatus = 202;
    ingestionLog.ReferenceId = referenceId;

    await dbContext.CustomerIngestionLogs.AddAsync(ingestionLog);

    // Step 3: Create transaction record
    var transaction = new CustomerTransactionEntity
    {
        LogId = logId,
        CustomerCode = request.CustomerCode!,
        CustomerName = request.CustomerName!,
        TransactionType = "INSERT",
        TransactionStatus = "PENDING",
        ReferenceId = referenceId
    };

    try
    {
        // Check if customer already exists
        var existingCustomer = await customerRepo.GetByCodeAsync(request.CustomerCode!);

        if (existingCustomer != null)
        {
            transaction.TransactionType = "UPDATE";
            MappingService.UpdateEntity(existingCustomer, request);
            existingCustomer.ReferenceId = referenceId;
            await customerRepo.UpdateAsync(existingCustomer);
        }
        else
        {
            var customerEntity = MappingService.ToEntity(request, referenceId);
            await customerRepo.CreateAsync(customerEntity);
        }

        transaction.TransactionStatus = "SUCCESS";
        transaction.CompletedAt = DateTime.UtcNow;
        ingestionLog.ProcessStatus = "PROCESSED";
        ingestionLog.ProcessedAt = DateTime.UtcNow;
    }
    catch (Exception ex)
    {
        transaction.TransactionStatus = "FAILED";
        transaction.ErrorMessage = ex.Message;
        ingestionLog.ProcessStatus = "ERROR";
        ingestionLog.ErrorMessage = ex.Message;
    }

    await dbContext.CustomerTransactions.AddAsync(transaction);
    await dbContext.SaveChangesAsync();

    return Results.Accepted(null, new SuccessResponse
    {
        Status = "Success",
        Message = "Customer data received and queued for processing.",
        ReferenceId = referenceId
    });
})
.WithName("IngestCustomer")
.WithOpenApi();

// Get All Customers (for testing)
app.MapGet("/api/v1/customers/dummy", [Authorize] (DummyDataService dummyDataService) =>
{
    return Results.Ok(dummyDataService.GetAllCustomers());
})
.WithName("GetDummyCustomers")
.WithOpenApi();

// Item Ingest Endpoint
app.MapPost("/api/v1/items/ingest", [Authorize] async (
    DummyDataService dummyDataService,
    IItemRepository itemRepo,
    AppDbContext dbContext,
    ItemRequest? request) =>
{
    // If no request body provided or missing material data, use random dummy data
    if (request?.Material == null || string.IsNullOrEmpty(request.Material.ItemCode))
    {
        request = dummyDataService.GetRandomItem();
        if (request == null)
        {
            return Results.BadRequest(new FailureResponse
            {
                Status = "Failure",
                Message = "No dummy data available and no valid request provided.",
                ValidationErrors = new List<ValidationError>()
            });
        }
    }

    // Step 1: Log to staging table immediately
    var logId = Guid.NewGuid();
    var rawPayload = System.Text.Json.JsonSerializer.Serialize(request);
    var ingestionLog = new ItemIngestionLogEntity
    {
        LogId = logId,
        RawPayload = rawPayload,
        HttpStatus = 202,
        Status = "PENDING",
        ProcessStatus = "PENDING"
    };

    // Step 2: Validate request
    var validationErrors = ValidationService.ValidateItem(request);

    if (validationErrors.Any())
    {
        ingestionLog.HttpStatus = 400;
        ingestionLog.Status = "VALIDATION_FAILED";
        ingestionLog.ValidationDetails = System.Text.Json.JsonSerializer.Serialize(validationErrors);
        await dbContext.ItemIngestionLogs.AddAsync(ingestionLog);
        await dbContext.SaveChangesAsync();

        return Results.BadRequest(new FailureResponse
        {
            Status = "Failure",
            Message = "Validation failed for one or more fields.",
            ValidationErrors = validationErrors
        });
    }

    // Validation passed
    var referenceId = logId.ToString(); // Use log_id as referenceId
    ingestionLog.Status = "SUCCESS";
    ingestionLog.HttpStatus = 202;
    ingestionLog.ReferenceId = referenceId;

    await dbContext.ItemIngestionLogs.AddAsync(ingestionLog);

    // Step 3: Create transaction record
    var transaction = new ItemTransactionEntity
    {
        LogId = logId,
        ItemCode = request.Material!.ItemCode!,
        ItemName = request.Material.ItemName!,
        TransactionType = "INSERT",
        TransactionStatus = "PENDING",
        ReferenceId = referenceId
    };

    try
    {
        // Check if item already exists
        var existingItem = await itemRepo.GetByCodeAsync(request.Material!.ItemCode!);

        if (existingItem != null)
        {
            transaction.TransactionType = "UPDATE";
            existingItem.ItemName = request.Material.ItemName ?? existingItem.ItemName;
            existingItem.ArabicDescription = request.Material.ArabicDescription ?? existingItem.ArabicDescription;
            existingItem.ReferenceId = referenceId;
            existingItem.UpdatedAt = DateTime.UtcNow;
            await itemRepo.UpdateAsync(existingItem);
        }
        else
        {
            var itemEntity = MappingService.ToEntity(request, referenceId);
            await itemRepo.CreateAsync(itemEntity);
        }

        transaction.TransactionStatus = "SUCCESS";
        transaction.CompletedAt = DateTime.UtcNow;
        ingestionLog.ProcessStatus = "PROCESSED";
        ingestionLog.ProcessedAt = DateTime.UtcNow;
    }
    catch (Exception ex)
    {
        transaction.TransactionStatus = "FAILED";
        transaction.ErrorMessage = ex.Message;
        ingestionLog.ProcessStatus = "ERROR";
        ingestionLog.ErrorMessage = ex.Message;
    }

    await dbContext.ItemTransactions.AddAsync(transaction);
    await dbContext.SaveChangesAsync();

    return Results.Accepted(null, new SuccessResponse
    {
        Status = "Success",
        Message = "Item data received and queued for processing.",
        ReferenceId = referenceId
    });
})
.WithName("IngestItem")
.WithOpenApi();

// Get All Items (for testing)
app.MapGet("/api/v1/items/dummy", [Authorize] (DummyDataService dummyDataService) =>
{
    return Results.Ok(dummyDataService.GetAllItems());
})
.WithName("GetDummyItems")
.WithOpenApi();

// Dashboard Endpoints (No Auth for easier access)
app.MapGet("/api/v1/dashboard/metrics", async (IDashboardService dashboardService) =>
{
    var metrics = await dashboardService.GetDashboardMetricsAsync();
    return Results.Ok(metrics);
})
.WithName("GetDashboardMetrics")
.WithOpenApi();

app.MapGet("/api/v1/dashboard/logs", async (
    IDashboardService dashboardService,
    string? domain,
    int pageNumber = 1,
    int pageSize = 20) =>
{
    var logs = await dashboardService.GetLogsAsync(domain, pageNumber, pageSize);
    return Results.Ok(logs);
})
.WithName("GetDashboardLogs")
.WithOpenApi();

app.Run();
