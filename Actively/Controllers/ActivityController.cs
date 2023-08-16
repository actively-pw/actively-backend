using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
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

		[HttpGet]
		public async Task<ActionResult<List<GetActivityDto>>> GetAllActivities([FromQuery] PaginationParams @params)
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

		[HttpPost]
		public async Task<ActionResult<Activity>> AddActivity(AddActivityDto addActivityDto)
		{
			try
			{
				var result = await _activityRepository.AddActivity(addActivityDto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to add new activity. Exception {ex.Message}");
			}
		}
	}
}
