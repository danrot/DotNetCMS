namespace DotNetCMS.Persistence.EntityFrameworkCore;

using DotNetCMS.Domain.Pages;
using Microsoft.EntityFrameworkCore;

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
