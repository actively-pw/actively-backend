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
		public async Task<List<Activity>> GetAllActivities()
		{
			return await _context.Activities
				.ToListAsync();
		}
	}
}
