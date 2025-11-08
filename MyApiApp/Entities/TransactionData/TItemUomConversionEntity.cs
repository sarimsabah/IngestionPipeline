using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Entities.MasterData;

namespace MyApiApp.Entities.TransactionData;

[Table("t_item_uom_conversion")]
[Index(nameof(ItemId), nameof(UomId), IsUnique = true)]
public class TItemUomConversionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("uom_id")]
    public int UomId { get; set; }

    [Column("conversion_factor")]
    public decimal ConversionFactor { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    [ForeignKey("ItemId")]
    public TItemEntity Item { get; set; } = null!;

    [ForeignKey("UomId")]
    public MUomEntity Uom { get; set; } = null!;
}
