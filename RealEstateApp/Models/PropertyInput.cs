using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Models;

public class PropertyInput
{
    public string? ContractNumber { get; set; }
    public DateTime? ContractDate { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Гүйлгээний төрлөө сонгоно уу")]
    public int? TransactionTypeId { get; set; }

    [Required(ErrorMessage = "Хөрөнгийн төрлөө сонгоно уу")]
    public int? PropertyTypeId { get; set; }

    [Required(ErrorMessage = "Дүүргээ сонгоно уу")]
    public int? DistrictId { get; set; }

    public string? LocationDescription { get; set; }
    public string? ComplexName { get; set; }

    [StringLength(200, ErrorMessage = "Гарчиг 200 тэмдэгтээс хэтрэхгүй байх ёстой")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Хаягаа оруулна уу")]
    public string Address { get; set; } = "";

    [Required(ErrorMessage = "Үнээ оруулна уу")]
    [Range(1, 999999999999, ErrorMessage = "Үнэ 0-ээс их байх ёстой")]
    public decimal? Price { get; set; }

    [Required(ErrorMessage = "Талбайн хэмжээгээ оруулна уу")]
    [Range(0.01, 99999, ErrorMessage = "Талбай 0-ээс их байх ёстой")]
    public decimal? AreaSize { get; set; }

    public int? RoomCount { get; set; }
    public int? CommissionedYear { get; set; }
    public int? BuildingFloors { get; set; }
    public int? FloorNumber { get; set; }
    public int? WindowDirectionId { get; set; }
    public bool HasGarage { get; set; }
    public string? ContactPhone { get; set; }
    public bool OwnerSelfSelling { get; set; }

    // ҮХХ-ийн эзэмшигчийн мэдээлэл
    [Required(ErrorMessage = "Эзэмшигчийн овгийг оруулна уу")]
    public string? OwnerLastName { get; set; }

    [Required(ErrorMessage = "Эзэмшигчийн нэрийг оруулна уу")]
    public string? OwnerFirstName { get; set; }

    [Required(ErrorMessage = "Эзэмшигчийн РД-г оруулна уу")]
    [RegularExpression(@"^[А-Яа-яӨөҮүЁё]{2}\d{8}$", ErrorMessage = "РД буруу форматтай байна (жишээ: УБ12345678)")]
    public string? OwnerRegisterNumber { get; set; }

    public bool HasAuthorizedRepresentative { get; set; }
    public string? RepresentativeLastName { get; set; }
    public string? RepresentativeFirstName { get; set; }

    [RegularExpression(@"^[А-Яа-яӨөҮүЁё]{2}\d{8}$", ErrorMessage = "РД буруу форматтай байна (жишээ: УБ12345678)")]
    public string? RepresentativeRegisterNumber { get; set; }

    public static PropertyInput FromDetail(PropertyDetailDto d) => new()
    {
        ContractNumber = d.ContractNumber,
        ContractDate = d.ContractDate,
        TransactionTypeId = d.TransactionTypeId,
        PropertyTypeId = d.PropertyTypeId,
        DistrictId = d.DistrictId,
        LocationDescription = d.LocationDescription,
        ComplexName = d.ComplexName,
        Title = d.Title,
        Address = d.Address,
        Price = d.Price,
        AreaSize = d.AreaSize,
        RoomCount = d.RoomCount,
        CommissionedYear = d.CommissionedYear,
        BuildingFloors = d.BuildingFloors,
        FloorNumber = d.FloorNumber,
        WindowDirectionId = d.WindowDirectionId,
        HasGarage = d.HasGarage,
        ContactPhone = d.ContactPhone,
        OwnerSelfSelling = d.OwnerSelfSelling,
        OwnerLastName = d.OwnerLastName,
        OwnerFirstName = d.OwnerFirstName,
        OwnerRegisterNumber = d.OwnerRegisterNumber,
        HasAuthorizedRepresentative = d.HasAuthorizedRepresentative,
        RepresentativeLastName = d.RepresentativeLastName,
        RepresentativeFirstName = d.RepresentativeFirstName,
        RepresentativeRegisterNumber = d.RepresentativeRegisterNumber,
    };
}

