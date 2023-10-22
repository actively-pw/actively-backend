using Actively.Services.StaticMapGenerator.Configuration;
using Actively.Services.StaticMapGenerator.Interfaces;

namespace Actively.Services.StaticMapGenerator
{
	public class StaticMapGenerator : IStaticMapGenerator
	{
		private readonly MapBoxConfig _config;
		public async Task<MemoryStream> Generate(MemoryStream geojson)
		{
			using(var client = new HttpClient())
			{
				string url = "mapbox/streets-v12/static/geojson(%7B%22type%22%3A%22Point%22%2C%22coordinates%22%3A%5B-73.99%2C40.7%5D%7D)/-73.99,40.70,12/500x300?access_token=pk.eyJ1IjoiYWN0aXZlbHliYWNrZW5kIiwiYSI6ImNsbzBjYjdvdzBoYWQya3F1eTBkamdiODkifQ.O_3zKmq1mXYhZcuyXKeH8w";
				var response = await client.GetAsync(Path.Combine(_config.BaseUrl, url));
				var stream = new MemoryStream();
				var writer = new StreamWriter(stream);
				writer.Write(response.Content);
				return stream;
			}
		}
	}
}
