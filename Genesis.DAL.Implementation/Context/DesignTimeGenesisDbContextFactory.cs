using Genesis.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Genesis.DAL.Implementation.Context
{
    internal class DesignTimeGenesisDbContextFactory : IDesignTimeDbContextFactory<GenesisDbContext>
    {
        public GenesisDbContext CreateDbContext(string[] args)
        {
            const string connectionStringName = "GENESIS";
            const string connectionStringPrefix = "SQLCONNSTR_";

            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new GenesisApplicationException($"{connectionStringPrefix}{connectionStringName} environment variable is not set.");
            }

            Console.WriteLine($"Using {connectionStringPrefix}{connectionStringName} environment variable as a connection string.");

            var builderOptions = new DbContextOptionsBuilder<GenesisDbContext>().UseSqlServer(connectionString).Options;
            return new GenesisDbContext(builderOptions);
        }
    }
}
