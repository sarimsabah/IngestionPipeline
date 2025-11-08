namespace MyApiApp.Models.Dashboard;

public class DashboardMetrics
{
    public int TotalRequests { get; set; }
    public DomainMetrics CustomerMetrics { get; set; } = new();
    public DomainMetrics ItemMetrics { get; set; } = new();
    public IngestionStatusMatrix IngestionStatus { get; set; } = new();
    public ProcessingStatusMatrix ProcessingStatus { get; set; } = new();
}

public class DomainMetrics
{
    public int TotalRequests { get; set; }
    public decimal ApiSuccessRate { get; set; }
    public decimal ProcessingSuccessRate { get; set; }
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public int ProcessedCount { get; set; }
    public int PendingCount { get; set; }
    public int ErrorCount { get; set; }
}

public class IngestionStatusMatrix
{
    public int ApiSuccess { get; set; }
    public int ApiFailure { get; set; }
}

public class ProcessingStatusMatrix
{
    public int Pending { get; set; }
    public int Processed { get; set; }
    public int Error { get; set; }
}
