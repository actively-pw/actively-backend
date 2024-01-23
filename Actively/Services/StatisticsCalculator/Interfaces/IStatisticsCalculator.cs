using Actively.Models;
using Actively.Models.DTOs;
using Actively.Models.DTOs.Statistics;

namespace Actively.Services.StatisticsCalculator.Interfaces
{
	/// <summary>
	/// Interface for classes that can be used for activity and summary statistics calculation
	/// </summary>
    public interface IStatisticsCalculator
	{
		ActivityStatistics Calculate(AddActivityDto addActivityDto);
		(WeeklyStatisticsDto, YearToDateStatisticsDto, AllTimeStatisticsDto) CalculateSportSummary(List<Activity> weekActivities,
			List<Activity> yearActivities, List<Activity> allActivities);
	}
}
