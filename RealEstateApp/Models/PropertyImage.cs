namespace RealEstateApp.Models;

public class PropertyImage
{
    public int ImageId { get; set; }
    public int PropertyId { get; set; }
    public string ImagePath { get; set; } = "";
    public string? PublicId { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}