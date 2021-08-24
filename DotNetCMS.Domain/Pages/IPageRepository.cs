using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCMS.Domain.Pages
{
	public interface IPageRepository
	{
		public void Add(Page page);
		public void Remove(Page page);
		public Task<Page> GetByIdAsync(Guid id);
		public Task<List<Page>> GetAllAsync();
	}
}
