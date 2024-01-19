using Actively.BlobStorage;
using Actively.BlobStorage.Interfaces;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Models.DTOs.Statistics;
using Actively.Models.Enums;
using Actively.Services.AuthService.Interfaces;
using Actively.Services.GeoJsonGenerator.Interfaces;
using Actively.Services.StaticMapGenerator.Interfaces;
using Actively.Services.StatisticsCalculator.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Actively.Controllers
{
	[Authorize]
    [Route("Activities")]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[ApiController]
	public class ActivityController : Controller
	{
		private readonly IActivityRepository _activityRepository;
		private readonly IStorageManager _blobStorage;
		private readonly IGeoJsonGenerator _geoJsonGenerator;
		private readonly IStaticMapGenerator _staticMapGenerator;
		private readonly IStatisticsCalculator _statisticsCalculator;
		private readonly ITokenService _tokenService;
		public ActivityController(IActivityRepository activityRepository, IStorageManager blobStorage, IGeoJsonGenerator geoJsonGenerator,
			IStaticMapGenerator staticMapGenerator, IStatisticsCalculator statisticsCalculator, ITokenService tokenService)
		{
			_activityRepository = activityRepository;
			_blobStorage = blobStorage;
			_geoJsonGenerator = geoJsonGenerator;
			_staticMapGenerator = staticMapGenerator;
			_statisticsCalculator = statisticsCalculator;
			_tokenService = tokenService;
		}
		/// <summary>
		/// Returns a list of activities that have been recorded by a user who is the owner of provided JWT
		/// </summary>
		/// <param name="staticMapType"></param>
		/// <param name="params"></param>
		/// <returns>a list of user's activities</returns>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<List<GetActivityDto>>> GetActivitiesByUserId([FromHeader(Name = "staticMapType")] string staticMapType, [FromQuery] PaginationParams @params)
		{
			try
			{
				var accessToken = await HttpContext.GetTokenAsync("access_token");
				var jwt = _tokenService.GetJwt(accessToken);
				var userId = jwt.Claims.First().Value;

				var activities = await _activityRepository.GetActivitiesByUserId(new Guid(userId));
				var enumerable = activities.ToList();
				enumerable.Sort((a, b) => b.Start.CompareTo(a.Start));

				StaticMap type;
				switch(staticMapType)
				{
					case "web":
						type = StaticMap.Web;
						break;
					case "mobile":
						type = StaticMap.Mobile;
						break;
					default:
						return BadRequest("Invalid value for header \"staticMapType\"");
				}

				var activitiesList = enumerable
				.Select(a => new GetActivityDto(a, type))
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

		[HttpGet("{id}")]
		public async Task<ActionResult<GetActivityWithsStatisticsDto>> GetActivityById(Guid id, [FromHeader(Name = "staticMapType")] string staticMapType)
		{
			try
			{
				StaticMap type;
				switch (staticMapType)
				{
					case "web":
						type = StaticMap.Web;
						break;
					case "mobile":
						type = StaticMap.Mobile;
						break;
					default:
						return BadRequest("Invalid value for header \"staticMapType\"");
				}

				var activity = await _activityRepository.GetActivityById(id);

				if (activity is null) return NotFound();

				return new GetActivityWithsStatisticsDto(activity, type);
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to get activity with id {id}. Exception {ex.Message}");
			}
		}



		[HttpPost]
		public async Task<ActionResult> AddActivity(AddActivityDto addActivityDto)
		{
			try
			{
				int pointsCount = 0;
				foreach (var slice in addActivityDto.Route) pointsCount += slice.Locations.Length;
				if (pointsCount < 2)
				{
					return BadRequest("Route must consist of at least two points");
				}

				if (await _activityRepository.GetActivityById(addActivityDto.Id) is not null)
				{
					return BadRequest("Activity with this id already exists.");
				}

				ActivityStatistics statistics = _statisticsCalculator.Calculate(addActivityDto);

				var accessToken = await HttpContext.GetTokenAsync("access_token");
				var jwt = _tokenService.GetJwt(accessToken);
				var userId = jwt.Claims.FirstOrDefault().Value;

				var result = await _activityRepository.AddActivity(addActivityDto, statistics, new Guid(userId));

				(var geojson, var encodedPolyline) = _geoJsonGenerator.Generate(addActivityDto, out bool encoded);
				using (geojson)
				{
					await _blobStorage.Upload(addActivityDto.Id, BlobType.Geojson, geojson);

					var staticMapArgument = encoded ? encodedPolyline : geojson;
					using (var staticMaps = await _staticMapGenerator.Generate(staticMapArgument, encoded))
					{
						await _blobStorage.Upload(addActivityDto.Id, BlobType.StaticMapWebLight, staticMaps.WebLight);
						await _blobStorage.Upload(addActivityDto.Id, BlobType.StaticMapMobileLight, staticMaps.MobileLight);
						await _blobStorage.Upload(addActivityDto.Id, BlobType.StaticMapWebDark, staticMaps.WebDark);
						await _blobStorage.Upload(addActivityDto.Id, BlobType.StaticMapMobileDark, staticMaps.MobileDark);
					}
				}

				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest($"Failed to add new activity. Exception: {ex.Message}");
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<ActivityResponseDto>> DeleteActivity(Guid id)
		{
			try
			{
				await _blobStorage.DeleteActivityBlobs(id); // delete all files related to this activity from blob storage

				var result = await _activityRepository.DeleteActivity(id); // delete activity from db

				return new ActivityResponseDto(result);
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
		public async Task<ActionResult<ActivityResponseDto>> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc)
		{
			try
			{
				var result = await _activityRepository.EditActivity(id, patchDoc);
				return Ok(new ActivityResponseDto(result));
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
