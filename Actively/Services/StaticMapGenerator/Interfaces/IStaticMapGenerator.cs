using MyFitBook.Models.DTOs;

namespace MyFitBook.Services.StaticMapGenerator.Interfaces
{
	/// <summary>
	/// Interface for classes that generate static maps based on geojsons
	/// </summary>
	public interface IStaticMapGenerator
	{
		Task<StaticMapsDto> Generate(MemoryStream geojson, bool encoded);
	}
}
