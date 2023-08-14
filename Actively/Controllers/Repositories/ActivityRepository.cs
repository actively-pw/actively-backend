using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Microsoft.EntityFrameworkCore;

namespace Actively.Controllers.Repositories
{
	public class ActivityRepository : IActivityRepository
	{
		private readonly ActivelyDbContext _context;
		public ActivityRepository(ActivelyDbContext context)
		{
			_context = context;
		}
		public List<Activity> GetActivitiesByUser(Guid userId)
		{
			return new List<Activity>();
		}
	}
}
