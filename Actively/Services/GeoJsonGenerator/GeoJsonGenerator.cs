using Actively.Models.DTOs;
using Actively.Services.GeoJsonGenerator.Interfaces;

namespace Actively.Services.GeoJsonGenerator
{
	public class GeoJsonGenerator : IGeoJsonGenerator
	{
		public MemoryStream Generate(AddActivityDto addActivityDto)
		{
			//simplify geojson

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);

			writer.Write("{\n\"type\":\"LineString\",\n\"coordinates\":\n[\n");

			foreach (var slice in addActivityDto.Route)
			{
				for(int i=0; i<slice.Locations.Length; i++)
				{
					writer.Write("[");
					writer.Write(slice.Locations[i].Longitude);
					writer.Write(", ");
					writer.Write(slice.Locations[i].Latitude);
					writer.Write("]");

					if (i < slice.Locations.Length - 1)
					{
						writer.Write(",\n");
					}
					else
					{
						writer.Write("\n");
					}
				}
			}

			writer.Write("]\n}");

			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		// Douglas-Peucker Line Approximation Algorithm
		private List<(double longitude, double latitude)> Simplify(RouteSlice[] route)
		{
			List<(double longitude, double latitude)> simplified = new();

			return simplified;
		}
	}
}
