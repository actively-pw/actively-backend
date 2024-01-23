using Microsoft.EntityFrameworkCore;
using MyFitBook.Models;

namespace MyFitBook.Context
{
	/// <summary>
	/// Class that models My FitBook database
	/// </summary>
	public class MyFitBookDbContext : DbContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MyFitBookDbContext"/> class.
		/// </summary>
		/// <param name="options"></param>
		public MyFitBookDbContext(DbContextOptions options) : base(options)
		{
		}

		/// <summary>
		/// Property that models Activities table in Actively database
		/// </summary>
		public DbSet<Activity> Activities { get; set; }

		/// <summary>
		/// Property that models Users table in Actively database
		/// </summary>
		public DbSet<User> Users { get; set; }

		/// <summary>
		/// Property that models RefreshTokens table in Actively database
		/// </summary>
		public DbSet<UserRefreshToken> RefreshTokens { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Activity>().Property(a => a.AverageSpeed).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.MaxSpeed).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.SumOfAscent).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.SumOfDescent).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.Distance).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.TotalTime).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.StartLongitude).HasDefaultValue(0);
			modelBuilder.Entity<Activity>().Property(a => a.StartLatitude).HasDefaultValue(0);
		}
	}
}
