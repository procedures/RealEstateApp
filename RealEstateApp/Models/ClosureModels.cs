using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models;

// Агент хаалт хүсэхэд бөглөх маягт
public class ClosureInput
{
    [Required(ErrorMessage = "Хаалтын шалтгаанаа сонгоно уу")]
    public int? ClosureReasonId { get; set; }

    [Required(ErrorMessage = "Хаалт хийсэн огноог оруулна уу")]
    public DateTime? ClosureDate { get; set; } = DateTime.Today;

    public decimal? SoldTotalPrice { get; set; }
    public decimal? SoldPricePerSqm { get; set; }

    // Худалдан авагч
    public string? BuyerLastName { get; set; }
    public string? BuyerFirstName { get; set; }
    public string? BuyerRegNo { get; set; }
    public string? BuyerPhone { get; set; }

    // Төлбөрийн нөхцөл: 1=Бэлэн төлбөр, 2=Энгийн ОС зээл, 3=Ипотекийн зээл
    public int? PaymentMethod { get; set; }

    // Итгэмжлэлээр төлөөлж буй эсэх
    public bool HasBuyerRepresentative { get; set; }
    public string? BuyerRepLastName { get; set; }
    public string? BuyerRepFirstName { get; set; }
    public string? BuyerRepRegNo { get; set; }

    // Хамтран борлуулсан агент
    public int? CoAgentBranchId { get; set; }
    public string? CoAgentLastName { get; set; }
    public string? CoAgentFirstName { get; set; }
    public string? CoAgentPhone { get; set; }
}

// Зөвхөн энэ хаалттай холбоотой, хожим нэмэгдсэн нэмэлт мэдээлэл
// (Set/Get-ийг тусад нь дуудахад ашиглана — dbo.usp_PropertyClosure_Request-д хүрэлгүйгээр)
public class ClosureExtraInfo
{
    public int? PaymentMethod { get; set; }
    public bool HasBuyerRepresentative { get; set; }
    public string? BuyerRepLastName { get; set; }
    public string? BuyerRepFirstName { get; set; }
    public string? BuyerRepRegNo { get; set; }
}

public static class PaymentMethods
{
    public const int Cash = 1;
    public const int RegularLoan = 2;
    public const int Mortgage = 3;

    public static string Name(int? code) => code switch
    {
        Cash => "Бэлэн төлбөр",
        RegularLoan => "Энгийн ОС зээл",
        Mortgage => "Ипотекийн зээл",
        _ => "-"
    };
}

// Хаалтын бичлэгийн дэлгэрэнгүй
public class PropertyClosureDto
{
    public int ClosureId { get; set; }
    public int PropertyId { get; set; }
    public int ClosureReasonId { get; set; }
    public string ClosureReasonName { get; set; } = "";
    public DateTime ClosureDate { get; set; }
    public decimal? SoldTotalPrice { get; set; }
    public decimal? SoldPricePerSqm { get; set; }

    public string? BuyerLastName { get; set; }
    public string? BuyerFirstName { get; set; }
    public string? BuyerRegNo { get; set; }
    public string? BuyerPhone { get; set; }

    public int? CoAgentBranchId { get; set; }
    public string? CoAgentBranchName { get; set; }
    public string? CoAgentLastName { get; set; }
    public string? CoAgentFirstName { get; set; }
    public string? CoAgentPhone { get; set; }

    public int ApprovalStatus { get; set; }
    public int SubmittedByAgentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int? ApprovedByAgentId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? AdminNote { get; set; }
}

// Админд ирсэн хүлээгдэж буй хүсэлт
public class PendingClosure
{
    public int ClosureId { get; set; }
    public int PropertyId { get; set; }
    public string Address { get; set; } = "";
    public string? ComplexName { get; set; }
    public string DistrictName { get; set; } = "";
    public decimal ListedPrice { get; set; }
    public decimal AreaSize { get; set; }

    public string ClosureReasonName { get; set; } = "";
    public bool IsSold { get; set; }
    public DateTime ClosureDate { get; set; }
    public decimal? SoldTotalPrice { get; set; }
    public decimal? SoldPricePerSqm { get; set; }

    public string? BuyerName { get; set; }
    public string? BuyerPhone { get; set; }
    public bool HasCoAgent { get; set; }

    public string SubmittedByAgent { get; set; } = "";
    public DateTime SubmittedAt { get; set; }

    public bool HasMissingInfo { get; set; }
}

// Админ баталгаажуулахад харах бүрэн мэдээлэл
public class ClosureDetailDto
{
    public int ClosureId { get; set; }
    public int PropertyId { get; set; }
    public int ClosureReasonId { get; set; }
    public string ClosureReasonName { get; set; } = "";
    public bool IsSold { get; set; }
    public DateTime ClosureDate { get; set; }
    public decimal? SoldTotalPrice { get; set; }
    public decimal? SoldPricePerSqm { get; set; }

    // Худалдан авагч
    public string? BuyerLastName { get; set; }
    public string? BuyerFirstName { get; set; }
    public string? BuyerRegNo { get; set; }
    public string? BuyerPhone { get; set; }

    // Төлбөрийн нөхцөл ба итгэмжлэл (зөвхөн админ + бүртгэсэн агент харна)
    public int? PaymentMethod { get; set; }
    public bool HasBuyerRepresentative { get; set; }
    public string? BuyerRepLastName { get; set; }
    public string? BuyerRepFirstName { get; set; }
    public string? BuyerRepRegNo { get; set; }

    // Хамтран борлуулсан агент
    public int? CoAgentBranchId { get; set; }
    public string? CoAgentBranchName { get; set; }
    public string? CoAgentLastName { get; set; }
    public string? CoAgentFirstName { get; set; }
    public string? CoAgentPhone { get; set; }

    // Урсгал
    public int ApprovalStatus { get; set; }
    public int SubmittedByAgentId { get; set; }
    public string SubmittedByAgentName { get; set; } = "";
    public string? SubmittedByAgentPhone { get; set; }
    public DateTime SubmittedAt { get; set; }
    public int? ApprovedByAgentId { get; set; }
    public string? ApprovedByAgentName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? AdminNote { get; set; }

    // Хөрөнгө
    public string? ContractNumber { get; set; }
    public DateTime? ContractDate { get; set; }
    public string Address { get; set; } = "";
    public string? ComplexName { get; set; }
    public string DistrictName { get; set; } = "";
    public string PropertyTypeName { get; set; } = "";
    public string TransactionTypeName { get; set; } = "";
    public decimal ListedPrice { get; set; }
    public decimal AreaSize { get; set; }
    public decimal? ListedPricePerSqm { get; set; }
    public int? RoomCount { get; set; }
    public int PropertyStatus { get; set; }
    public string PropertyAgentName { get; set; } = "";

    // Тооцоо
    public decimal? PriceDiff { get; set; }
    public decimal? PriceDiffPercent { get; set; }
    public int ImageCount { get; set; }

    public string BuyerFullName => $"{BuyerLastName} {BuyerFirstName}".Trim();
    public string BuyerRepFullName => $"{BuyerRepLastName} {BuyerRepFirstName}".Trim();
    public string PaymentMethodName => PaymentMethods.Name(PaymentMethod);
    public string CoAgentFullName => $"{CoAgentLastName} {CoAgentFirstName}".Trim();
    public bool HasCoAgentInfo =>
        !string.IsNullOrWhiteSpace(CoAgentFullName) || CoAgentBranchId.HasValue;
}