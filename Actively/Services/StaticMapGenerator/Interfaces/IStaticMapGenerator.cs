using Actively.Models.DTOs;

namespace Actively.Services.StaticMapGenerator.Interfaces
{
	public interface IStaticMapGenerator
	{
		Task<StaticMapsDto> Generate(MemoryStream geojson, bool encoded);
	}
}
