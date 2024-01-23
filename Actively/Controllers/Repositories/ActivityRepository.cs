using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFitBook.Context;
using MyFitBook.Controllers.Repositories.Interfaces;
using MyFitBook.Models;
using MyFitBook.Models.DTOs;
using MyFitBook.Models.DTOs.Statistics;
using MyFitBook.Models.Enums;

namespace MyFitBook.Controllers.Repositories
{
	/// <summary>
	/// Class that contains operations related to database queries about activities
	/// </summary>
	public class ActivityRepository : IActivityRepository
	{
		private readonly MyFitBookDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActivityRepository"/> class.
		/// </summary>
		/// <param name="context"></param>
		public ActivityRepository(MyFitBookDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Returns list of activities recorded by user whose <c>userId</c> is provided as the argument
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Activity>> GetActivitiesByUserId(Guid userId)
		{
			return await _context.Activities.Where(a => a.User.Id == userId).ToListAsync();
		}

		/// <summary>
		/// Adds new activity recorded by user whose Id is equal to <c>userId</c>
		/// </summary>
		/// <param name="addActivityDto"></param>
		/// <param name="statistics"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public async Task<Activity> AddActivity(AddActivityDto addActivityDto, ActivityStatistics statistics, Guid userId)
		{
			var user = _context.Users.Find(userId);

			if (user is null) throw new ArgumentException($"User with id {userId} does not exist");

			var activity = new Activity(addActivityDto, statistics, user);
			await _context.Activities.AddAsync(activity);
			await _context.SaveChangesAsync();
			return activity;
		}

		/// <summary>
		/// Returns activity with Id equal to given argument
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Activity?> GetActivityById(Guid id)
		{
			return await _context.Activities.FirstOrDefaultAsync(a => a.Id == id);
		}

		/// <summary>
		/// Deletes activity with Id equal to given argument
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		public async Task<Activity> DeleteActivity(Guid id)
		{
			var result = _context.Activities.FirstOrDefault(a => a.Id == id);

			if (result is null) throw new KeyNotFoundException("Activity with given id does not exist.");

			_context.Activities.Remove(result);
			await _context.SaveChangesAsync();
			return result;
		}

		/// <summary>
		/// Edits activity with Id equal to <c>id</c>
		/// </summary>
		/// <param name="id"></param>
		/// <param name="patchDoc"></param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException"></exception>
		/// <exception cref="ArgumentException"></exception>
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

				if (!operation.path.Contains("Title") && !operation.path.Contains("title"))
				{
					throw new ArgumentException("Invalid operation - only Title can be modified (at least for now).");
				}
			}

			patchDoc.ApplyTo(result);

			await _context.SaveChangesAsync();

			return result;
		}

		/// <summary>
		/// Returns total activities count
		/// </summary>
		/// <returns></returns>
		public int GetActivitiesCount()
		{
			return _context.Activities.Count();
		}

		/// <summary>
		/// Returns acitivities of given type <c>sport</c> recorded by user with Id equal to <c>userId</c>
		/// </summary>
		/// <param name="sport"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Activity>> GetActivitiesBySport(Sport sport, Guid userId)
		{
			return await _context.Activities.Where(a => a.User.Id == userId && a.Sport == sport).ToListAsync();
		}

		/// <summary>
		/// Returns acitivities of given type <c>sport</c>
		/// recorded by user with Id equal to <c>userId</c> in the last <c>daysCount</c> days
		/// </summary>
		/// <param name="daysCount"></param>
		/// <param name="sport"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<List<Activity>> GetLatestActivitiesByDaysCountAndSport(int daysCount, Sport sport, Guid userId)
		{
			return await _context.Activities
				.Where(a =>
					a.User.Id == userId
					&& a.Sport == sport
					&& a.Start >= DateTime.Today.AddDays(-daysCount))
				.ToListAsync();
		}
	}
}
