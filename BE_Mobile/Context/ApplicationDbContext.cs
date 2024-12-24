using BE_Mobile.Models;
using Microsoft.EntityFrameworkCore;

namespace BE_Mobile.Context
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Product> Products { get; set; }
		public DbSet<User> Users { get; set; }
	}
}
