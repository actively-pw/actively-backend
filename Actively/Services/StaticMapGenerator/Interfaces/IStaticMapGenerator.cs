namespace Actively.Services.StaticMapGenerator.Interfaces
{
	public interface IStaticMapGenerator
	{
		Task<Stream> Generate(MemoryStream geojson);
	}
}
