using Microsoft.EntityFrameworkCore;
using Pi_Plant.Data; // Adjust this namespace to match your project structure

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite with Entity Framework Core
builder.Services.AddDbContext<PlantMonitoringContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ensure database is created and apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PlantMonitoringContext>();
    try
    {
        // This will create the database if it doesn't exist and apply migrations
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log the error - you might want to add proper logging here
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serve static files from wwwroot (for your React build)
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

// Fallback to React for client-side routing
app.MapFallbackToFile("index.html");

app.Run();