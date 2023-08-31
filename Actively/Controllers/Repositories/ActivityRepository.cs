using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
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
			return await _context.Activities.ToListAsync();
		}

		public async Task<Activity> AddActivity(AddActivityDto addActivityDto)
		{
			var activity = new Activity(addActivityDto);
			await _context.Activities.AddAsync(activity);
			await _context.SaveChangesAsync();
			return activity;
		}

		public async Task<Activity?> GetActivityById(Guid id)
		{
			return await _context.Activities.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<Activity> DeleteActivity(Guid id)
		{
			var result = _context.Activities.FirstOrDefault(a => a.Id == id);
			if (result is null) throw new KeyNotFoundException("Activity with given id does not exist.");
			_context.Activities.Remove(result);
			await _context.SaveChangesAsync();
			return result;
		}
	}
}
