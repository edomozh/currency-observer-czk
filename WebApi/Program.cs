using DataAccessLibrary.Contexts;
using DataAccessLibrary.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("CurrencyDatabase");
builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName));
            options.UseSqlServer(connectionString, x => x.MigrationsAssembly("DataAccessLibrary"));
        }
    );

builder.Services.AddScoped<RateRepository>();
builder.Services.AddScoped<CurrencyRepository>();

builder.Services.AddScoped<RateService>();
builder.Services.AddScoped<CurrencyService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigins");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();

app.Run();
