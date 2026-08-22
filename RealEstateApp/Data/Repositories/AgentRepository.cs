using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IAgentRepository
{
    Task<AgentAccount?> GetForLoginAsync(string email);
    Task<(ProcResult Result, int NewAgentId)> InsertAsync(
        string lastName, string firstName, string? phone,
        string email, int? branchId, string passwordHash, string role = "Agent");
    Task<PagedResult<AgentPublicItem>> ListPublicAsync(string? search, int? branchId, string sortBy, int pageNumber, int pageSize);
    Task<AgentPublicDetail?> GetPublicAsync(int agentId);
    Task<PagedResult<AgentAdminItem>> ListAdminAsync(string? search, string? role, int? branchId, bool? isActive, int pageNumber, int pageSize);
    Task<AgentAdminItem?> GetAdminAsync(int agentId);
    Task<(ProcResult Result, int NewAgentId)> CreateAsync(AgentInput input, string passwordHash, int adminAgentId);
    Task<ProcResult> UpdateAsync(AgentInput input, int adminAgentId);
    Task<ProcResult> SetActiveAsync(int agentId, bool isActive, int adminAgentId);
    Task<ProcResult> ResetPasswordAsync(int agentId, string passwordHash, int adminAgentId);
    Task<List<BranchAdminItem>> ListBranchesAsync();
    Task<ProcResult> SaveBranchAsync(int branchId, string name, bool isActive, int adminAgentId);
    Task<(ProcResult Result, string? OldPublicId)> SetPhotoAsync(int agentId, string? photoUrl, string? photoPublicId, int actorAgentId);
}

public class AgentRepository : IAgentRepository
{
    private readonly IDbConnectionFactory _factory;
    public AgentRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<AgentAccount?> GetForLoginAsync(string email)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Email", email);
        return await db.QueryFirstOrDefaultAsync<AgentAccount>(
            "dbo.usp_Agent_GetForLogin", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<(ProcResult, int)> InsertAsync(
        string lastName, string firstName, string? phone,
        string email, int? branchId, string passwordHash, string role = "Agent")
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@LastName", lastName);
        p.Add("@FirstName", firstName);
        p.Add("@Phone", phone);
        p.Add("@Email", email);
        p.Add("@BranchId", branchId);
        p.Add("@PasswordHash", passwordHash);
        p.Add("@Role", role);
        p.Add("@NewAgentId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);

        await db.ExecuteAsync("dbo.usp_Agent_Insert", p, commandType: CommandType.StoredProcedure);

        var result = new ProcResult(
            p.Get<int>("@ResponseCode"),
            p.Get<string>("@ResponseMessage") ?? "");
        return (result, p.Get<int>("@NewAgentId"));
    }
    public async Task<PagedResult<AgentPublicItem>> ListPublicAsync(
    string? search, int? branchId, string sortBy, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@Search", search);
        p.Add("@BranchId", branchId);
        p.Add("@SortBy", sortBy);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<AgentPublicItem>(
            "dbo.usp_Agent_ListPublic", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<AgentPublicItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<AgentPublicDetail?> GetPublicAsync(int agentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);

        return await db.QueryFirstOrDefaultAsync<AgentPublicDetail>(
            "dbo.usp_Agent_GetPublic", p, commandType: CommandType.StoredProcedure);
    }
    private static DynamicParameters WithOutputs(DynamicParameters p)
    {
        p.Add("@ResponseCode", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@ResponseMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
        return p;
    }

    private static ProcResult ReadResult(DynamicParameters p) =>
        new(p.Get<int>("@ResponseCode"), p.Get<string>("@ResponseMessage") ?? "");

    public async Task<PagedResult<AgentAdminItem>> ListAdminAsync(
        string? search, string? role, int? branchId, bool? isActive, int pageNumber, int pageSize)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@Search", search);
        p.Add("@Role", role);
        p.Add("@BranchId", branchId);
        p.Add("@IsActive", isActive);
        p.Add("@PageNumber", pageNumber);
        p.Add("@PageSize", pageSize);

        var rows = (await db.QueryAsync<AgentAdminItem>(
            "dbo.usp_Agent_ListAdmin", p, commandType: CommandType.StoredProcedure)).ToList();

        return new PagedResult<AgentAdminItem>
        {
            Items = rows,
            Total = rows.Count > 0 ? rows[0].TotalCount : 0
        };
    }

    public async Task<AgentAdminItem?> GetAdminAsync(int agentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        return await db.QueryFirstOrDefaultAsync<AgentAdminItem>(
            "dbo.usp_Agent_GetAdmin", p, commandType: CommandType.StoredProcedure);
    }

    public async Task<(ProcResult, int)> CreateAsync(AgentInput i, string passwordHash, int adminAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@LastName", i.LastName);
        p.Add("@FirstName", i.FirstName);
        p.Add("@Phone", i.Phone);
        p.Add("@Email", i.Email);
        p.Add("@BranchId", i.BranchId);
        p.Add("@PasswordHash", passwordHash);
        p.Add("@Role", i.Role);
        p.Add("@AdminAgentId", adminAgentId);
        p.Add("@NewAgentId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Agent_Create", p, commandType: CommandType.StoredProcedure);
        return (ReadResult(p), p.Get<int>("@NewAgentId"));
    }

    public async Task<ProcResult> UpdateAsync(AgentInput i, int adminAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", i.AgentId);
        p.Add("@LastName", i.LastName);
        p.Add("@FirstName", i.FirstName);
        p.Add("@Phone", i.Phone);
        p.Add("@Email", i.Email);
        p.Add("@BranchId", i.BranchId);
        p.Add("@Role", i.Role);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Agent_Update", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<ProcResult> SetActiveAsync(int agentId, bool isActive, int adminAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@IsActive", isActive);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Agent_SetActive", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<ProcResult> ResetPasswordAsync(int agentId, string passwordHash, int adminAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@PasswordHash", passwordHash);
        p.Add("@AdminAgentId", adminAgentId);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Agent_ResetPassword", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }

    public async Task<List<BranchAdminItem>> ListBranchesAsync()
    {
        using var db = _factory.Create();
        var rows = await db.QueryAsync<BranchAdminItem>(
            "dbo.usp_Branch_ListAdmin", commandType: CommandType.StoredProcedure);
        return rows.ToList();
    }

    public async Task<ProcResult> SaveBranchAsync(int branchId, string name, bool isActive, int adminAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@BranchId", branchId == 0 ? null : branchId);
        p.Add("@Name", name);
        p.Add("@IsActive", isActive);
        p.Add("@AdminAgentId", adminAgentId);
        p.Add("@NewBranchId", dbType: DbType.Int32, direction: ParameterDirection.Output);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Branch_Save", p, commandType: CommandType.StoredProcedure);
        return ReadResult(p);
    }
    public async Task<(ProcResult, string?)> SetPhotoAsync(
    int agentId, string? photoUrl, string? photoPublicId, int actorAgentId)
    {
        using var db = _factory.Create();

        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@PhotoUrl", photoUrl);
        p.Add("@PhotoPublicId", photoPublicId);
        p.Add("@ActorAgentId", actorAgentId);
        p.Add("@OldPublicId", dbType: DbType.String, direction: ParameterDirection.Output, size: 300);
        WithOutputs(p);

        await db.ExecuteAsync("dbo.usp_Agent_SetPhoto", p, commandType: CommandType.StoredProcedure);

        return (ReadResult(p), p.Get<string?>("@OldPublicId"));
    }
}