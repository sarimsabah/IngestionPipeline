using MyApiApp.Models;

namespace MyApiApp.Services;

public static class ValidationService
{
    public static List<ValidationError> ValidateCustomer(CustomerRequest request)
    {
        var errors = new List<ValidationError>();

        // Mandatory Presence Validations
        if (string.IsNullOrWhiteSpace(request.CustomerCode))
            errors.Add(new ValidationError { Field = "customerCode", Reason = "Customer code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(request.CustomerName))
            errors.Add(new ValidationError { Field = "customerName", Reason = "Customer name is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(request.ContactNo))
            errors.Add(new ValidationError { Field = "contactNo", Reason = "Contact number is mandatory and cannot be empty." });

        // Email Format Validation
        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add(new ValidationError { Field = "email", Reason = "Email is mandatory and cannot be empty." });
        else if (!IsValidEmail(request.Email))
            errors.Add(new ValidationError { Field = "email", Reason = "Email format is invalid." });

        // City Validations
        if (string.IsNullOrWhiteSpace(request.CityCode))
            errors.Add(new ValidationError { Field = "cityCode", Reason = "City code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(request.CityName))
            errors.Add(new ValidationError { Field = "cityName", Reason = "City name is mandatory and cannot be empty." });

        // Region Validations
        if (string.IsNullOrWhiteSpace(request.RegionCode))
            errors.Add(new ValidationError { Field = "regionCode", Reason = "Region code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(request.RegionName))
            errors.Add(new ValidationError { Field = "regionName", Reason = "Region name is mandatory and cannot be empty." });

        // Customer Type Validation
        if (string.IsNullOrWhiteSpace(request.CustomerType))
            errors.Add(new ValidationError { Field = "customerType", Reason = "Customer type is mandatory and cannot be empty." });

        // Boolean Data Type Validations
        if (!request.IsActive.HasValue)
            errors.Add(new ValidationError { Field = "isActive", Reason = "isActive is mandatory and must be a boolean (true/false)." });

        if (!request.IsBlocked.HasValue)
            errors.Add(new ValidationError { Field = "isBlocked", Reason = "isBlocked is mandatory and must be a boolean (true/false)." });

        // Numeric Data Type Validations
        if (!request.CreditLimit.HasValue)
            errors.Add(new ValidationError { Field = "creditLimit", Reason = "Credit limit is mandatory and must be numeric." });

        if (!request.CreditDays.HasValue)
            errors.Add(new ValidationError { Field = "creditDays", Reason = "Credit days is mandatory and must be numeric." });

        // Business Rule: Credit Logic
        if (!string.IsNullOrWhiteSpace(request.CustomerType) &&
            request.CustomerType.Trim().ToUpper() != "CASH")
        {
            if (request.CreditLimit.HasValue && request.CreditLimit.Value <= 0)
                errors.Add(new ValidationError { Field = "creditLimit", Reason = "For non-CASH customer types, credit limit must be greater than 0." });

            if (request.CreditDays.HasValue && request.CreditDays.Value <= 0)
                errors.Add(new ValidationError { Field = "creditDays", Reason = "For non-CASH customer types, credit days must be greater than 0." });
        }

        return errors;
    }

    public static List<ValidationError> ValidateItem(ItemRequest request)
    {
        var errors = new List<ValidationError>();

        if (request.Material == null)
        {
            errors.Add(new ValidationError { Field = "material", Reason = "Material data is required." });
            return errors;
        }

        var material = request.Material;

        // Mandatory Presence Validations
        if (string.IsNullOrWhiteSpace(material.ItemCode))
            errors.Add(new ValidationError { Field = "material.itemCode", Reason = "Item code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(material.ItemName))
            errors.Add(new ValidationError { Field = "material.itemName", Reason = "Item name is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(material.SalesOrgCode))
            errors.Add(new ValidationError { Field = "material.salesOrgCode", Reason = "Sales org code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(material.BaseUOM))
            errors.Add(new ValidationError { Field = "material.baseUOM", Reason = "Base UOM is mandatory and cannot be empty." });

        // Brand Validations
        if (string.IsNullOrWhiteSpace(material.BrandCode))
            errors.Add(new ValidationError { Field = "material.brandCode", Reason = "Brand code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(material.BrandName))
            errors.Add(new ValidationError { Field = "material.brandName", Reason = "Brand name is mandatory and cannot be empty." });

        // Category Validations
        if (string.IsNullOrWhiteSpace(material.CategoryCode))
            errors.Add(new ValidationError { Field = "material.categoryCode", Reason = "Category code is mandatory and cannot be empty." });

        if (string.IsNullOrWhiteSpace(material.CategoryName))
            errors.Add(new ValidationError { Field = "material.categoryName", Reason = "Category name is mandatory and cannot be empty." });

        // Boolean Data Type Validation
        if (!material.IsBatchEnabled.HasValue)
            errors.Add(new ValidationError { Field = "material.isBatchEnabled", Reason = "isBatchEnabled is mandatory and must be a boolean (true/false)." });

        // IsActive Value Check (must be "0" or "1")
        if (string.IsNullOrWhiteSpace(material.IsActive))
            errors.Add(new ValidationError { Field = "material.isActive", Reason = "isActive is mandatory and cannot be empty." });
        else if (material.IsActive != "0" && material.IsActive != "1")
            errors.Add(new ValidationError { Field = "material.isActive", Reason = "isActive must be either '0' or '1'." });

        // UOM List Validations
        if (material.UomList == null || !material.UomList.Any())
        {
            errors.Add(new ValidationError { Field = "material.uomList", Reason = "uomList is mandatory and must be a non-empty array." });
        }
        else
        {
            // Validate each UOM entry
            for (int i = 0; i < material.UomList.Count; i++)
            {
                var uomEntry = material.UomList[i];

                if (string.IsNullOrWhiteSpace(uomEntry.Uom))
                    errors.Add(new ValidationError { Field = $"material.uomList[{i}].uom", Reason = "UOM is mandatory and cannot be empty." });

                if (!uomEntry.ConversionFactor.HasValue)
                    errors.Add(new ValidationError { Field = $"material.uomList[{i}].conversionFactor", Reason = "Conversion factor is mandatory and must be numeric." });
                else if (uomEntry.ConversionFactor.Value <= 0)
                    errors.Add(new ValidationError { Field = $"material.uomList[{i}].conversionFactor", Reason = "Conversion factor must be greater than 0." });
            }
        }

        return errors;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
