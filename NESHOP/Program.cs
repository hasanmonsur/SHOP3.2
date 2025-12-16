using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using NeSHOP.Contacts;
using NeSHOP.DAL;
using NESHOP.Auth;
using NESHOP.Contacts;
using Serilog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

try
{
    // Early logger for startup
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    Log.Information("Starting up");

    var builder = WebApplication.CreateBuilder(args);

    // Main Serilog configuration
    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    // MVC + Razor
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Session
    builder.Services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    });

    // CORS
    builder.Services.AddCors();

    // Response caching
    builder.Services.AddResponseCaching();

    // Dependency Injection
    builder.Services.AddScoped<IBllDbConnection, BllDbConnection>();
    builder.Services.AddScoped<IBllAdmin, BllAdmin>();
    builder.Services.AddScoped<IBllUserInfo, BllUserInfo>();
    builder.Services.AddScoped<IBllCommon, BllCommon>();
    builder.Services.AddScoped<IBllIteam, BllIteam>();
    builder.Services.AddScoped<IBllSettings, BllSettings>();
    builder.Services.AddScoped<IBllOrder, BllOrder>();
    builder.Services.AddScoped<IBllSuplier, BllSuplier>();
    builder.Services.AddScoped<IBllTran, BllTran>();
    
    var app = builder.Build();

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // HTTPS redirection
    app.UseHttpsRedirection();

    // Static files (with cache headers)
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            const int durationInSeconds = 60 * 60 * 24 * 7; // 7 days
            ctx.Context.Response.Headers["Cache-Control"] = $"public,max-age={durationInSeconds}";
        }
    });

    // Routing
    app.UseRouting();

    // Authorization (after routing)
    app.UseAuthorization();

    // Response caching
    app.UseResponseCaching();

    // Session (must be before MVC endpoint execution)
    app.UseSession();

    // Razor pages
    app.MapRazorPages();

    // MVC Route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Logon}/{id?}");

    // HSTS for non-Development
    if (!app.Environment.IsDevelopment())
    {
        app.UseHsts();
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}
