using Actively.Models.DTOs;

namespace Actively.Services.StatisticsCalculator.Interfaces
{
	public interface IStatisticsCalculator
	{
		ActivityStatistics Calculate(AddActivityDto addActivityDto);
	}
}
