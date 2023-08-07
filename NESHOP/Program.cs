using AspNetCore.Report.ReportService2010_;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using NeSHOP.Contacts;
using NeSHOP.DAL;
using NESHOP.Auth;
using NESHOP.Contacts;
//using NESHOP.DAL;
using Serilog;

try
{
        //create the logger and setup your sinks, filters and properties
        Log.Logger = new LoggerConfiguration()
         .WriteTo.Console()
         .CreateBootstrapLogger();

        Log.Information("Starting up");


        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));


        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();

        builder.Services.AddScoped<IBllDbConnection, BllDbConnection>();
        builder.Services.AddScoped<IBllAdmin, BllAdmin>();
        builder.Services.AddScoped<IBllUserInfo, BllUserInfo>();
        builder.Services.AddScoped<IBllCommon, BllCommon>();
        builder.Services.AddScoped<IBllDbConnection, BllDbConnection>();
        builder.Services.AddScoped<IBllIteam, BllIteam>();
        builder.Services.AddScoped<IBllSettings, BllSettings>();
        builder.Services.AddScoped<IBllOrder, BllOrder>();
        builder.Services.AddScoped<IBllSuplier, BllSuplier>();
        builder.Services.AddScoped<IBllTran, BllTran>();
        //builder.Services.AddScoped<IBllBarcode, BllBarcode>();


         builder.Services.AddCors();
        builder.Services.AddRazorPages();
    //builder.Services.AddAuthentication();

   // builder.Services.AddAuthorization();

        //builder.Services.AddSingleton<IAuthorizationHandler, AccountHandler>();

        builder.Services.AddControllersWithViews();

        //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        //var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var environmentName = builder.Environment.EnvironmentName;

        var app = builder.Build();

        app.UseSerilogRequestLogging();
      
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.UseSession();

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