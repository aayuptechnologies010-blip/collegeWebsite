using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CollegeWebSite.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var databaseProvider = configuration["DatabaseProvider"];
        var connectionString = "";

        if (string.Equals(databaseProvider, "MySQL", System.StringComparison.OrdinalIgnoreCase))
        {
            connectionString = configuration.GetConnectionString("MySQL");
            builder.UseMySql(connectionString, new MySqlServerVersion(new System.Version(8, 0, 35)), 
                o => o.CommandTimeout(120).EnableRetryOnFailure());
        }
        else 
        {
            connectionString = configuration.GetConnectionString("PostgreSQL") ?? configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString);
        }

        return new ApplicationDbContext(builder.Options);
    }
}
