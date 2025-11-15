using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using NeSHOP.Contacts;
using NeSHOP.DAL;
using NESHOP.Auth;
using NESHOP.Contacts;
using Serilog;

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
    builder.Services.AddSession();

    // CORS
    builder.Services.AddCors();

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
    //builder.Services.AddScoped<IBllBarcode, BllBarcode>();

    var app = builder.Build();

    // Serilog request logging
    app.UseSerilogRequestLogging();

    // Static files
    app.UseStaticFiles();

    // Routing
    app.UseRouting();

    // Authorization (after routing)
    app.UseAuthorization();

    // Session (must be before MVC endpoint execution)
    app.UseSession();

    // Razor pages
    app.MapRazorPages();

    // MVC Route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Logon}/{id?}");

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
