using DotNetCMS.Domain.Pages;
using Microsoft.EntityFrameworkCore;

namespace DotNetCMS.Persistence.EntityFrameworkCore
{
	public sealed class CmsContext : DbContext
	{
		public DbSet<Page> Pages => Set<Page>();

		public CmsContext(DbContextOptions<CmsContext> options) : base(options) {}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Page>()
				.HasKey(page => page.Id);
		}
	}
}
