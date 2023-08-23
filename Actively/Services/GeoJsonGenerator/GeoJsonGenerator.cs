using Actively.Models.DTOs;

namespace Actively.Services.GeoJsonGenerator
{
	public class GeoJsonGenerator
	{
		public MemoryStream Generate(AddActivityDto addActivityDto)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);

			writer.Write("{\n\"type\":\"LineString\",\n\"coordinates\":[\n[\n");

			foreach (var slice in addActivityDto.Route)
			{
				for(int i=0; i<slice.Locations.Length; i++)
				{
					writer.Write("[");
					writer.Write(slice.Locations[i].Latitude);
					writer.Write(", ");
					writer.Write(slice.Locations[i].Longitude);
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

			writer.Write("]\n]\n}");

			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}
