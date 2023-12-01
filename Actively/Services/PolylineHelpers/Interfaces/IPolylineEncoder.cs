namespace Actively.Services.PolylineHelpers.Interfaces
{
	public interface IPolylineEncoder
	{
		string EncodePolyline(List<(double x, double y)> polyline);
	}
}
