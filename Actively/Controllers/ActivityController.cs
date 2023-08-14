using Microsoft.AspNetCore.Mvc;

namespace Actively.Controllers
{
	[Route("Activities")]
	[ApiController]
	public class ActivityController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
