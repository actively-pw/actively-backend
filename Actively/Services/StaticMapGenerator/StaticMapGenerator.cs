using Actively.Services.StaticMapGenerator.Configuration;
using Actively.Services.StaticMapGenerator.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;


namespace Actively.Services.StaticMapGenerator
{
	public class StaticMapGenerator : IStaticMapGenerator
	{
		private readonly MapBoxConfig _config;

		public StaticMapGenerator(IOptions<MapBoxConfig> config)
		{
			_config = config.Value;
		}
		public async Task<Stream> Generate(MemoryStream geojson)
		{
			using(var reader = new StreamReader(geojson))
			using(var client = new HttpClient())
			{
				geojson.Position = 0;
				string g = reader.ReadToEnd();

				if(g.Length>=7000)
				{
					throw new ArgumentException("Provided geojson file might be too long - cannot generate static map");
				}

				string url = $"mapbox/streets-v12/static/geojson({g})/auto/500x300?access_token={_config.StaticImagesToken}";
				var response = await client.GetAsync(Path.Combine(_config.BaseUrl, url));

				if(!response.IsSuccessStatusCode)
				{
					throw new WebException(response.StatusCode.ToString());
				}

				return response.Content.ReadAsStream();

			}
		}
	}
}
