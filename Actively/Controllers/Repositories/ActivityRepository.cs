using Actively.Context;
using Actively.Controllers.Repositories.Interfaces;
using Actively.Models;
using Actively.Models.DTOs;
using Actively.Services.StatisticsCalculator;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
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

		public async Task<List<Activity>> GetActivitiesByUserId(Guid userId)
		{
			return await _context.Activities.Where(a => a.User.Id == userId).ToListAsync();
		}

		public async Task<Activity> AddActivity(AddActivityDto addActivityDto, ActivityStatistics statistics)
		{
			var activity = new Activity(addActivityDto, statistics);
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

		public async Task<Activity> EditActivity(Guid id, [FromBody] JsonPatchDocument<Activity> patchDoc)
		{
			var result = await _context.Activities.SingleOrDefaultAsync(e => e.Id == id);

			if (result is null) throw new KeyNotFoundException("Activity with given id does not exist.");

			foreach (var operation in patchDoc.Operations) // check if all changes are valid
			{
				if (operation.OperationType != OperationType.Replace)
				{
					throw new ArgumentException("Invalid operation - only Replace operations are allowed.");
				}

				if (operation.path.Contains("id") || operation.path.Contains("Id"))
				{
					throw new ArgumentException("Invalid operation - cannot modify identificators.");
				}

				if(!operation.path.Contains("Title") && !operation.path.Contains("title"))
				{
					throw new ArgumentException("Invalid operation - only Title can be modified (at least for now).");
				}
			}

			patchDoc.ApplyTo(result);

			await _context.SaveChangesAsync();

			return result;
		}

		public int GetActivitiesCount()
		{
			return _context.Activities.Count();
		}
	}
}
