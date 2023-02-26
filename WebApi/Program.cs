using DataAccessLibrary.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
