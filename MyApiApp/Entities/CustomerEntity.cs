using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MyApiApp.Entities;

[Table("customers")]
public class CustomerEntity
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

    [Column("city_code")]
    [MaxLength(50)]
    public string? CityCode { get; set; }

    [Column("city_name")]
    [MaxLength(100)]
    public string? CityName { get; set; }

    [Column("city_name_arabic")]
    [MaxLength(100)]
    public string? CityNameArabic { get; set; }

    [Column("region_code")]
    [MaxLength(50)]
    public string? RegionCode { get; set; }

    [Column("region_name")]
    [MaxLength(100)]
    public string? RegionName { get; set; }

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

    [Column("payment_term_name")]
    [MaxLength(100)]
    public string? PaymentTermName { get; set; }

    [Column("payment_term_code")]
    [MaxLength(50)]
    public string? PaymentTermCode { get; set; }

    [Column("customer_type")]
    [MaxLength(50)]
    public string? CustomerType { get; set; }

    [Column("channel_code")]
    [MaxLength(50)]
    public string? ChannelCode { get; set; }

    [Column("channel_name")]
    [MaxLength(255)]
    public string? ChannelName { get; set; }

    [Column("sub_channel_code")]
    [MaxLength(50)]
    public string? SubChannelCode { get; set; }

    [Column("sub_channel_name")]
    [MaxLength(255)]
    public string? SubChannelName { get; set; }

    [Column("is_blocked")]
    public bool IsBlocked { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("reference_id")]
    [MaxLength(100)]
    public string? ReferenceId { get; set; }
}
