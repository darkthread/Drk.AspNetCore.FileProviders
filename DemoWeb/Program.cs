using System.Text;
using Drk.AspNetCore.FileProviders;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
var sqliteDbPath = Path.Combine(builder.Environment.ContentRootPath, "static-files.sqlite");
builder.Services.AddDbContext<StaticFileDbContext>(options =>
{
    //options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Initial Catalog=StaticFiles;Integrated Security=True", b => b.MigrationsAssembly("DemoWeb"));
    options.UseSqlite($"data source={sqliteDbPath}", b => b.MigrationsAssembly("DemoWeb"));
});
builder.Services.AddScoped<StaticFileDbRepository>();
var app = builder.Build();

// init db
if (!File.Exists(sqliteDbPath))
{
    using (var scope = app.Services.CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<StaticFileDbContext>()
              .Database.Migrate();
    }
}

// insert the demo data on-demand
if (app.Configuration.GetValue<string>("insertDemoData", "false") == "true")
{
    using (var scope = app.Services.CreateScope())
    {
        var rsp = scope.ServiceProvider.GetRequiredService<StaticFileDbRepository>();
        var userId = "jeffrey";
        var clientIp = "::1";
        rsp.UpdateFile("/index.html", Encoding.UTF8.GetBytes("<html><body><link href=css/site.css rel=stylesheet /> Web in JSON<img src=imgs/logo.gif /></body></html>"), userId, clientIp);
        rsp.UpdateFile("/css/site.css", Encoding.UTF8.GetBytes("body { font-size: 9pt } img { display: block; margin-top: 5px; }"), userId, clientIp);
        rsp.UpdateFile("/imgs/logo.gif", Convert.FromBase64String("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"), userId, clientIp);
    }
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Drk.AspNetCore.FileProviders.StaticFileDbProvider(app.Services),
    RequestPath = "/web-in-db"
});

app.MapGet("/", () => Results.Redirect("~/web-in-db/index.html"));

app.Run();
