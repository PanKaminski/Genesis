using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.Dtos;
using Microsoft.EntityFrameworkCore;
using Genesis.Common.Enums;

namespace Genesis.DAL.Implementation.Context
{
    internal static class ModelBuilderExtensions
    {
        public static ModelBuilder CreateGenesisRelations(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RefreshTokenDto>()
                .HasOne(rt => rt.Account)
                .WithMany(a => a.RefreshTokens)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<AccountDto>()
                .HasMany(a => a.Roles)
                .WithMany(r => r.Accounts)
                .UsingEntity(e => e.ToTable("persons_roles"));

            modelBuilder.Entity<AccountDto>()
                .HasOne(a => a.RootPerson)
                .WithOne(p => p.Account)
                .HasForeignKey<PersonDto>()
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<AccountDto>()
                .HasMany(a => a.AvailableTrees)
                .WithMany(p => p.Modifiers)
                .UsingEntity(e => e.ToTable("trees_modifiers"));

            modelBuilder.Entity<PictureDto>()
                .HasOne(p => p.Person)
                .WithOne()
                .HasForeignKey<PictureDto>();

            modelBuilder.Entity<PersonDto>()
                .HasOne(p => p.Biography)
                .WithOne(b => b.Person)
                .HasForeignKey<BiographyDto>(b => b.PersonId)
                .IsRequired(false);

            modelBuilder.Entity<PersonDto>()
                .HasMany(p => p.RelatedDocuments)
                .WithMany(d => d.Persons)
                .UsingEntity(e => e.ToTable("persons_documents"));

            modelBuilder.Entity<PersonDto>()
                .HasMany(p => p.Photos)
                .WithOne(ph => ph.Person)
                .HasForeignKey(ph => ph.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersonRelationDto>()
                .HasOne(pr => pr.ToPerson)
                .WithMany(p => p.RelationsAsDependent)
                .HasForeignKey(pr => pr.ToPersonId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PersonRelationDto>()
                .HasOne(pr => pr.FromPerson)
                .WithMany(p => p.RelationsAsRoot)
                .HasForeignKey(pr => pr.FromPersonId);

            modelBuilder.Entity<BiographyDto>()
                .HasOne(b => b.BirthPlace)
                .WithMany(a => a.BirthdayAttachedBiographies)
                .HasForeignKey(b => b.BirthPlaceId);

            modelBuilder.Entity<BiographyDto>()
                .HasOne(b => b.DeathPlace)
                .WithMany(a => a.DayOfDeathAttachedBiographies)
                .HasForeignKey(b => b.DeathPlaceId);

            modelBuilder.Entity<BiographyDto>()
                .HasMany(b => b.Events)
                .WithMany(n => n.Biographies)
                .UsingEntity(e => e.ToTable("events_biographies"));

            modelBuilder.Entity<AddressDto>()
                .HasMany(a => a.Notations)
                .WithOne(n => n.Place)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<AccountDto>()
                .HasMany(a => a.PersonalTrees)
                .WithOne(t => t.Owner)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GenealogicalTreeDto>()
                .HasOne(gt => gt.CoatOfArms)
                .WithOne(p => p.GenealogicalTree)
                .HasForeignKey<PictureDto>(ph => ph.GenealogicalTreeId)
                .IsRequired(false);

            modelBuilder.Entity<GenealogicalTreeDto>()
                .HasMany(gt => gt.Persons)
                .WithOne(p => p.GenealogicalTree)
                .HasForeignKey(p => p.GenealogicalTreeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GenealogicalTreeDto>()
                .HasMany(gt => gt.Relations)
                .WithOne(p => p.GenealogicalTree)
                .HasForeignKey(p => p.GenealogicalTreeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccountConnectionDto>()
            .HasOne(ac => ac.AccountFrom)
            .WithMany(a => a.OutgoingConnections)
            .HasForeignKey(ac => ac.AccountFromId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccountConnectionDto>()
                .HasOne(ac => ac.AccountTo)
                .WithMany(a => a.IncomingConnections)
                .HasForeignKey(uc => uc.AccountToId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoricalNotationDto>()
                .HasMany(hn => hn.Pictures)
                .WithOne(p => p.HistoricalNotation)
                .HasForeignKey(p => p.HistoricalNotationId)
                .IsRequired(false);

            return modelBuilder;
        }

        public static ModelBuilder ConfigureTableNaming(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountDto>().ToTable("accounts");
            modelBuilder.Entity<RefreshTokenDto>().ToTable("refresh_tokens");
            modelBuilder.Entity<PersonDto>().ToTable("persons");
            modelBuilder.Entity<BiographyDto>().ToTable("biographies");
            modelBuilder.Entity<AddressDto>().ToTable("locations");
            modelBuilder.Entity<GenealogicalTreeDto>().ToTable("genealogical_trees");
            modelBuilder.Entity<DocumentDto>().ToTable("documents");
            modelBuilder.Entity<HistoricalNotationDto>().ToTable("notations");
            modelBuilder.Entity<PictureDto>().ToTable("pictures");
            modelBuilder.Entity<RoleDto>().ToTable("roles");
            modelBuilder.Entity<PersonRelationDto>().ToTable("relations");
            modelBuilder.Entity<AccountConnectionDto>().ToTable("account_connections");

            return modelBuilder;
        }

        public static ModelBuilder SeedData(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleDto>().HasData(
                new { Id = 1, RoleName = Role.User },
                new { Id = 2, RoleName = Role.Admin },
                new { Id = 3, RoleName = Role.ArchiveWorker }
            );

            return modelBuilder;
        }

        public static ModelBuilder ApplyConstraints(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonDto>().Property(p => p.FirstName).HasMaxLength(32);
            modelBuilder.Entity<PersonDto>().Property(p => p.LastName).HasMaxLength(32);
            modelBuilder.Entity<PersonDto>().Property(p => p.MiddleName).HasMaxLength(32);

            modelBuilder.Entity<AccountDto>().Property(a => a.Login).HasMaxLength(32);
            modelBuilder.Entity<AccountDto>().Property(a => a.CountryCode).HasMaxLength(8);
            modelBuilder.Entity<AccountDto>().Ignore(a => a.IsVerified);

            modelBuilder.Entity<BiographyDto>().Property(b => b.Info).HasMaxLength(1024);

            modelBuilder.Entity<RoleDto>().Property(r => r.RoleName)
                .HasConversion(n => n.ToString(), 
                    str => Enum.Parse<Role>(str));

            return modelBuilder;
        }
    }
}
