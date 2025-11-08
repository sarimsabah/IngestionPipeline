using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_payment_term")]
public class MPaymentTermEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("payment_term_code")]
    [MaxLength(50)]
    public string PaymentTermCode { get; set; } = string.Empty;

    [Column("payment_term_name")]
    [MaxLength(100)]
    public string PaymentTermName { get; set; } = string.Empty;

    [Column("credit_days")]
    public int? CreditDays { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
