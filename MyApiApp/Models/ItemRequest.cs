namespace MyApiApp.Models;

public class ItemRequest
{
    public MaterialData? Material { get; set; }
}

public class MaterialData
{
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? ArabicDescription { get; set; }
    public string? SalesOrgCode { get; set; }
    public string? BaseUOM { get; set; }
    public string? BrandCode { get; set; }
    public string? BrandName { get; set; }
    public string? CategoryCode { get; set; }
    public string? CategoryName { get; set; }
    public string? IsActive { get; set; }
    public bool? IsBatchEnabled { get; set; }
    public string? BusinessType { get; set; }
    public string? BusinessTypeDescription { get; set; }
    public List<UomData>? UomList { get; set; }
}

public class UomData
{
    public string? Uom { get; set; }
    public int? ConversionFactor { get; set; }
}
