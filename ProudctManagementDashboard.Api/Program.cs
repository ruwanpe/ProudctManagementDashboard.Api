using Microsoft.EntityFrameworkCore;
using ProudctManagementDashboard.Api.Cache;
using ProudctManagementDashboard.Api.Data;
using ProudctManagementDashboard.Api.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IMemeoryCacheService, MemoryCacheService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProductDbContext>(
     o => o.UseSqlite(builder.Configuration.GetConnectionString("Default"))
    );

builder.Services.AddScoped<IProduct, ProductRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
