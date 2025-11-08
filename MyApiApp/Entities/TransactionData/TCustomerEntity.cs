using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MyApiApp.Entities.MasterData;

namespace MyApiApp.Entities.TransactionData;

[Table("t_customer")]
[Index(nameof(CustomerCode), IsUnique = true)]
public class TCustomerEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("customer_code")]
    [MaxLength(50)]
    public string CustomerCode { get; set; } = string.Empty;

    [Column("customer_name")]
    [MaxLength(255)]
    public string CustomerName { get; set; } = string.Empty;

    [Column("arabic_description")]
    [MaxLength(500)]
    public string? ArabicDescription { get; set; }

    [Column("parent_customer_code")]
    [MaxLength(50)]
    public string? ParentCustomerCode { get; set; }

    [Column("parent_customer_name")]
    [MaxLength(255)]
    public string? ParentCustomerName { get; set; }

    [Column("contact_no")]
    [MaxLength(50)]
    public string? ContactNo { get; set; }

    [Column("fax")]
    [MaxLength(50)]
    public string? Fax { get; set; }

    [Column("email")]
    [MaxLength(255)]
    public string? Email { get; set; }

    [Column("address1")]
    [MaxLength(255)]
    public string? Address1 { get; set; }

    [Column("address2")]
    [MaxLength(255)]
    public string? Address2 { get; set; }

    [Column("address3")]
    [MaxLength(255)]
    public string? Address3 { get; set; }

    [Column("address4")]
    [MaxLength(255)]
    public string? Address4 { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("longitude")]
    public double? Longitude { get; set; }

    [Column("latitude")]
    public double? Latitude { get; set; }

    [Column("contact_person_name")]
    [MaxLength(255)]
    public string? ContactPersonName { get; set; }

    // Foreign Keys to Master Data
    [Column("region_id")]
    public int? RegionId { get; set; }

    [Column("city_id")]
    public int? CityId { get; set; }

    [Column("payment_term_id")]
    public int? PaymentTermId { get; set; }

    [Column("channel_id")]
    public int? ChannelId { get; set; }

    [Column("price_list_code")]
    [MaxLength(50)]
    public string? PriceListCode { get; set; }

    [Column("customer_group_code")]
    [MaxLength(50)]
    public string? CustomerGroupCode { get; set; }

    [Column("customer_group_name")]
    [MaxLength(255)]
    public string? CustomerGroupName { get; set; }

    [Column("credit_limit")]
    [Precision(18, 2)]
    public decimal? CreditLimit { get; set; }

    [Column("credit_days")]
    public int? CreditDays { get; set; }

    [Column("customer_type")]
    [MaxLength(50)]
    public string? CustomerType { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("reference_id")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; }

    // Navigation Properties
    [ForeignKey("RegionId")]
    public MRegionEntity? Region { get; set; }

    [ForeignKey("CityId")]
    public MCityEntity? City { get; set; }

    [ForeignKey("PaymentTermId")]
    public MPaymentTermEntity? PaymentTerm { get; set; }

    [ForeignKey("ChannelId")]
    public MChannelEntity? Channel { get; set; }
}
