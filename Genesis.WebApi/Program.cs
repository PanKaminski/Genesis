using Genesis.App.Implementation.Utils;
using Genesis.WebApi.Middlewares;
using Genesis.WebApi.Platform;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CorsSettings>(builder.Configuration.GetSection("CorsSettings"));
builder.Services.Configure<SwaggerSettings>(builder.Configuration.GetSection("SwaggerSettings"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Add services to the container.
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCors();
builder.Services.AddEFCore(builder.Configuration);
builder.Services.AddDal(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
var swaggerSettings = app.Services.GetRequiredService<IOptionsMonitor<SwaggerSettings>>();
if (app.Environment.IsDevelopment() && swaggerSettings.CurrentValue.SwaggerOn)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseExceptionMiddleware();

var corsSettings = app.Services.GetRequiredService<IOptionsMonitor<CorsSettings>>();

app.UseCors(options =>
{
    options.WithOrigins(corsSettings.CurrentValue.AllowedCorsOrigins)
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();