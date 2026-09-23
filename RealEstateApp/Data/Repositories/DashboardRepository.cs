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

    /// <summary>Агент: сонгосон сарын хувийн статистик.</summary>
    Task<AgentMonthlyStats> GetAgentMonthlyStatsAsync(int agentId, int year, int month);

    /// <summary>Админ: сонгосон жилийн сар бүрийн нийт борлуулалтын трэнд.</summary>
    Task<List<MonthlySalesPoint>> GetAdminSalesTrendAsync(int year);

    /// <summary>Админ: сонгосон сарын агентуудын чансаа.</summary>
    Task<List<AgentLeaderboardRow>> GetAdminMonthlyLeaderboardAsync(int year, int month);

    /// <summary>Админ: дүүрэг/хотхойн задаргаа.</summary>
    Task<LocationBreakdownResult> GetAdminLocationBreakdownAsync(int top = 15);

    /// <summary>Админ: сонгосон жилийн хөрөнгийн урсгалын тойм.</summary>
    Task<PropertyFunnelResult> GetAdminPropertyFunnelAsync(int year);
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

    public async Task<AgentMonthlyStats> GetAgentMonthlyStatsAsync(int agentId, int year, int month)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@AgentId", agentId);
        p.Add("@Year", year);
        p.Add("@Month", month);

        var stats = await db.QueryFirstOrDefaultAsync<AgentMonthlyStats>(
            "dbo.usp_Dashboard_AgentMonthlyStats", p, commandType: CommandType.StoredProcedure);

        return stats ?? new AgentMonthlyStats();
    }

    public async Task<List<MonthlySalesPoint>> GetAdminSalesTrendAsync(int year)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Year", year);

        var rows = await db.QueryAsync<MonthlySalesPoint>(
            "dbo.usp_Dashboard_AdminSalesTrend", p, commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<List<AgentLeaderboardRow>> GetAdminMonthlyLeaderboardAsync(int year, int month)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Year", year);
        p.Add("@Month", month);

        var rows = await db.QueryAsync<AgentLeaderboardRow>(
            "dbo.usp_Dashboard_AdminMonthlyLeaderboard", p, commandType: CommandType.StoredProcedure);

        return rows.ToList();
    }

    public async Task<LocationBreakdownResult> GetAdminLocationBreakdownAsync(int top = 15)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Top", top);

        using var multi = await db.QueryMultipleAsync(
            "dbo.usp_Admin_LocationBreakdown", p, commandType: CommandType.StoredProcedure);

        var districts = (await multi.ReadAsync<LocationBreakdownRow>()).ToList();
        var complexes = (await multi.ReadAsync<LocationBreakdownRow>()).ToList();

        return new LocationBreakdownResult { Districts = districts, Complexes = complexes };
    }

    public async Task<PropertyFunnelResult> GetAdminPropertyFunnelAsync(int year)
    {
        using var db = _factory.Create();
        var p = new DynamicParameters();
        p.Add("@Year", year);

        using var multi = await db.QueryMultipleAsync(
            "dbo.usp_Admin_PropertyFunnel", p, commandType: CommandType.StoredProcedure);

        var months = (await multi.ReadAsync<PropertyFunnelPoint>()).ToList();
        var cancelledTotal = await multi.ReadFirstOrDefaultAsync<int>();

        return new PropertyFunnelResult { Months = months, CancelledTotal = cancelledTotal };
    }
}
