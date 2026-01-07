using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpOverrides;
using _2022_CS_668.Data;
using _2022_CS_668.Models;
using _2022_CS_668.Repositories;
using _2022_CS_668.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Antiforgery
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".MessSystem.Antiforgery";
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.HeaderName = "X-CSRF-TOKEN";
});

// Configure Database - prefer DATABASE_URL (Render) which implies PostgreSQL
var connectionString = string.Empty;
var preferNpgsql = false;

var envDatabaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrWhiteSpace(envDatabaseUrl))
{
    preferNpgsql = true;
    // Trim and remove surrounding quotes that some platforms add
    envDatabaseUrl = envDatabaseUrl.Trim();
    if ((envDatabaseUrl.StartsWith("\"") && envDatabaseUrl.EndsWith("\"")) || (envDatabaseUrl.StartsWith("'") && envDatabaseUrl.EndsWith("'")))
    {
        envDatabaseUrl = envDatabaseUrl.Substring(1, envDatabaseUrl.Length - 2);
    }

    // Normalize scheme variants
    if (envDatabaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        envDatabaseUrl = "postgres://" + envDatabaseUrl.Substring("postgresql://".Length);
    }

    // If DATABASE_URL is in the form postgres://user:pass@host:port/db
    if (envDatabaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var databaseUri = new Uri(envDatabaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':', 2);
            var host = databaseUri.Host;
            var port = databaseUri.Port > 0 ? databaseUri.Port : 5432;
            var database = databaseUri.LocalPath.TrimStart('/');
            var user = userInfo.Length > 0 ? userInfo[0] : string.Empty;
            var pass = userInfo.Length > 1 ? userInfo[1] : string.Empty;
            connectionString = $"Host={host};Port={port};Database={database};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true";
        }
        catch
        {
            // Fallback to using raw env value (may be a full connection string)
            connectionString = envDatabaseUrl;
        }
    }
    else
    {
        // If it's not a URL, assume it might already be a valid connection string
        connectionString = envDatabaseUrl;
    }
}

// If no DATABASE_URL, choose based on environment
if (string.IsNullOrWhiteSpace(connectionString))
{
    if (builder.Environment.IsProduction())
    {
        connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrWhiteSpace(connectionString)) preferNpgsql = true;
    }
    else
    {
        connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    }
}

// Validate connection string for production when no provider selected
if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No database connection string configured for production. Set the DATABASE_URL environment variable or PostgreSQL connection string.");
}

if (preferNpgsql)
{
    // Final sanity check: if the connectionString appears to start with a URL scheme still, throw a helpful error
    var trimmed = connectionString?.TrimStart();
    if (!string.IsNullOrWhiteSpace(trimmed) && (trimmed.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) || trimmed.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
    {
        throw new InvalidOperationException("DATABASE_URL appears to be a URL. Ensure Program.cs converts it to a valid Npgsql connection string or set a full connection string in the PostgreSQL setting.");
    }

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Cookie Settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".MessSystem.Auth";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Register Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

// Configure forwarded headers to respect X-Forwarded-For and X-Forwarded-Proto (needed behind proxy)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // Allow any proxy (Render provides forwarded headers)
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Apply migrations automatically in production
        if (app.Environment.IsProduction())
        {
            // Use EnsureDeleted+EnsureCreated for Npgsql to reset DB in production
            var provider = context.Database.ProviderName ?? string.Empty;
            if (provider.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            {
                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }
            else
            {
                await context.Database.MigrateAsync();
            }
        }
        
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData.Initialize(services, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Account/Login");
    app.UseHsts();
}

// Enable forwarded headers middleware early so request.IsHttps is correct behind proxy
app.UseForwardedHeaders();

// Use HTTPS redirection in production
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Default route redirects to Login
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
