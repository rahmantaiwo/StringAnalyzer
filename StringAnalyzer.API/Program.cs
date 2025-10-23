using Microsoft.EntityFrameworkCore;
using StringAnalyzer.API.Middleware;
using StringAnalyzer.API.Persistence;
using StringAnalyzer.API.Repositories.IRepository;
using StringAnalyzer.API.Repositories.Repository;
using StringAnalyzer.API.Services.IService;
using StringAnalyzer.API.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add repositories
builder.Services.AddScoped<IStringRepository, StringRepository>();

// Add services
builder.Services.AddScoped<IStringAnalyzerService, StringAnalyzerService>();

// Configure SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS redirection
app.UseHttpsRedirection();
app.UseAuthorization();

// Redirect root / to Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

// Map controllers
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Run app with Kestrel (no IIS)
app.Run();
