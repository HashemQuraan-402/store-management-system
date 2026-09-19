using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using StoreManagement.Data;
using StoreManagement.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Services
// ---------------------------------------------------------------------------

builder.Services.AddControllers();

// EF Core - SQL Server. Swap UseSqlServer for UseInMemoryDatabase("CSU") for quick local
// testing without a real database.
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnString")));

// Application services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be configured with at least 32 characters. " +
        "Use .NET user-secrets for local development.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.AddAuthorization();

// CORS - allow the Angular dev server (and any origins configured in appsettings) to call the API.
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("StoreManagement.Client", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger / OpenAPI, with JWT "Authorize" support so the API can be exercised directly.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Store Management API",
        Version = "v1",
        Description = ".Net Full Stack Web Developer Assignment - backend API"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter the JWT token obtained from /api/auth/login."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ---------------------------------------------------------------------------
// Seed the database with dummy users (per assignment note) on startup.
// Skipped when EF Core design-time tooling (Add-Migration / Update-Database /
// dotnet ef) is the one executing this file to discover the DbContext - that
// tooling runs everything between builder.Build() and app.Run() for real, so
// hitting a database that isn't reachable/created yet here would break
// migration commands with an opaque error instead of just being skipped.
var isEfDesignTime = AppDomain.CurrentDomain.GetAssemblies()
    .Any(a => a.GetName().Name == "Microsoft.EntityFrameworkCore.Design");

if (!isEfDesignTime)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    var seedUsersPassword = builder.Configuration["SeedUsers:Password"];
    if (string.IsNullOrWhiteSpace(seedUsersPassword))
    {
        throw new InvalidOperationException(
            "SeedUsers:Password must be configured. Use .NET user-secrets for local development.");
    }

    DbSeeder.Seed(context, seedUsersPassword);
}

// ---------------------------------------------------------------------------
// Middleware pipeline
// ---------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Management API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("StoreManagement.Client");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
