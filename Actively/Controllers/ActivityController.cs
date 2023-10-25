using Actively.BlobStorage;
using Actively.BlobStorage.Interfaces;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.GeoJsonGenerator.Interfaces;
using Actively.Services.StaticMapGenerator.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;


namespace Actively.Controllers
{
	[Route("Activities")]
	[ApiController]
	public class ActivityController : Controller
	{
		private readonly IActivityRepository _activityRepository;
		private readonly IStorageManager _blobStorage;
		private readonly IGeoJsonGenerator _geoJsonGenerator;
		private readonly IStaticMapGenerator _staticMapGenerator;
		public ActivityController(IActivityRepository activityRepository, IStorageManager blobStorage, IGeoJsonGenerator geoJsonGenerator, IStaticMapGenerator staticMapGenerator)
		{
			_activityRepository = activityRepository;
			_blobStorage = blobStorage;
			_geoJsonGenerator = geoJsonGenerator;
			_staticMapGenerator = staticMapGenerator;
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<List<GetActivityDto>>> GetAllActivities([FromQuery] PaginationParams @params)
		{
			try
			{
				var activities = await _activityRepository.GetAllActivities();
				var enumerable = activities.ToList();
				enumerable.Sort((a, b) => b.Start.CompareTo(a.Start));

				if (!enumerable.Any()) return NotFound();

				var activitiesList = enumerable
					.Select(a => new GetActivityDto(a))
					.Skip((@params.Page - 1) * @params.ItemsPerPage)
					.Take(@params.ItemsPerPage);

				int totalPagesCount = (int)Math.Ceiling((double)_activityRepository.GetActivitiesCount() / @params.ItemsPerPage);

				int nextPage = @params.Page < totalPagesCount ? @params.Page + 1 : -1;

				Response.Headers.Add("nextPage", nextPage.ToString());

				return Ok(activitiesList);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get Activities: {ex.Message}");
			}
		}

		[HttpPost]
		[Authorize]
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
					await _blobStorage.Upload(addActivityDto.Id, BlobType.Geojson, geojson);

					using var staticMap = await _staticMapGenerator.Generate(geojson);

					await _blobStorage.Upload(addActivityDto.Id, BlobType.StaticMap, staticMap);
				}

				return Ok(result);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to add new activity. Exception {ex.Message}");
			}
		}

		[HttpDelete("{id}")]
		[Authorize]
		public async Task<ActionResult<Activity>> DeleteActivity(Guid id)
		{
			try
			{
				await _blobStorage.DeleteActivityBlobs(id); // delete all files related to this activity from blob storage

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
		[HttpPatch("{id}")]
		[Authorize]
		public async Task<ActionResult<Activity>> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc)
		{
			try
			{
				var result = await _activityRepository.EditActivity(id, patchDoc);
				return Ok(result);
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound($"Exception: {ex.Message}");
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to edit activity with id {id}. Exception {ex.Message}");
			}
		}
	}
}
