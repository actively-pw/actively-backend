namespace MyFitBook.Models.DTOs.Statistics
{
	public class YearToDateStatisticsDto : StatisticsDto
	{
		public long Time { get; set; }
		public int ElevationGain { get; set; }
	}
}
