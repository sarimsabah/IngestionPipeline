using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_brand")]
public class MBrandEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("brand_code")]
    [MaxLength(50)]
    public string BrandCode { get; set; } = string.Empty;

    [Column("brand_name")]
    [MaxLength(255)]
    public string BrandName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
