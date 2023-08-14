using Actively.Controllers.Repositories.Interfaces;
using Actively.Models.DTOs;
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

		[HttpGet("")]
		public async Task<ActionResult<List<GetActivityDto>>> GetAllActivities()
		{
			try
			{
				var activities = await _activityRepository.GetAllActivities();
				var enumerable = activities.ToList();
				if (!enumerable.Any()) return NotFound();
				var activitiesList = enumerable.Select(a => new GetActivityDto(a));
				return Ok(activitiesList);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get Activities: {ex.Message}");
			}
		}
	}
}
