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
			return await _context.Activities
				.ToListAsync();
		}

		public async Task<Activity> AddActivity(AddActivityDto addActivityDto)
		{
			var activity = new Activity(
				addActivityDto.Title,
				addActivityDto.Type,
				addActivityDto.Start,
				addActivityDto.Stats.TotalTime,
				addActivityDto.Stats.Distance,
				addActivityDto.Stats.AverageSpeed);

			await _context.Activities.AddAsync(activity);
			await _context.SaveChangesAsync();
			return activity;
		}
	}
}
