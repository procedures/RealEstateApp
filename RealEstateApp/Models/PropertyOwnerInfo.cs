namespace RealEstateApp.Models;

// ҮХХ-ийн эзэмшигч болон (байгаа бол) итгэмжлэгдсэн төлөөлөгчийн мэдээлэл.
// Зөвхөн админ/агент харна — нийтэд ил гарахгүй.
public class PropertyOwnerInfo
{
    public string? OwnerLastName { get; set; }
    public string? OwnerFirstName { get; set; }
    public string? OwnerRegisterNumber { get; set; }

    public bool HasAuthorizedRepresentative { get; set; }
    public string? RepresentativeLastName { get; set; }
    public string? RepresentativeFirstName { get; set; }
    public string? RepresentativeRegisterNumber { get; set; }
}
