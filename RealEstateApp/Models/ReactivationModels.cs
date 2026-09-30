using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models;

// Агент "Хаагдсан" зарыг дахин идэвхижүүлэх хүсэлт илгээхэд бөглөх маягт
public class ReactivationInput
{
    [Required(ErrorMessage = "Шалтгаанаа бичнэ үү")]
    public string? Reason { get; set; }

    // Гэрээний хугацаа дууссан тохиолдолд сунгах шинэ огноо (заавал биш)
    public DateTime? NewContractEndDate { get; set; }
}

// Админд ирсэн хүлээгдэж буй идэвхижүүлэх хүсэлт
public class PendingReactivation
{
    public int RequestId { get; set; }
    public int PropertyId { get; set; }
    public string Address { get; set; } = "";
    public string? ComplexName { get; set; }
    public string DistrictName { get; set; } = "";
    public decimal ListedPrice { get; set; }
    public decimal AreaSize { get; set; }

    public string? ClosureReasonName { get; set; }
    public DateTime? ClosureDate { get; set; }

    public string Reason { get; set; } = "";
    public DateTime? NewContractEndDate { get; set; }

    public string SubmittedByAgent { get; set; } = "";
    public DateTime SubmittedAt { get; set; }
}
