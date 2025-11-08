using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Entities.MasterData;

namespace MyApiApp.Entities.TransactionData;

[Table("t_item")]
[Index(nameof(ItemCode), IsUnique = true)]
public class TItemEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("item_code")]
    [MaxLength(50)]
    public string ItemCode { get; set; } = string.Empty;

    [Column("item_name")]
    [MaxLength(255)]
    public string ItemName { get; set; } = string.Empty;

    [Column("arabic_description")]
    [MaxLength(500)]
    public string? ArabicDescription { get; set; }

    [Column("sales_org_code")]
    [MaxLength(50)]
    public string? SalesOrgCode { get; set; }

    [Column("base_uom")]
    [MaxLength(50)]
    public string? BaseUOM { get; set; }

    // Foreign Keys to Master Data
    [Column("brand_id")]
    public int? BrandId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("is_active")]
    [MaxLength(10)]
    public string? IsActive { get; set; }

    [Column("is_batch_enabled")]
    public bool IsBatchEnabled { get; set; }

    [Column("business_type")]
    [MaxLength(50)]
    public string? BusinessType { get; set; }

    [Column("business_type_description")]
    [MaxLength(255)]
    public string? BusinessTypeDescription { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("reference_id")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    // Navigation Properties
    [ForeignKey("BrandId")]
    public MBrandEntity? Brand { get; set; }

    [ForeignKey("CategoryId")]
    public MCategoryEntity? Category { get; set; }

    public ICollection<TItemUomConversionEntity> UomConversions { get; set; } = new List<TItemUomConversionEntity>();
}
