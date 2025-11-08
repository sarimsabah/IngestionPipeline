using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_region")]
public class MRegionEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("region_code")]
    [MaxLength(50)]
    public string RegionCode { get; set; } = string.Empty;

    [Column("region_name")]
    [MaxLength(100)]
    public string RegionName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
