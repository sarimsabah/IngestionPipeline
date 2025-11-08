namespace MyApiApp.Models;

public class CustomerRequest
{
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? ArabicDescription { get; set; }
    public string? ParentCustomerCode { get; set; }
    public string? ParentCustomerName { get; set; }
    public string? ContactNo { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public bool? IsActive { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? ContactPersonName { get; set; }
    public string? CityCode { get; set; }
    public string? CityName { get; set; }
    public string? CityNameArabic { get; set; }
    public string? RegionCode { get; set; }
    public string? RegionName { get; set; }
    public string? PriceListCode { get; set; }
    public string? CustomerGroupCode { get; set; }
    public string? CustomerGroupName { get; set; }
    public decimal? CreditLimit { get; set; }
    public int? CreditDays { get; set; }
    public string? PaymentTermName { get; set; }
    public string? PaymentTermCode { get; set; }
    public string? CustomerType { get; set; }
    public string? ChannelCode { get; set; }
    public string? ChannelName { get; set; }
    public string? SubChannelCode { get; set; }
    public string? SubChannelName { get; set; }
    public bool? IsBlocked { get; set; }
}
