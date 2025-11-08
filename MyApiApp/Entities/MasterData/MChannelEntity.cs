using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiApp.Entities.MasterData;

[Table("m_channel")]
public class MChannelEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("channel_code")]
    [MaxLength(50)]
    public string ChannelCode { get; set; } = string.Empty;

    [Column("channel_name")]
    [MaxLength(255)]
    public string ChannelName { get; set; } = string.Empty;

    [Column("sub_channel_code")]
    [MaxLength(50)]
    public string? SubChannelCode { get; set; }

    [Column("sub_channel_name")]
    [MaxLength(255)]
    public string? SubChannelName { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
