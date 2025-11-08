using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_uom")]
public class MUomEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("uom_code")]
    [MaxLength(50)]
    public string UomCode { get; set; } = string.Empty;

    [Column("uom_description")]
    [MaxLength(255)]
    public string? UomDescription { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
