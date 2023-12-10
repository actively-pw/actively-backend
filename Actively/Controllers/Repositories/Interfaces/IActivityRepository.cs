using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.StatisticsCalculator;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IActivityRepository
	{
		Task<List<Activity>> GetAllActivities();
		Task<Activity> AddActivity(AddActivityDto addActivityDto, ActivityStatistics statistics);
		Task<Activity?> GetActivityById(Guid id);
		Task<Activity> DeleteActivity(Guid id);
		Task<Activity> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc);
		int GetActivitiesCount();
	}
}
