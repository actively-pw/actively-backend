using MyFitBook.Models.Enums;

namespace MyFitBook.Models.DTOs.Statistics
{
	public class SportSummaryDto
	{
		public string Sport { get; set; }
		public WeeklyStatisticsDto Weekly { get; set; }
		public YearToDateStatisticsDto YearToDate { get; set; }
		public AllTimeStatisticsDto AllTime { get; set; }
		public SportSummaryDto(Sport sport, (WeeklyStatisticsDto, YearToDateStatisticsDto, AllTimeStatisticsDto) statistics)
		{
			Sport = SportConverter.SportToString(sport);
			Weekly = statistics.Item1;
			YearToDate = statistics.Item2;
			AllTime = statistics.Item3;
		}

	}
}
