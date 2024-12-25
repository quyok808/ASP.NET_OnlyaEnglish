using BE_Mobile.Models;
using BE_Mobile.Models.VocabularyApi.Models;
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
		public DbSet<Word> Words { get; set; }
		public DbSet<Example> Examples { get; set; }
	}
}
