using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyFitBook.BlobStorage;
using MyFitBook.BlobStorage.Interfaces;
using MyFitBook.Context;
using MyFitBook.Controllers.Repositories;
using MyFitBook.Controllers.Repositories.Interfaces;
using MyFitBook.Services.AuthService;
using MyFitBook.Services.AuthService.Configuration;
using MyFitBook.Services.AuthService.Interfaces;
using MyFitBook.Services.GeoJsonGenerator;
using MyFitBook.Services.GeoJsonGenerator.Interfaces;
using MyFitBook.Services.InputFormatters;
using MyFitBook.Services.PasswordHasher;
using MyFitBook.Services.PasswordHasher.Interfaces;
using MyFitBook.Services.PolylineHelpers;
using MyFitBook.Services.PolylineHelpers.Interfaces;
using MyFitBook.Services.StaticMapGenerator;
using MyFitBook.Services.StaticMapGenerator.Configuration;
using MyFitBook.Services.StaticMapGenerator.Interfaces;
using MyFitBook.Services.StatisticsCalculator;
using MyFitBook.Services.StatisticsCalculator.Interfaces;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
	options.InputFormatters.Insert(0, MyJPIF.GetJsonPatchInputFormatter());
});

builder.Services.AddEndpointsApiExplorer();


builder.Configuration.AddAzureKeyVault(
	new Uri(builder.Configuration.GetSection("KeyVaultUrl").Value!),
	new DefaultAzureCredential()
	);

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "My FitBook API",
		Description = "An ASP.NET Core Web API for final thesis project",
		License = new OpenApiLicense
		{
			Name = "MIT License",
			Url = new Uri("https://opensource.org/licenses/MIT")
		},
		Version = "v1"
	});

	// generate the xml docs that will drive the swagger docs
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

	c.IncludeXmlComments(xmlPath);

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

});

builder.Services
	.AddDbContext<MyFitBookDbContext>(options =>
		options.UseSqlServer(builder.Configuration.GetSection("DbAzure").Value!))
	.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"))
	.Configure<MapBoxConfig>(builder.Configuration.GetSection("MapBox"))
	.AddScoped<JwtConfig>()
	.AddScoped<MapBoxConfig>()
	.AddScoped<IStorageManager, StorageManager>()
	.AddScoped<IPolylineEncoder, PolylineEncoder>()
	.AddScoped<IGeoJsonGenerator, GeoJsonGenerator>()
	.AddScoped<IStaticMapGenerator, StaticMapGenerator>()
	.AddScoped<IStatisticsCalculator, StatisticsCalculator>()
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


app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
