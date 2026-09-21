namespace RealEstateApp.Models;

public class PropertyDetailDto
{
    public int PropertyId { get; set; }
    public string? ContractNumber { get; set; }
    public DateTime? ContractDate { get; set; }
    public int TransactionTypeId { get; set; }
    public string TransactionTypeName { get; set; } = "";
    public int PropertyTypeId { get; set; }
    public string PropertyTypeName { get; set; } = "";
    public int DistrictId { get; set; }
    public string DistrictName { get; set; } = "";
    public string DistrictCode { get; set; } = "";
    public string? LocationDescription { get; set; }
    public string? ComplexName { get; set; }
    public string? Title { get; set; }
    public string Address { get; set; } = "";
    public decimal Price { get; set; }
    public decimal AreaSize { get; set; }
    public decimal? PricePerSqm { get; set; }
    public int? RoomCount { get; set; }
    public int? CommissionedYear { get; set; }
    public int? BuildingFloors { get; set; }
    public int? FloorNumber { get; set; }
    public int? WindowDirectionId { get; set; }
    public string? WindowDirectionName { get; set; }
    public bool HasGarage { get; set; }
    public string? ContactPhone { get; set; }
    public bool OwnerSelfSelling { get; set; }
    public int AgentId { get; set; }
    public string AgentName { get; set; } = "";
    public string? AgentPhone { get; set; }
    public int Status { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ContractClosedDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? AgentPhotoUrl { get; set; }
}