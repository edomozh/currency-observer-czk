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

builder.Services.AddControllers();
builder.Services.AddScoped<RateService>();
builder.Services.AddScoped<CurrencyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
