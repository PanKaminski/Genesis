using System.Text;
using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Connections.Services;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Implementation.Authentication.Services;
using Genesis.App.Implementation.Common.Services;
using Genesis.App.Implementation.Connections.Services;
using Genesis.App.Implementation.Dashboard.Services;
using Genesis.App.Implementation.DataManager;
using Genesis.App.Implementation.Utils;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Contract.UOW;
using Genesis.DAL.Implementation.Context;
using Genesis.DAL.Implementation.Repositories;
using Genesis.DAL.Implementation.UOW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Genesis.WebApi.Platform;

internal static class ServiceExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.GetSection("JwtSettings").Bind(jwtSettings);
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(authenticationScheme: JwtBearerDefaults.AuthenticationScheme, configureOptions: jwtOptions =>
            {
                jwtOptions.IncludeErrorDetails = true;
                jwtOptions.SaveToken = true;
                jwtOptions.RequireHttpsMetadata = false;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "https://localhost:8181",
                    ValidAudience = "https://localhost:8181",
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = secretKey,
                };
            });
        
        return services;
    }

    public static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<GenesisDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Genesis")));
    }

    public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAccountsRepository, AccountsRepository>();
        services.AddScoped<IPersonsRepository, PersonsRepository>();
        services.AddScoped<IGenealogicalTreesRepository, GenealogicalTreesRepository>();
        services.AddScoped<IPicturesRepository, PicturesRepository>();
        services.AddScoped<IRelationsRepository, RelationsRepository>();
        services.AddScoped<IAccountConnectionsRepository, AccountConnectionsRepository>();

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddHttpClient<ILocationReceiver, LocationReceiver>();
        services.AddSingleton<ILocationsService, LocationsService>();
        services.AddScoped<ITokenManager, TokenManager>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<IEmailService, EmailService>();
        services.AddScoped<IGenealogicalTreeService, GenealogicalTreeService>();
        services.AddScoped<IPersonService, PersonService>();
        services.AddSingleton<ICloudinaryService, CloudinaryService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IRelationsService, RelationsService>();
        services.AddScoped<IConnectionsService, ConnectionsService>();
        services.AddScoped<DashboardToolService>();
        services.AddScoped<ContactsService>();
        services.AddScoped<DataManagerToolService>();

        return services;
    }
}