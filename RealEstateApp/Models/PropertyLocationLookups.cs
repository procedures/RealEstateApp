namespace RealEstateApp.Models;

// Хотхойн нэрээр бүлэглэсэн статистик (санал болгох + дундаж үнэ харуулахад ашиглана)
public class ComplexNameStat
{
    public string ComplexName { get; set; } = "";
    public int PropertyCount { get; set; }
    public decimal? AvgPrice { get; set; }
    public decimal? AvgPricePerSqm { get; set; }
}

// "2. Байршил" хэсгийн Хотхоны нэр / Байршил талбаруудад
// өмнө нь бүртгэгдсэн утгуудыг санал болгоход ашиглах өгөгдөл
public class PropertyLocationLookups
{
    public List<ComplexNameStat> ComplexNames { get; set; } = new();
    public List<string> LocationDescriptions { get; set; } = new();
}
