namespace RealEstateApp.Models;

public class District { public int DistrictId { get; set; } public string Code = ""; public string Name = ""; }
public class PropertyType { public int PropertyTypeId { get; set; } public string Name = ""; }
public class TransactionType { public int TransactionTypeId { get; set; } public string Name = ""; }
public class WindowDirection { public int WindowDirectionId { get; set; } public string Name = ""; }
public class ClosureReason { public int ClosureReasonId { get; set; } public string Name = ""; public bool IsSold { get; set; } public bool AllowsPriceEntry { get; set; } }
public class Branch { public int BranchId { get; set; } public string Name = ""; }

// Бүх dropdown-г нэг дор хадгалах сав
public class LookupData
{
    public List<District> Districts { get; set; } = new();
    public List<PropertyType> PropertyTypes { get; set; } = new();
    public List<TransactionType> TransactionTypes { get; set; } = new();
    public List<WindowDirection> WindowDirections { get; set; } = new();
    public List<ClosureReason> ClosureReasons { get; set; } = new();
    public List<Branch> Branches { get; set; } = new();
}