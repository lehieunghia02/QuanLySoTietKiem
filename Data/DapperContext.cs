using System.Data;
using Microsoft.Data.SqlClient;

namespace QuanLySoTietKiem.Data;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DeployConnection");
    }

    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}