using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Dtos.Account;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Context
{
    public class GenesisDbContext : DbContext
    {
        public GenesisDbContext(DbContextOptions options): base(options) { }

        public DbSet<AccountDto> Accounts { get; set; }
        public DbSet<RefreshTokenDto> RefreshTokens { get; set; }
        public DbSet<PersonDto> Persons { get; set; }
        public DbSet<PersonRelationDto> Relations { get; set; }
        public DbSet<BiographyDto> Biographies { get; set; }
        public DbSet<AddressDto> Locations { get; set; }
        public DbSet<GenealogicalTreeDto> Trees { get; set; }
        public DbSet<DocumentDto> Documents { get; set; }
        public DbSet<HistoricalNotationDto> Notations { get; set; }
        public DbSet<PictureDto> Pictures { get; set; }
        public DbSet<AccountConnectionDto> AccountConnections { get; set; }

        public DbSet<RoleDto> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureTableNaming()
                .ApplyConstraints()
                .CreateGenesisRelations()
                .SeedData();
        }
    }
}
