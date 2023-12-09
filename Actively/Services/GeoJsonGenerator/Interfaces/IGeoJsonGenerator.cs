using Actively.Models.DTOs;

namespace Actively.Services.GeoJsonGenerator.Interfaces
{
	public interface IGeoJsonGenerator
	{
		(MemoryStream geojson, MemoryStream encodedPolyline) Generate(AddActivityDto addActivityDto, out bool encoded);
	}
}
