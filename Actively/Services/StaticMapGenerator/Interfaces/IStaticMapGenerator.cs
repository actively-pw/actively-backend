namespace Actively.Services.StaticMapGenerator.Interfaces
{
	public interface IStaticMapGenerator
	{
		Task<MemoryStream> Generate(MemoryStream geojson);
	}
}
