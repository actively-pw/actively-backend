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
	}
}
