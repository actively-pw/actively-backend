using Actively.Models.DTOs;
using Actively.Services.GeoJsonGenerator.Interfaces;

namespace Actively.Services.GeoJsonGenerator
{
	public class GeoJsonGenerator : IGeoJsonGenerator
	{
		public MemoryStream Generate(AddActivityDto addActivityDto)
		{
			// convert addActivityDto.Route to list of points
			List<(double X, double Y)> points = new();
			foreach(var slice in addActivityDto.Route)
			{
				foreach(var location in slice.Locations)
				{
					points.Add((Math.Round(location.Longitude, 5), Math.Round(location.Latitude, 5)));
				}
			}

			//simplify geojson if totalPointsCount is big
			if(points.Count > 100) // Todo: better values of precision
			{
				points = Simplify(points, 0.000001);
			}

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);

			writer.Write("{\n\"type\":\"LineString\",\n\"coordinates\":\n[\n");

			for(int i=0; i< points.Count; i++)
			{
				writer.Write("[");
				writer.Write(points[i].X);
				writer.Write(", ");
				writer.Write(points[i].Y);
				writer.Write("]");

				if (i < points.Count - 1)
				{
					writer.Write(",\n");
				}
				else
				{
					writer.Write("\n");
				}
			}

			writer.Write("]\n}");

			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		// Douglas-Peucker Line Approximation Algorithm
		private List<(double X, double Y)> Simplify(List<(double X, double Y)> points, double tolerance)
		{
			if (points is null || points.Count < 3) return points;

			int firstPoint = 0;
			int lastPoint = points.Count - 1;
			List<int> pointIndicesToKeep = new List<int>()
			{
				firstPoint,
				lastPoint
			};

			// the first and last point cannot be the same
			while (points[firstPoint].Equals(points[lastPoint]))
			{
				lastPoint--;
			}

			DouglasPeuckerReduction(points, firstPoint, lastPoint, tolerance, ref pointIndicesToKeep);

			List<(double X, double Y)> simplified = new();
			pointIndicesToKeep.Sort();
            foreach (var index in pointIndicesToKeep)
            {
				simplified.Add(points[index]);
            }

            return simplified;
		}

		private void DouglasPeuckerReduction(List<(double X, double Y)> points, int firstPoint, int lastPoint, double tolerance, ref List<int> pointIndicesToKeep)
		{
			double maxDistance = 0;
			int indexFurthest = 0;
			for(int i=firstPoint; i<lastPoint; i++)
			{
				double distance = PerpendicularDistance(points[firstPoint], points[lastPoint], points[i]);
				if(distance > maxDistance)
				{
					maxDistance = distance;
					indexFurthest = i;
				}
			}

			if(maxDistance > tolerance && indexFurthest!=0)
			{
				// add the largest point that exceeds the tolerance
				pointIndicesToKeep.Add(indexFurthest);
				DouglasPeuckerReduction(points, firstPoint, indexFurthest, tolerance, ref pointIndicesToKeep);
				DouglasPeuckerReduction(points, indexFurthest, lastPoint, tolerance, ref pointIndicesToKeep);
			}
		}

		//distamce of a point from a line made from point1 and point2
		private double PerpendicularDistance((double X, double Y) point1, (double X, double Y) point2, (double X, double Y) point)
		{
			double area = Math.Abs(0.5 * (point1.X * point2.Y + point2.X * point.Y + point.X * point1.Y - point2.X * point1.Y - point.X * point2.Y - point1.X * point.Y));
			double bottom = Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
			double height = area / bottom * 2;
			return height;
		}
	}
}
