using Actively.BlobStorage;
using Actively.Context;
using Actively.Controllers.Repositories;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Services.GeoJsonGenerator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
	.AddDbContext<ActivelyDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetConnectionString("DbDev")))
	.AddScoped<IActivityRepository, ActivityRepository>()
	.AddScoped<StorageManager>()
	.AddScoped<GeoJsonGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
