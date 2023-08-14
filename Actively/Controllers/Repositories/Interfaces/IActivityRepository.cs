using Actively.Models;

namespace Actively.Controllers.Repositories.Interfaces
{
	public interface IActivityRepository
	{
		List<Activity> GetActivitiesByUser(Guid userId);
	}
}
