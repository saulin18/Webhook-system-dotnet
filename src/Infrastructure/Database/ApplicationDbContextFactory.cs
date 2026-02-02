using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Infrastructure.Database;

public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string infrastructurePath = Path.Combine(currentDirectory, "..", "Infrastructure");
        string webApiPath = Path.Combine(currentDirectory, "..", "Web.Api");

        var builder = new ConfigurationBuilder();

        // Try Web.Api first (most common location)
        if (Directory.Exists(webApiPath))
        {
            builder.SetBasePath(webApiPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true);
        }

        // Also try Infrastructure directory
        if (Directory.Exists(infrastructurePath))
        {
            builder.SetBasePath(infrastructurePath)
                .AddJsonFile("appsettings.json", optional: true);
        }

        // Fallback to current directory
        builder.SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", optional: true);

        IConfiguration configuration = builder.Build();

        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("Database"), npgsqlOptions =>
            npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
            .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


