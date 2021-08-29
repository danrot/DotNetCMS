using DotNetCMS.Domain.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

		public async Task<Page> GetAsync(GetCommand getCommand)
		{
			var page = await _pageRepository.GetByIdAsync(getCommand.Id);
			this.EnsureNotNullPage(page, getCommand.Id);

			return page;
		}

		public Page Create(CreateCommand createCommand)
		{
			var page = new Page(createCommand.Title);
			_pageRepository.Add(page);

			return page;
		}

		public async Task<Page> UpdateAsync(UpdateCommand updateCommand)
		{
			var page = await _pageRepository.GetByIdAsync(updateCommand.Id);
			this.EnsureNotNullPage(page, updateCommand.Id);

			page.ChangeTitle(updateCommand.Title);

			return page;
		}

		public async Task DeleteAsync(DeleteCommand deleteCommand)
		{
			var page = await _pageRepository.GetByIdAsync(deleteCommand.Id);
			this.EnsureNotNullPage(page, deleteCommand.Id);

			_pageRepository.Remove(page);
		}

		private void EnsureNotNullPage([NotNull] Page? page, Guid pageId)
		{
			if (page == null)
			{
				throw new PageNotFoundException(pageId);
			}
		}
	}
}
