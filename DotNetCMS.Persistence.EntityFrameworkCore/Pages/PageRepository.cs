using DotNetCMS.Domain.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCMS.Persistence.EntityFrameworkCore.Pages
{
	public sealed class PageRepository : IPageRepository
	{
		private readonly CmsContext _cmsContext;

		public PageRepository(CmsContext cmsContext)
		{
			_cmsContext = cmsContext;
		}

		public void Add(Page page)
		{
			_cmsContext.Pages.Add(page);
		}

		public void Remove(Page page)
		{
			_cmsContext.Pages.Remove(page);
		}

		public Task<Page?> GetByIdAsync(Guid id)
		{
			// TODO remove the null-forgiving operator once updated to EFCore 6
			return _cmsContext.Pages.SingleOrDefaultAsync(page => page.Id == id)!;
		}

		public Task<List<Page>> GetAllAsync()
		{
			return _cmsContext.Pages.ToListAsync();
		}
	}
}
