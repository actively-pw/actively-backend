using Actively.Models;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IActivityRepository
	{
		Task<List<Activity>> GetAllActivities();
	}
}
