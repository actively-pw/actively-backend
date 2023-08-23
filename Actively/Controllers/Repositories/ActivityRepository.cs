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
			var activity = new Activity(addActivityDto);

			await _context.Activities.AddAsync(activity);
			await _context.SaveChangesAsync();

			// generate geojson file and add it to blob storage

			return activity;
		}
	}
}
