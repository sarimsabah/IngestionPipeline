namespace MyApiApp.Models;

public class SuccessResponse
{
    public string Status { get; set; } = "Success";
    public string Message { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
}

public class FailureResponse
{
    public string Status { get; set; } = "Failure";
    public string Message { get; set; } = string.Empty;
    public List<ValidationError> ValidationErrors { get; set; } = new();
}

public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
