using ProjectTemplate.Application;
using ProjectTemplate.Infrastructure;
using ProjectTemplate.Infrastructure.Data;
using ProjectTemplate.Shared.Extensions;
using ProjectTemplate.Shared.PostgresqlCache;
using ProjectTemplate.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.AddProjectTemplateAuthorization();
var app = builder.Build();
await app.UsePostgresqlCacheAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{   
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});


app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));

app.MapEndpoints();

app.Run();

public partial class Program { }
