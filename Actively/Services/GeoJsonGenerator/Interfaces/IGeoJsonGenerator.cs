using Actively.Models.DTOs;

namespace Actively.Services.GeoJsonGenerator.Interfaces
{
	public interface IGeoJsonGenerator
	{
		MemoryStream Generate(AddActivityDto addActivityDto, out bool encoded);
	}
}
