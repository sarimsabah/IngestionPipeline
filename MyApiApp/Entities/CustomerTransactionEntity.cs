using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiApp.Entities;

[Table("customer_transactions")]
[Index(nameof(LogId))]
[Index(nameof(CustomerCode))]
[Index(nameof(TransactionStatus))]
public class CustomerTransactionEntity
{
    [Key]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; } = Guid.NewGuid();

    [Column("log_id")]
    public Guid LogId { get; set; }

    [Column("customer_code")]
    [MaxLength(50)]
    public string CustomerCode { get; set; } = string.Empty;

    [Column("customer_name")]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;

    [Column("transaction_type")]
    [MaxLength(20)]
    public string TransactionType { get; set; } = "INSERT"; // INSERT, UPDATE

    [Column("transaction_status")]
    [MaxLength(50)]
    public string TransactionStatus { get; set; } = "PENDING"; // PENDING, SUCCESS, FAILED

    [Column("transaction_time")]
    public DateTime TransactionTime { get; set; } = DateTime.UtcNow;

    [Column("completed_at")]
    public DateTime? CompletedAt { get; set; }

    [Column("error_message")]
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    [Column("reference_id")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    [ForeignKey("LogId")]
    public CustomerIngestionLogEntity? IngestionLog { get; set; }
}
