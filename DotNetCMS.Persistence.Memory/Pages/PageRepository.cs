using DotNetCMS.Domain.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCMS.Persistence.Memory.Pages
{
	public sealed class PageRepository : IPageRepository
	{
		private readonly IDictionary<Guid, Page> _pages = new Dictionary<Guid, Page>();

		public void Add(Page page)
		{
			_pages.Add(page.Id, page);
		}

		public void Remove(Page page)
		{
			_pages.Remove(page.Id);
		}

		public Task<Page?> GetByIdAsync(Guid id)
		{
			return Task.FromResult(_pages.Keys.Contains(id) ? _pages[id] : null);
		}

		public Task<List<Page>> GetAllAsync()
		{
			return Task.FromResult(_pages.Values.ToList());
		}
	}
}
