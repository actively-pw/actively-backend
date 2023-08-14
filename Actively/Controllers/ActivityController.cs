using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers
{
	[Route("Activities")]
	[ApiController]
	public class ActivityController : Controller
	{
		private readonly IActivityRepository _activityRepository;
		public ActivityController(IActivityRepository activityRepository)
		{
			_activityRepository = activityRepository;
		}

		[HttpGet("activitiesByUser/{userId}")]
		public List<Activity> GetActivitiesByUser(Guid userId)
		{
			return _activityRepository.GetActivitiesByUser(userId);
		}
	}
}
