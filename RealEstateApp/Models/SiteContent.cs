namespace RealEstateApp.Models;

public class SiteSetting
{
    public string SettingKey { get; set; } = "";
    public string? SettingValue { get; set; }
    public string Label { get; set; } = "";
    public string Category { get; set; } = "";
    public string InputType { get; set; } = "text";
    public int SortOrder { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SitePage
{
    public string PageKey { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Subtitle { get; set; }
    public string? Content { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Агуулгыг догол мөрөөр салгах
    public IEnumerable<string> Paragraphs =>
        (Content ?? "")
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0);
}

public class SiteBlock
{
    public int BlockId { get; set; }
    public string PageKey { get; set; } = "";
    public string BlockType { get; set; } = "feature";
    public string? Icon { get; set; }
    public string Title { get; set; } = "";
    public string? Body { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ContactInput
{
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = "";
}

public class ContactMessage
{
    public int MessageId { get; set; }
    public string FullName { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public int UnreadCount { get; set; }
    public int TotalCount { get; set; }
}

// Тохиргоог түлхүүрээр хялбар авах сав
public class SiteSettingsBag
{
    private readonly Dictionary<string, string?> _map;
    public IReadOnlyList<SiteSetting> All { get; }

    public SiteSettingsBag(IEnumerable<SiteSetting> items)
    {
        All = items.ToList();
        _map = All.ToDictionary(x => x.SettingKey, x => x.SettingValue);
    }

    public string this[string key] =>
        _map.TryGetValue(key, out var v) && !string.IsNullOrWhiteSpace(v) ? v! : "";

    public bool Has(string key) => !string.IsNullOrWhiteSpace(this[key]);

    public IEnumerable<string> Lines(string key) =>
        this[key].Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(x => x.Trim()).Where(x => x.Length > 0);
}