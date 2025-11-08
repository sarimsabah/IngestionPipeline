namespace MyApiApp.Models.Dashboard;

public class LogEntryDto
{
    public string ReferenceId { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public DateTime ReceivedTime { get; set; }
    public int ApiStatus { get; set; }
    public string? ValidationFailures { get; set; }
    public string ProcessingStatus { get; set; } = string.Empty;
    public string RawRequest { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

public class LogsResponse
{
    public List<LogEntryDto> Logs { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
