using Actively.BlobStorage;
using Actively.Context;
using Actively.Controllers.Repositories;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Services.AuthService;
using Actively.Services.AuthService.Configuration;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.GeoJsonGenerator;
using Actively.Services.InputFormatters;
using Actively.Services.PasswordHasher;
using Actively.Services.PasswordHasher.Interfaces;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
	.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"))
		.AddScoped<StorageManager>()
	.AddScoped<GeoJsonGenerator>()
	.AddScoped<JwtConfig>()
	.AddScoped<ITokenService, TokenService>()
	.AddScoped<IPasswordHasher, PasswordHasher>()
	.AddScoped<IActivityRepository, ActivityRepository>()
	.AddScoped<IUserRepository, UserRepository>()
	.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
		ValidAudience = builder.Configuration["JwtConfig:Audience"],
		IssuerSigningKey = new SymmetricSecurityKey
		(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!)),
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true
	};
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
