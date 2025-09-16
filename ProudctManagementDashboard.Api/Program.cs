using Microsoft.EntityFrameworkCore;
using ProudctManagementDashboard.Api.Cache;
using ProudctManagementDashboard.Api.Data;
using ProudctManagementDashboard.Api.Repository;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .WriteTo.File("logs/ProductManagementDashboard-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IMemeoryCacheService, MemoryCacheService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProductDbContext>(
     o => o.UseSqlite(builder.Configuration.GetConnectionString("Default"))
    );

builder.Services.AddScoped<IProductRepo, ProductRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

Log.CloseAndFlush();
