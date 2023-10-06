using Actively.BlobStorage.Interfaces;
using Actively.Controllers;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.GeoJsonGenerator.Interfaces;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Actively.Tests.Controllers
{
    public class ActivityControllerTests
    {
		private readonly IActivityRepository _activityRepository;
		private readonly IStorageManager _blobStorage;
		private readonly IGeoJsonGenerator _geoJsonGenerator;

        public ActivityControllerTests()
        {
            _activityRepository = A.Fake<IActivityRepository>();
            _blobStorage = A.Fake<IStorageManager>();
            _geoJsonGenerator = A.Fake<IGeoJsonGenerator>();
        }
		[Fact]
        public async Task GetAllActivities_ThereIsAtLeastOneActivity_ReturnsGetActivityDtoList()
        {
			//arrange
			PaginationParams @params = A.Dummy<PaginationParams>();
			List<Activity> activitiesList = new List<Activity>{ new Activity()};
			HttpResponse response = A.Fake<HttpResponse>();
			HeaderDictionary header = A.Fake<HeaderDictionary>();

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator);

			A.CallTo(() => _activityRepository.GetAllActivities()).Returns(Task.FromResult(activitiesList));

			controller.ControllerContext = A.Dummy<ControllerContext>();
			controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

			A.CallTo(() => controller.ControllerContext.HttpContext.Response.Headers).Returns(header);

			//act
			var result = await controller.GetAllActivities(@params);

			//assert
			var objectResult = (ObjectResult)result.Result;
			var enumerable = objectResult.Value as IEnumerable<GetActivityDto>;

			Assert.NotNull(objectResult);
            Assert.IsType<ActionResult<List<GetActivityDto>>>(result);
        }

        [Theory]
        [InlineData(1, 3, 5)]
		[InlineData(2, 3, 5)]
		[InlineData(2, 3, 10)]
		[InlineData(4, 3, 10)]
		[InlineData(5, 3, 10)]
        public async Task GetAllActivities_NaturalNumberPaginationParams_CanPaginate(int page, int itemsPerPage, int itemsCount)
        {
			//arrange
			PaginationParams @params = new PaginationParams()
			{
				Page = page,
				ItemsPerPage = itemsPerPage
			};

			List<Activity> activitiesList = new List<Activity>();
			for (int i = 0; i < itemsCount; i++)
			{
				activitiesList.Add(new Activity());
				activitiesList[i].Title = i.ToString();
			}

			HttpResponse response = A.Fake<HttpResponse>();
			HeaderDictionary header = A.Fake<HeaderDictionary>();

			A.CallTo(() => _activityRepository.GetAllActivities()).Returns(Task.FromResult(activitiesList));

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator);

			controller.ControllerContext = A.Dummy<ControllerContext>();
			controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

			A.CallTo(() => controller.ControllerContext.HttpContext.Response.Headers).Returns(header);

			//act
			var result = await controller.GetAllActivities(@params);

			//assert
			var objectResult = (ObjectResult)result.Result;
			var enumerable = objectResult.Value as IEnumerable<GetActivityDto>;

			if(page*itemsPerPage < itemsCount)
			{
				Assert.Equal(enumerable.Count(), itemsPerPage);
			}
			else
			{
				Assert.Equal(enumerable.Count(), itemsCount - (page-1)*itemsPerPage > 0 ? itemsCount - (page - 1) * itemsPerPage : 0);
			}
		}
    }
}
