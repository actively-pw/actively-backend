using Actively.Models;
using Microsoft.EntityFrameworkCore;

namespace Actively.Context
{
	public class ActivelyDbContext : DbContext
	{
		public ActivelyDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Activity> Activities { get; set; }
		public DbSet<User> Users { get; set; }
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
