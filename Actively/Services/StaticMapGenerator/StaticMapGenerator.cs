using Actively.Services.StaticMapGenerator.Configuration;
using Actively.Services.StaticMapGenerator.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Drawing;
using System.IO;

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
				//geojson.Position = 0;
				//string g = reader.ReadToEnd();
				//g = g.Replace("\n", "");
				//string g = "{\r\n\"type\":\"LineString\",\r\n\"coordinates\":[\r\n[20.9668484, 52.2663835],\r\n[20.9667964, 52.2663666],\r\n[20.9668411, 52.266378]\r\n]\r\n}";
				//string g = "{\r\n\"type\":\"LineString\",\r\n\"coordinates\":[\r\n[21.016285, 52.215316],\r\n[21.316112, 53.315620],\r\n[21.015967, 54.515872],\r\n[21.616418, 56.215954],\r\n[21.217033, 62.816089]\r\n]\r\n}";
				string g = "{\r\n\"type\":\"LineString\",\r\n\"coordinates\":[\r\n[21.016285, 52.215316],\r\n[21.016112, 52.215620],\r\n[21.015967, 52.215872],\r\n[21.016418, 52.215954],\r\n[21.017033, 52.216089]\r\n]\r\n}";
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
