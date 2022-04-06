namespace DotNetCMS.Persistence.Test.Pages;

using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.EntityFrameworkCore;
using DotNetCMS.Persistence.EntityFrameworkCore.Pages;
using Microsoft.EntityFrameworkCore;
using System;

public abstract class EntityFrameworkCorePageRepositoryTest : PageRepositoryTest, IDisposable
{
	private readonly DbContextOptions<CmsContext> _options;

	private CmsContext _context;

	protected EntityFrameworkCorePageRepositoryTest(DbContextOptions<CmsContext> options)
	{
		_options = options;
		_context = new CmsContext(_options);

		_context.Database.EnsureDeleted();
		_context.Database.EnsureCreated();
	}

	protected override void SaveChanges()
	{
		_context.SaveChanges();
	}

	protected override IPageRepository CreatePageRepository()
	{
		return new PageRepository(_context);
	}

	protected override void Clear()
	{
		_context.Dispose();
		_context = new CmsContext(_options);
	}

	public void Dispose()
	{
		_context.Dispose();
	}
}
