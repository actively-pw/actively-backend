using Actively.BlobStorage;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.GeoJsonGenerator;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers
{
	[Route("Activities")]
	[ApiController]
	public class ActivityController : Controller
	{
		private readonly IActivityRepository _activityRepository;
		private readonly StorageManager _blobStorage;
		private readonly GeoJsonGenerator _geoJsonGenerator;
		public ActivityController(IActivityRepository activityRepository, StorageManager blobStorage, GeoJsonGenerator geoJsonGenerator)
		{
			_activityRepository = activityRepository;
			_blobStorage = blobStorage;
			_geoJsonGenerator = geoJsonGenerator;
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
				if (await _activityRepository.GetActivityById(addActivityDto.Id) is not null)
				{
					return BadRequest("Activity with this id already exists.");
				}

				var result = await _activityRepository.AddActivity(addActivityDto);

				using(var geojson = _geoJsonGenerator.Generate(addActivityDto))
				{
					await _blobStorage.Upload(addActivityDto.Id, geojson);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to add new activity. Exception {ex.Message}");
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<Activity>> DeleteActivity(Guid id)
		{
			try
			{

				await _blobStorage.Delete(id); // delete route file from blob storage

				var result = await _activityRepository.DeleteActivity(id); // delete activity from db

				return result;
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound($"Exception: {ex.Message}");
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to delete activity with id {id}. Exception {ex.Message}");
			}
		}
	}
}
