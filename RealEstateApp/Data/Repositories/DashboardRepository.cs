using System.Data;
using Dapper;
using RealEstateApp.Models;

namespace RealEstateApp.Data.Repositories;

public interface IDashboardRepository
{
    /// <summary>Админ: агентуудын чансаа (хөрөнгө / хаалтын тоо).</summary>
    Task<List<AgentLeaderboardRow>> GetAdminStatsAsync(int top = 10);

    /// <summary>Агент: өөрийн хувийн статистик.</summary>
    Task<AgentDashboardStats> GetAgentStatsAsync(int agentId);
}

public class DashboardRepository : IDashboardRepository
{
    private readonly IDbConnectionFactory _factory;
    public DashboardRepository(IDbConnectionFactory factory) => _factory = factory;

    public async Task<List<AgentLeaderboardRow>> GetAdminStatsAsync(int top = 10)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Top", top);

        var rows = await db.QueryAsync<AgentLeaderboardRow>(
            "dbo.usp_Dashboard_AdminStats", p, commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<AgentDashboardStats> GetAgentStatsAsync(int agentId)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);

        var stats = await db.QueryFirstOrDefaultAsync<AgentDashboardStats>(
            "dbo.usp_Dashboard_AgentStats", p, commandType: CommandType.StoredProcedure);

        return stats ?? new AgentDashboardStats();
    }
}
