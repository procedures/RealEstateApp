using System.Data;
using Microsoft.Data.SqlClient;

namespace RealEstateApp.Data;

public interface IDbConnectionFactory
{
    IDbConnection Create();
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("'DefaultConnection' connection string тохируулаагүй байна.");
    }

    public IDbConnection Create() => new SqlConnection(_connectionString);
}