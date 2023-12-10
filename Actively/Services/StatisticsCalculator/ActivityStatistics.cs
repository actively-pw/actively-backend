namespace Actively.Services.StatisticsCalculator
{
	public class ActivityStatistics
	{
		public double Distance { get; }
		public long Duration { get; }
		public double AvgSpeed { get; }
		public double MaxSpeed { get; }
		public int SumOfAscent { get; }
		public int SumOfDescent { get; }

		public ActivityStatistics(double distance, long duration, double avgSpeed, double maxSpeed, int sumOfAscent, int sumOfDescent)
		{
			Distance = distance;
			Duration = duration;
			AvgSpeed = avgSpeed;
			MaxSpeed = maxSpeed;
			SumOfAscent = sumOfAscent;
			SumOfDescent = sumOfDescent;
		}
	}
}
