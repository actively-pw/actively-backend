using Actively.Models.DTOs;

namespace Actively.Services.GeoJsonGenerator.Interfaces
{
	/// <summary>
	/// Interface for classes that generate geoJSON files based on <c>AddActivityDto</c> values
	/// </summary>
	public interface IGeoJsonGenerator
	{
		(MemoryStream geojson, MemoryStream? encodedPolyline) Generate(AddActivityDto addActivityDto, out bool encoded);
	}
}
