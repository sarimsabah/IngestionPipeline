using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiApp.Entities;

[Table("item_transactions")]
[Index(nameof(LogId))]
[Index(nameof(ItemCode))]
[Index(nameof(TransactionStatus))]
public class ItemTransactionEntity
{
    [Key]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; } = Guid.NewGuid();

    [Column("log_id")]
    public Guid LogId { get; set; }

    [Column("item_code")]
    [MaxLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Column("item_name")]
    [MaxLength(255)]
    public string ItemName { get; set; } = string.Empty;

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
    public ItemIngestionLogEntity? IngestionLog { get; set; }
}
