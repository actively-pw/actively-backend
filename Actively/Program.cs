using Actively.BlobStorage;
using Actively.Context;
using Actively.Controllers.Repositories;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Services.GeoJsonGenerator;
using Actively.Services.InputFormatters;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
	options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Configuration.AddAzureKeyVault(
	new Uri(builder.Configuration.GetSection("KeyVaultUrl").Value!),
	new DefaultAzureCredential()
	);

builder.Services
	.AddDbContext<ActivelyDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetSection("DbAzure").Value!))
	.AddScoped<IActivityRepository, ActivityRepository>()
	.AddScoped<StorageManager>()
	.AddScoped<GeoJsonGenerator>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
