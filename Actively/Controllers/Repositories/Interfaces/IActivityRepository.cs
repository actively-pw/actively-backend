using Actively.Models;
using Actively.Models.DTOs;
using Actively.Models.DTOs.Statistics;
using Actively.Models.Enums;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers.Repositories.Interfaces
{
    public interface IActivityRepository
	{
		Task<List<Activity>> GetActivitiesByUserId(Guid userId);
		Task<Activity> AddActivity(AddActivityDto addActivityDto, ActivityStatistics statistics, Guid userId);
		Task<Activity?> GetActivityById(Guid id);
		Task<Activity> DeleteActivity(Guid id);
		Task<Activity> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc);
		int GetActivitiesCount();
		Task<List<Activity>> GetActivitiesBySport(Sport sport, Guid userId);
		Task<List<Activity>> GetLatestActivitiesByDaysCountAndSport(int daysCount, Sport sport, Guid userId);
	}
}
