using Actively.Models;
using Actively.Models.DTOs;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IActivityRepository
	{
		Task<List<Activity>> GetAllActivities();
		Task<Activity> AddActivity(AddActivityDto addActivityDto);
	}
}
