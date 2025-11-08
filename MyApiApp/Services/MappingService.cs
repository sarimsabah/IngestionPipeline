using MyApiApp.Models;
using MyApiApp.Entities;

namespace MyApiApp.Services;

public static class MappingService
{
    public static CustomerEntity ToEntity(CustomerRequest request, string referenceId)
    {
        return new CustomerEntity
        {
            CustomerCode = request.CustomerCode ?? string.Empty,
            CustomerName = request.CustomerName ?? string.Empty,
            ArabicDescription = request.ArabicDescription,
            ParentCustomerCode = request.ParentCustomerCode,
            ParentCustomerName = request.ParentCustomerName,
            ContactNo = request.ContactNo,
            Fax = request.Fax,
            Email = request.Email,
            Address1 = request.Address1,
            Address2 = request.Address2,
            Address3 = request.Address3,
            Address4 = request.Address4,
            IsActive = request.IsActive ?? false,
            Longitude = request.Longitude,
            Latitude = request.Latitude,
            ContactPersonName = request.ContactPersonName,
            CityCode = request.CityCode,
            CityName = request.CityName,
            CityNameArabic = request.CityNameArabic,
            RegionCode = request.RegionCode,
            RegionName = request.RegionName,
            PriceListCode = request.PriceListCode,
            CustomerGroupCode = request.CustomerGroupCode,
            CustomerGroupName = request.CustomerGroupName,
            CreditLimit = request.CreditLimit,
            CreditDays = request.CreditDays,
            PaymentTermName = request.PaymentTermName,
            PaymentTermCode = request.PaymentTermCode,
            CustomerType = request.CustomerType,
            ChannelCode = request.ChannelCode,
            ChannelName = request.ChannelName,
            SubChannelCode = request.SubChannelCode,
            SubChannelName = request.SubChannelName,
            IsBlocked = request.IsBlocked ?? false,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ItemEntity ToEntity(ItemRequest request, string referenceId)
    {
        var item = new ItemEntity
        {
            ItemCode = request.Material?.ItemCode ?? string.Empty,
            ItemName = request.Material?.ItemName ?? string.Empty,
            ArabicDescription = request.Material?.ArabicDescription,
            SalesOrgCode = request.Material?.SalesOrgCode,
            BaseUOM = request.Material?.BaseUOM,
            BrandCode = request.Material?.BrandCode,
            BrandName = request.Material?.BrandName,
            CategoryCode = request.Material?.CategoryCode,
            CategoryName = request.Material?.CategoryName,
            IsActive = request.Material?.IsActive,
            IsBatchEnabled = request.Material?.IsBatchEnabled ?? false,
            BusinessType = request.Material?.BusinessType,
            BusinessTypeDescription = request.Material?.BusinessTypeDescription,
            ReferenceId = referenceId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Map UOM list
        if (request.Material?.UomList != null)
        {
            foreach (var uom in request.Material.UomList)
            {
                item.UomList.Add(new UomEntity
                {
                    Uom = uom.Uom,
                    ConversionFactor = uom.ConversionFactor ?? 1
                });
            }
        }

        return item;
    }

    public static void UpdateEntity(CustomerEntity entity, CustomerRequest request)
    {
        entity.CustomerName = request.CustomerName ?? entity.CustomerName;
        entity.ArabicDescription = request.ArabicDescription ?? entity.ArabicDescription;
        entity.ParentCustomerCode = request.ParentCustomerCode ?? entity.ParentCustomerCode;
        entity.ParentCustomerName = request.ParentCustomerName ?? entity.ParentCustomerName;
        entity.ContactNo = request.ContactNo ?? entity.ContactNo;
        entity.Fax = request.Fax ?? entity.Fax;
        entity.Email = request.Email ?? entity.Email;
        entity.Address1 = request.Address1 ?? entity.Address1;
        entity.Address2 = request.Address2 ?? entity.Address2;
        entity.Address3 = request.Address3 ?? entity.Address3;
        entity.Address4 = request.Address4 ?? entity.Address4;
        entity.IsActive = request.IsActive ?? entity.IsActive;
        entity.Longitude = request.Longitude ?? entity.Longitude;
        entity.Latitude = request.Latitude ?? entity.Latitude;
        entity.ContactPersonName = request.ContactPersonName ?? entity.ContactPersonName;
        entity.CityCode = request.CityCode ?? entity.CityCode;
        entity.CityName = request.CityName ?? entity.CityName;
        entity.CityNameArabic = request.CityNameArabic ?? entity.CityNameArabic;
        entity.RegionCode = request.RegionCode ?? entity.RegionCode;
        entity.RegionName = request.RegionName ?? entity.RegionName;
        entity.PriceListCode = request.PriceListCode ?? entity.PriceListCode;
        entity.CustomerGroupCode = request.CustomerGroupCode ?? entity.CustomerGroupCode;
        entity.CustomerGroupName = request.CustomerGroupName ?? entity.CustomerGroupName;
        entity.CreditLimit = request.CreditLimit ?? entity.CreditLimit;
        entity.CreditDays = request.CreditDays ?? entity.CreditDays;
        entity.PaymentTermName = request.PaymentTermName ?? entity.PaymentTermName;
        entity.PaymentTermCode = request.PaymentTermCode ?? entity.PaymentTermCode;
        entity.CustomerType = request.CustomerType ?? entity.CustomerType;
        entity.ChannelCode = request.ChannelCode ?? entity.ChannelCode;
        entity.ChannelName = request.ChannelName ?? entity.ChannelName;
        entity.SubChannelCode = request.SubChannelCode ?? entity.SubChannelCode;
        entity.SubChannelName = request.SubChannelName ?? entity.SubChannelName;
        entity.IsBlocked = request.IsBlocked ?? entity.IsBlocked;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
