using Actively.Models.DTOs;
using Actively.Services.StaticMapGenerator.Configuration;
using Actively.Services.StaticMapGenerator.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Web;

namespace Actively.Services.StaticMapGenerator
{
	/// <summary>
	/// Helper class for generating Mapbox static maps
	/// </summary>
	public class StaticMapGenerator : IStaticMapGenerator
	{
		private readonly MapBoxConfig _config;
		private readonly int _webWidth = 1100; // in pixels
		private readonly int _webHeight = 500;
		private readonly int _mobileWidth = 1100;
		private readonly int _mobileHeight = 750;
		private readonly string _lightStyle = "outdoors-v11";
		private readonly string _darkStyle = "dark-v11";
		private readonly string _lightLineColor = "#374d2d";
		private readonly string _darkLineColor = "#fabc49";
		private readonly int _lineWidth = 9;

		/// <summary>
		/// Initializes a new instance of the <see cref="StaticMapGenerator"/> class.
		/// </summary>
		/// <param name="config"></param>
		public StaticMapGenerator(IOptions<MapBoxConfig> config)
		{
			_config = config.Value;
		}

		/// <summary>
		/// Generates Mapbox static maps based on provided geojson
		/// </summary>
		/// <param name="geojson"></param>
		/// <param name="encoded"></param>
		/// <returns></returns>
		public async Task<StaticMapsDto> Generate(MemoryStream geojson, bool encoded)
		{
			using (var reader = new StreamReader(geojson))
			{
				geojson.Position = 0;
				string g = reader.ReadToEnd();

				if (!encoded)
				{
					g = g.Replace("\n", "");
					g = g.Replace(" ", "");
				}

				Stream webLight = await GetStaticMap(g, _webWidth, _webHeight, encoded, false);
				Stream mobileLight = await GetStaticMap(g, _mobileWidth, _mobileHeight, encoded, false);
				Stream webDark = await GetStaticMap(g, _webWidth, _webHeight, encoded, true);
				Stream mobileDark = await GetStaticMap(g, _mobileWidth, _mobileHeight, encoded, true);

				return new StaticMapsDto() { MobileLight = mobileLight, WebLight = webLight, WebDark = webDark, MobileDark = mobileDark };
			}
		}

		/// <summary>
		/// Generates static map based on provided geojson with style defined by the provided arguments
		/// </summary>
		/// <param name="geojson"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="encoded"></param>
		/// <param name="darkMode"></param>
		/// <returns></returns>
		/// <exception cref="WebException"></exception>
		private async Task<Stream> GetStaticMap(string geojson, int width, int height, bool encoded, bool darkMode)
		{
			string styleName = darkMode? _darkStyle : _lightStyle;
			string lineColor = darkMode? _darkLineColor : _lightLineColor;

			string url = encoded ? $"mapbox/{styleName}/static/path-{_lineWidth}-{lineColor}({geojson})/auto/{width}x{height}?access_token={_config.StaticImagesToken}" :
				$"mapbox/{styleName}/static/geojson({AddColorToGeojson(geojson, lineColor)})/auto/{width}x{height}?access_token={_config.StaticImagesToken}";

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

		/// <summary>
		/// Adds line color information to provided geojson
		/// </summary>
		/// <param name="geojson"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		private string AddColorToGeojson(string geojson, string color)
		{
			geojson = "{\"type\":\"Feature\",\"geometry\":" + geojson;
			geojson += $",\"properties\":{{\"stroke\":\"{color}\",\"stroke-width\":{_lineWidth}}}}}";
			geojson = HttpUtility.UrlEncode(geojson);
			return geojson;
		}
	}
}
