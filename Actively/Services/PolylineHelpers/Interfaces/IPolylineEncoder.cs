namespace MyFitBook.Services.PolylineHelpers.Interfaces
{
	/// <summary>
	/// Interface for classes that can encode polylines
	/// </summary>
	public interface IPolylineEncoder
	{
		string EncodePolyline(List<(double x, double y)> polyline);
	}
}
