namespace MyApiApp.Services;

public static class ReferenceIdGenerator
{
    public static string GenerateCustomerReferenceId()
    {
        var dateString = DateTime.Now.ToString("yyyyMMdd");
        var randomId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"INGEST-C-{dateString}-{randomId}";
    }

    public static string GenerateItemReferenceId()
    {
        var dateString = DateTime.Now.ToString("yyyyMMdd");
        var randomId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        return $"INGEST-I-{dateString}-{randomId}";
    }
}
