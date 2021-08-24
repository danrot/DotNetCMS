using DotNetCMS.Domain.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCMS.Application.Pages
{
	public class PageService
	{
		private readonly IPageRepository _pageRepository;

		public PageService(IPageRepository pageRepository)
		{
			_pageRepository = pageRepository;
		}

		public Task<List<Page>> GetAllAsync()
		{
			return _pageRepository.GetAllAsync();
		}
	}
}
