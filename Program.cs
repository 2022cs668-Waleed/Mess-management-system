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
    // Use SameAsRequest so forwarded HTTPS (from proxy) is respected
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Configure Database - Use PostgreSQL in production, SQL Server in development
var connectionString = string.Empty;
if (builder.Environment.IsProduction())
{
    // Prefer DATABASE_URL environment variable (Render)
    var envDatabaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(envDatabaseUrl))
    {
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
            connectionString = envDatabaseUrl; // maybe already a connection string
        }
    }

    // If still empty, try appsettings
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    }
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Validate connection string for production
if (builder.Environment.IsProduction() && string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("No database connection string configured for production. Set the DATABASE_URL environment variable or PostgreSQL connection string.");
}

if (builder.Environment.IsProduction())
{
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
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // Respect forwarded protocol by using SameAsRequest
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.MaxAge = null;
    options.Cookie.Expiration = null;
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
            context.Database.Migrate();
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
