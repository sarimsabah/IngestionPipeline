using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_city")]
public class MCityEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("city_code")]
    [MaxLength(50)]
    public string CityCode { get; set; } = string.Empty;

    [Column("city_name")]
    [MaxLength(100)]
    public string CityName { get; set; } = string.Empty;

    [Column("city_name_arabic")]
    [MaxLength(100)]
    public string? CityNameArabic { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
