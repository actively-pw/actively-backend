using Actively.Services.PolylineHelpers.Interfaces;
using System.Text;
using System.Web;

namespace Actively.Services.PolylineHelpers
{
	public class PolylineEncoder : IPolylineEncoder
	{
		public string EncodePolyline(List<(double x, double y)> polyline)
		{
			List<(double x, double y)> polylineCopy = new();

			for (int i=0; i<polyline.Count; i++)
			{
				polylineCopy.Add((polyline[i].y, polyline[i].x));
			}

			var builder = new StringBuilder();

			List<(int x, int y)> pointsAfterStep12 = polylineCopy
				.Select(tuple => ((int)(tuple.x * 1e5), (int)(tuple.y * 1e5)))
				.ToList();

			var prev = pointsAfterStep12[0];

			builder.Append($"{EncodeInt(prev.x)}{EncodeInt(prev.y)}");

			for (int i = 1; i < pointsAfterStep12.Count; i++)
			{
				var current = pointsAfterStep12[i];
				var xDiff = current.x - prev.x;
				var yDiff = current.y - prev.y;
				builder.Append($"{EncodeInt(xDiff)}{EncodeInt(yDiff)}");
				prev = current;
			}

			return HttpUtility.UrlEncode(builder.ToString());
		}

		protected string EncodeInt(int value)
		{
			var shifted = value << 1;
			var step5 = Step5(shifted);
			var chunks = Steps78(To5BitChunks(step5));
			return ToAsciiString(chunks);
		}

		protected int TwosComplement(int value) => ~Math.Abs(value) + 1;

		protected int Step5(int value) => value < 0 ? ~value : value;

		protected int[] To5BitChunks(int value)
		{
			var chunks = new int[6];
			for (int i = 0; i < chunks.Length; i++)
			{
				var shift = (chunks.Length - 1 - i) * 5;
				chunks[i] = (value & (0b_11111 << shift)) >> shift;
			}

			return chunks;
		}

		protected int[] Steps78(int[] values)
		{
			var chunks = values.Reverse().ToArray();
			for (int i = 0; i < chunks.Length - 1; i++)
			{
				chunks[i] |= 0x20;
			}

			return chunks;
		}

		protected string ToAsciiString(int[] values)
		{
			var builder = new StringBuilder();
			for (int i = 0; i < values.Length; i++)
			{
				values[i] += 63;
				var c = (char)values[i];
				builder.Append(c == 92 ? "\\" : c); // check for \ character
			}

			return builder.ToString();
		}
	}
}
