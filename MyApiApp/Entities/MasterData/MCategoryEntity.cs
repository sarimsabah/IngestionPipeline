using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_category")]
public class MCategoryEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("category_code")]
    [MaxLength(50)]
    public string CategoryCode { get; set; } = string.Empty;

    [Column("category_name")]
    [MaxLength(255)]
    public string CategoryName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
