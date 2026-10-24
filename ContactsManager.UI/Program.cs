using Entities;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.StartupExtensions;
using CRUDExample.Middleware;


var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration LoggerConfiguration)
   =>
{
    LoggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);



var app = builder.Build();


// create application pipeline
if(builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();


app.UseHttpLogging();



/*
app.Logger.LogDebug("debug-message");
app.Logger.LogInformation("information-message");
app.Logger.LogWarning("warning-message");
app.Logger.LogCritical("critical-message");
*/


if (builder.Environment.IsEnvironment("Test") == false)
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

ExcelPackage.License.SetNonCommercialPersonal("Tristan");

app.UseStaticFiles();


app.UseRouting();
app.UseAuthentication(); // Reading Idenity cookie 
app.UseAuthorization();
app.MapControllers();

app.UseEndpoints(enpoints =>
{
    enpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}"
        );

    enpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{ action}"
        );
});

app.Run();

public partial class Program { }


