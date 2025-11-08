using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities;

[Table("log_customer_ingestion")]
public class CustomerIngestionLogEntity
{
    [Key]
    [Column("log_id")]
    public Guid LogId { get; set; } = Guid.NewGuid();

    [Column("request_time")]
    public DateTime RequestTime { get; set; } = DateTime.UtcNow;

    [Column("raw_payload")]
    [MaxLength(10000)]
    public string RawPayload { get; set; } = string.Empty;

    [Column("http_status")]
    public int HttpStatus { get; set; }

    [Column("status")]
    [MaxLength(50)]
    public string Status { get; set; } = "PENDING"; // SUCCESS, VALIDATION_FAILED

    [Column("validation_details")]
    [MaxLength(5000)]
    public string? ValidationDetails { get; set; }

    [Column("process_status")]
    [MaxLength(50)]
    public string ProcessStatus { get; set; } = "PENDING"; // PENDING, PROCESSED, ERROR

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }

    [Column("error_message")]
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    [Column("reference_id")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; }
}
