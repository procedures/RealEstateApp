using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface ISiteRepository
{
    Task<SiteSettingsBag> GetSettingsAsync();
    Task<ProcResult> SaveSettingAsync(string key, string? value, int adminAgentId);

    Task<SitePage?> GetPageAsync(string pageKey);
    Task<ProcResult> SavePageAsync(SitePage page, int adminAgentId);

    Task<List<SiteBlock>> ListBlocksAsync(string pageKey, bool activeOnly = true);
    Task<ProcResult> SaveBlockAsync(SiteBlock block, int adminAgentId);
    Task<ProcResult> DeleteBlockAsync(int blockId, int adminAgentId);

    Task<ProcResult> SendMessageAsync(ContactInput input);
    Task<PagedResult<ContactMessage>> ListMessagesAsync(bool unreadOnly, int pageNumber, int pageSize);
    Task<ProcResult> MarkReadAsync(int messageId, int adminAgentId);
    Task<ProcResult> DeleteMessageAsync(int messageId, int adminAgentId);
}

public class SiteRepository : ISiteRepository
{
    private readonly IDbConnectionFactory _factory;
    public SiteRepository(IDbConnectionFactory factory) => _factory = factory;

    private static DynamicParameters WithOutputs(DynamicParameters p)
    {
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
        return p;
    }

    private static ProcResult ReadResult(DynamicParameters p) =>
        new(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");

    public async Task<SiteSettingsBag> GetSettingsAsync()
    {
        using var db = _factory.Create();
        var rows = await db.QueryAsync<SiteSetting>(
            "dbo.usp_SiteSettings_GetAll", commandType: CommandType.StoredProcedure);
        return new SiteSettingsBag(rows);
    }

    public async Task<ProcResult> SaveSettingAsync(string key, string? value, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@SettingKey", key);
        p.Add("@SettingValue", value);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_SiteSetting_Save", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<SitePage?> GetPageAsync(string pageKey)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@PageKey", pageKey);
        return await db.QueryFirstOrDefaultAsync<SitePage>(
            "dbo.usp_SitePage_Get", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<ProcResult> SavePageAsync(SitePage page, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@PageKey", page.PageKey);
        p.Add("@Title", page.Title);
        p.Add("@Subtitle", page.Subtitle);
        p.Add("@Content", page.Content);
        p.Add("@ImageUrl", page.ImageUrl);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_SitePage_Save", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<List<SiteBlock>> ListBlocksAsync(string pageKey, bool activeOnly = true)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@PageKey", pageKey);
        p.Add("@ActiveOnly", activeOnly);

        var rows = await db.QueryAsync<SiteBlock>(
            "dbo.usp_SiteBlock_List", p, commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> SaveBlockAsync(SiteBlock b, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@BlockId", b.BlockId == 0 ? null : b.BlockId);
        p.Add("@PageKey", b.PageKey);
        p.Add("@BlockType", b.BlockType);
        p.Add("@Icon", b.Icon);
        p.Add("@Title", b.Title);
        p.Add("@Body", b.Body);
        p.Add("@SortOrder", b.SortOrder);
        p.Add("@IsActive", b.IsActive);
        p.Add("@AdminAgentId", adminAgentId);
        p.Add("@NewBlockId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_SiteBlock_Save", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<ProcResult> DeleteBlockAsync(int blockId, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@BlockId", blockId);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_SiteBlock_Delete", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<ProcResult> SendMessageAsync(ContactInput i)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@FullName", i.FullName);
        p.Add("@Phone", i.Phone);
        p.Add("@Email", i.Email);
        p.Add("@Subject", i.Subject);
        p.Add("@Message", i.Message);
        p.Add("@NewMessageId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_ContactMessage_Insert", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<PagedResult<ContactMessage>> ListMessagesAsync(bool unreadOnly, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@UnreadOnly", unreadOnly);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<ContactMessage>(
            "dbo.usp_ContactMessage_List", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<ContactMessage>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<ProcResult> MarkReadAsync(int messageId, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@MessageId", messageId);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_ContactMessage_MarkRead", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<ProcResult> DeleteMessageAsync(int messageId, int adminAgentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@MessageId", messageId);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_ContactMessage_Delete", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }
}