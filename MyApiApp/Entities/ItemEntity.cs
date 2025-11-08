using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities;

[Table("items")]
public class ItemEntity
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

    [Column("brand_code")]
    [MaxLength(50)]
    public string? BrandCode { get; set; }

    [Column("brand_name")]
    [MaxLength(255)]
    public string? BrandName { get; set; }

    [Column("category_code")]
    [MaxLength(50)]
    public string? CategoryCode { get; set; }

    [Column("category_name")]
    [MaxLength(255)]
    public string? CategoryName { get; set; }

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

    public ICollection<UomEntity> UomList { get; set; } = new List<UomEntity>();
}

[Table("item_uoms")]
public class UomEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("item_id")]
    public int ItemId { get; set; }

    [Column("uom")]
    [MaxLength(50)]
    public string? Uom { get; set; }

    [Column("conversion_factor")]
    public int ConversionFactor { get; set; }

    [ForeignKey("ItemId")]
    public ItemEntity Item { get; set; } = null!;
}
