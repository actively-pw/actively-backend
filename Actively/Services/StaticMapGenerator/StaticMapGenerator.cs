using Actively.Models.DTOs;
using Actively.Services.StaticMapGenerator.Configuration;
using Actively.Services.StaticMapGenerator.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;


namespace Actively.Services.StaticMapGenerator
{
	public class StaticMapGenerator : IStaticMapGenerator
	{
		private readonly MapBoxConfig _config;
		private readonly int _webWidth = 1100; // in pixels
		private readonly int _webHeight = 500;
		private readonly int _mobileWidth = 1100;
		private readonly int _mobileHeight = 750;

		public StaticMapGenerator(IOptions<MapBoxConfig> config)
		{
			_config = config.Value;
		}
		public async Task<StaticMapsDto> Generate(MemoryStream geojson)
		{
			using(var reader = new StreamReader(geojson))
			{
				geojson.Position = 0;
				string g = reader.ReadToEnd();

				Stream webLight = await GetStaticMap(g, _webWidth, _webHeight);
				Stream mobileLight = await GetStaticMap(g, _mobileWidth, _mobileHeight);

				return new StaticMapsDto() { MobileLight = mobileLight, WebLight = webLight };
			}
		}

		private async Task<Stream> GetStaticMap(string geojson, int width, int height)
		{
			string url = $"mapbox/streets-v12/static/geojson({geojson})/auto/{width}x{height}?access_token={_config.StaticImagesToken}";

			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(Path.Combine(_config.BaseUrl, url));

				if (!response.IsSuccessStatusCode)
				{
					throw new WebException(response.StatusCode.ToString());
				}

				return response.Content.ReadAsStream();
			}
		}
	}
}
