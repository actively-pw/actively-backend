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
        public async Task GetAllActivities_ReturnsGetActivityDtoList()
        {
			//arrange
			PaginationParams @params = A.Dummy<PaginationParams>();

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator);

            //act
            var result = await controller.GetAllActivities(@params);

            //assert
            Assert.NotNull(result);
            Assert.IsType<ActionResult<List<GetActivityDto>>>(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(0)]
        public async Task GetAllActivities_ItemsPerPageLessThanActivitiesCount_ReturnsAmoutOfItemsEqualToItemsPerPage(int itemsPerPage)
        {
			//arrange
			PaginationParams @params = new PaginationParams()
            {
                Page=1,
                ItemsPerPage = itemsPerPage
            };
            List<Activity> activitiesList = new List<Activity>();

            for(int i=0; i<itemsPerPage+10; i++)
            {
                activitiesList.Add(A.Fake<Activity>());
            }

            A.CallTo(() => _activityRepository.GetAllActivities()).Returns(Task.FromResult(activitiesList));

            HttpResponse response = A.Fake<HttpResponse>();
            HeaderDictionary header = A.Fake<HeaderDictionary>();

			var controller = new ActivityController(_activityRepository, _blobStorage, _geoJsonGenerator);

			controller.ControllerContext = A.Dummy<ControllerContext>();
			controller.ControllerContext.HttpContext = A.Dummy<HttpContext>();

			A.CallTo(() => controller.ControllerContext.HttpContext.Response.Headers).Returns(header);

			//act
		    var result = await controller.GetAllActivities(@params);

            //assert
            var objectResult = (ObjectResult)result.Result;
            var enumerable = objectResult.Value as IEnumerable<GetActivityDto>;

            Assert.NotNull(enumerable);
            Assert.True(enumerable.Count()==itemsPerPage);
		}
    }
}
