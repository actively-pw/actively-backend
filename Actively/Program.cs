using Actively.BlobStorage;
using Actively.Context;
using Actively.Controllers.Repositories;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Services.GeoJsonGenerator;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
	.AddDbContext<ActivelyDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings:DbDev").Value!))
	.AddScoped<IActivityRepository, ActivityRepository>()
	.AddScoped<StorageManager>()
	.AddScoped<GeoJsonGenerator>();

// TODO:
//builder.Configuration.AddAzureKeyVault(
//	new Uri(builder.Configuration.GetSection("KeyVaultUrl").Value!),
//	new DefaultAzureCredential()
//	);

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
