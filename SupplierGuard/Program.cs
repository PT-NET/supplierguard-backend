using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using SupplierGuard.Application;
using SupplierGuard.Domain.Interfaces;
using SupplierGuard.Infrastructure;
using SupplierGuard.Infrastructure.Persistence;
using SupplierGuard.Infrastructure.Persistence.Seeds;
using SupplierGuard.Middleware;
using SupplierGuard.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/supplierguard-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
    options.Audience = builder.Configuration["Auth0:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}/",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Auth0:Audience"],
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

// Add services to the container.

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Supplier Management API",
        Version = "v1",
        Description = "API para gestión de proveedores con screening de listas de alto riesgo",
        Contact = new OpenApiContact
        {
            Name = "EY Technical Assessment",
            Email = "tu-email@example.com"
        }
    });

    // SIMPLIFIED AUTH0 CONFIGURATION
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ingrese 'Bearer' [espacio] y luego su token.\r\n\r\nEjemplo: \"Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:3000", "http://localhost:4200" };

    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});



var app = builder.Build();

app.UseExceptionHandlingMiddleware();

// Swagger (only in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Supplier Management API v1");
        options.RoutePrefix = string.Empty; 
    });

    // Automatic seed in development
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        await context.Database.MigrateAsync();

        logger.LogInformation("Seeding database...");
        await ApplicationDbContextSeed.SeedAsync(context);

        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}


// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}*/

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

//app.MapHealthChecks("/health");

app.MapControllers();

app.Logger.LogInformation("🚀 Supplier Management API is running");
app.Logger.LogInformation("📖 Swagger UI available at: {Url}",
    app.Environment.IsDevelopment() ? "https://localhost:{port}/" : "N/A");

try
{
    Log.Information("Starting Risk Screening API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}