using Microsoft.EntityFrameworkCore;
using MyApiApp.Data;
using MyApiApp.Models.Dashboard;

namespace MyApiApp.Services;

public interface IDashboardService
{
    Task<DashboardMetrics> GetDashboardMetricsAsync();
    Task<LogsResponse> GetLogsAsync(string? domain, int pageNumber, int pageSize);
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;

    public DashboardService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardMetrics> GetDashboardMetricsAsync()
    {
        // Get customer metrics
        var customerLogs = await _dbContext.CustomerIngestionLogs.ToListAsync();
        var customerMetrics = CalculateDomainMetrics(
            customerLogs.Count,
            customerLogs.Count(l => l.HttpStatus == 202),
            customerLogs.Count(l => l.HttpStatus == 400),
            customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PROCESSED"),
            customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PENDING"),
            customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "ERROR"),
            customerLogs.Count(l => l.Status == "SUCCESS")
        );

        // Get item metrics
        var itemLogs = await _dbContext.ItemIngestionLogs.ToListAsync();
        var itemMetrics = CalculateDomainMetrics(
            itemLogs.Count,
            itemLogs.Count(l => l.HttpStatus == 202),
            itemLogs.Count(l => l.HttpStatus == 400),
            itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PROCESSED"),
            itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PENDING"),
            itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "ERROR"),
            itemLogs.Count(l => l.Status == "SUCCESS")
        );

        var totalRequests = customerLogs.Count + itemLogs.Count;
        var totalApiSuccess = customerLogs.Count(l => l.HttpStatus == 202) + itemLogs.Count(l => l.HttpStatus == 202);
        var totalApiFailure = customerLogs.Count(l => l.HttpStatus == 400) + itemLogs.Count(l => l.HttpStatus == 400);

        var totalPending = customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PENDING") +
                          itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PENDING");
        var totalProcessed = customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PROCESSED") +
                            itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "PROCESSED");
        var totalError = customerLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "ERROR") +
                        itemLogs.Count(l => l.Status == "SUCCESS" && l.ProcessStatus == "ERROR");

        return new DashboardMetrics
        {
            TotalRequests = totalRequests,
            CustomerMetrics = customerMetrics,
            ItemMetrics = itemMetrics,
            IngestionStatus = new IngestionStatusMatrix
            {
                ApiSuccess = totalApiSuccess,
                ApiFailure = totalApiFailure
            },
            ProcessingStatus = new ProcessingStatusMatrix
            {
                Pending = totalPending,
                Processed = totalProcessed,
                Error = totalError
            }
        };
    }

    public async Task<LogsResponse> GetLogsAsync(string? domain, int pageNumber, int pageSize)
    {
        var logs = new List<LogEntryDto>();

        if (domain == null || domain.ToLower() == "customer")
        {
            var customerLogs = await _dbContext.CustomerIngestionLogs
                .OrderByDescending(l => l.RequestTime)
                .ToListAsync();

            logs.AddRange(customerLogs.Select(l => new LogEntryDto
            {
                ReferenceId = l.ReferenceId ?? l.LogId.ToString(),
                Domain = "Customer",
                ReceivedTime = l.RequestTime,
                ApiStatus = l.HttpStatus,
                ValidationFailures = l.ValidationDetails,
                ProcessingStatus = l.ProcessStatus,
                RawRequest = l.RawPayload,
                ErrorMessage = l.ErrorMessage
            }));
        }

        if (domain == null || domain.ToLower() == "item")
        {
            var itemLogs = await _dbContext.ItemIngestionLogs
                .OrderByDescending(l => l.RequestTime)
                .ToListAsync();

            logs.AddRange(itemLogs.Select(l => new LogEntryDto
            {
                ReferenceId = l.ReferenceId ?? l.LogId.ToString(),
                Domain = "Item",
                ReceivedTime = l.RequestTime,
                ApiStatus = l.HttpStatus,
                ValidationFailures = l.ValidationDetails,
                ProcessingStatus = l.ProcessStatus,
                RawRequest = l.RawPayload,
                ErrorMessage = l.ErrorMessage
            }));
        }

        var orderedLogs = logs.OrderByDescending(l => l.ReceivedTime).ToList();
        var totalCount = orderedLogs.Count;
        var pagedLogs = orderedLogs
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new LogsResponse
        {
            Logs = pagedLogs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private DomainMetrics CalculateDomainMetrics(
        int total,
        int successCount,
        int failureCount,
        int processedCount,
        int pendingCount,
        int errorCount,
        int successfulLogsCount)
    {
        if (total == 0)
        {
            return new DomainMetrics();
        }

        var apiSuccessRate = total > 0 ? (decimal)successCount / total * 100 : 0;
        var processingSuccessRate = successfulLogsCount > 0
            ? (decimal)processedCount / successfulLogsCount * 100
            : 0;

        return new DomainMetrics
        {
            TotalRequests = total,
            ApiSuccessRate = Math.Round(apiSuccessRate, 2),
            ProcessingSuccessRate = Math.Round(processingSuccessRate, 2),
            SuccessCount = successCount,
            FailureCount = failureCount,
            ProcessedCount = processedCount,
            PendingCount = pendingCount,
            ErrorCount = errorCount
        };
    }
}
