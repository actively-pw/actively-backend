using Actively.BlobStorage;
using Actively.BlobStorage.Interfaces;
using Actively.Context;
using Actively.Controllers.Repositories;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Services.AuthService;
using Actively.Services.AuthService.Configuration;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.GeoJsonGenerator;
using Actively.Services.GeoJsonGenerator.Interfaces;
using Actively.Services.InputFormatters;
using Actively.Services.PasswordHasher;
using Actively.Services.PasswordHasher.Interfaces;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
	options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Configuration.AddAzureKeyVault(
	new Uri(builder.Configuration.GetSection("KeyVaultUrl").Value!),
	new DefaultAzureCredential()
	);

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Actively" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "This API uses bearer authentication. Bearer token must be passed as \"Bearer PasteJwtHere\".",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
		new OpenApiSecurityScheme
		{
			Name="Bearer",
			Scheme="oauth2",
			In = ParameterLocation.Header,
			Reference = new OpenApiReference
			{
				Type = ReferenceType.SecurityScheme,
				Id = "Bearer"
			}
		},
		new List<string>()
		}
	});

}
);

builder.Services
	.AddDbContext<ActivelyDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetSection("DbAzure").Value!))
	.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"))
	.AddScoped<IStorageManager, StorageManager>()
	.AddScoped<IGeoJsonGenerator, GeoJsonGenerator>()
	.AddScoped<JwtConfig>()
	.AddScoped<ITokenService, TokenService>()
	.AddScoped<IPasswordHasher, PasswordHasher>()
	.AddScoped<IActivityRepository, ActivityRepository>()
	.AddScoped<IUserRepository, UserRepository>()
	.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", policy =>
	{
		policy.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader();
	});
});


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


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
