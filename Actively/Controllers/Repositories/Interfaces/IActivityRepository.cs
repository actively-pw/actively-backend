using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyFitBook.Models;
using MyFitBook.Models.DTOs;
using MyFitBook.Models.DTOs.Statistics;
using MyFitBook.Models.Enums;

namespace MyFitBook.Controllers.Repositories.Interfaces
{
	/// <summary>
	/// Interface for classes used for database management and queries related to activities
	/// </summary>
	public interface IActivityRepository
	{
		Task<List<Activity>> GetActivitiesByUserId(Guid userId);
		Task<Activity> AddActivity(AddActivityDto addActivityDto, ActivityStatistics statistics, Guid userId);
		Task<Activity?> GetActivityById(Guid id);
		Task<Activity> DeleteActivity(Guid id);
		Task<Activity> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc);
		Task<List<Activity>> GetActivitiesBySport(Sport sport, Guid userId);
		Task<List<Activity>> GetLatestActivitiesByDaysCountAndSport(int daysCount, Sport sport, Guid userId);
	}
}
