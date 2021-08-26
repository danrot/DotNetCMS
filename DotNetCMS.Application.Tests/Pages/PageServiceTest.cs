using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.Memory.Pages;
using System;
using Xunit;

namespace DotNetCMS.Application.Test.Pages
{
	public sealed class PageServiceTest
	{
		private readonly PageService _pageService;

		private readonly PageRepository _pageRepository;

		public PageServiceTest()
		{
			_pageRepository = new PageRepository();
			_pageService = new PageService(_pageRepository);
		}

		[Fact]
		public async void GetAllEmptyAsync()
		{
			Assert.Empty(await _pageService.GetAllAsync());
		}

		[Fact]
		public async void GetAllAsync()
		{
			var page1 = new Page("Page Title 1");
			Guid pageId1 = page1.Id;
			var page2 = new Page("Page Title 2");
			Guid pageId2 = page2.Id;

			_pageRepository.Add(page1);
			_pageRepository.Add(page2);

			var pages = await _pageService.GetAllAsync();
			Assert.Equal(2, pages.Count);
			Assert.Contains(pages, page => page.Title == "Page Title 1");
			Assert.Contains(pages, page => page.Title == "Page Title 2");
		}

		[Fact]
		public async void GetAsync()
		{
			var page1 = new Page("Page Title 1");
			_pageRepository.Add(page1);

			Assert.Same(page1, await _pageService.GetAsync(new GetCommand(page1.Id)));
		}

		[Fact]
		public async void GetNonExistingAsync()
		{
			await Assert.ThrowsAsync<PageNotFoundException>(() => _pageService.GetAsync(new GetCommand(Guid.Empty)));
		}

		[Fact]
		public async void CreateAsync()
		{
			var page1 = _pageService.Create(new CreateCommand("Page Title 1"));
			Assert.Equal("Page Title 1", page1.Title);
			var page2 = _pageService.Create(new CreateCommand("Page Title 2"));
			Assert.Equal("Page Title 2", page2.Title);

			var pages = await _pageService.GetAllAsync();
			Assert.Equal(2, pages.Count);
			Assert.Contains(pages, page => page.Title == "Page Title 1");
			Assert.Contains(pages, page => page.Title == "Page Title 2");
		}

		[Fact]
		public async void UpdateAsync()
		{
			var page = new Page("Page Title");
			var pageId = page.Id;
			_pageRepository.Add(page);

			page = await _pageService.UpdateAsync(new UpdateCommand(page.Id, "Updated Page Title"));
			Assert.Equal(pageId, page.Id);
			Assert.Equal("Updated Page Title", page.Title);

			page = await _pageRepository.GetByIdAsync(pageId);
			Assert.Equal(pageId, page.Id);
			Assert.Equal("Updated Page Title", page.Title);
		}

		[Fact]
		public async void UpdateNonExistingAsync()
		{
			await Assert.ThrowsAsync<PageNotFoundException>(
				() => _pageService.UpdateAsync(new UpdateCommand(Guid.Empty, "UpdatedPage Title"))
			);
		}

		[Fact]
		public async void DeleteAsync()
		{
			var page = new Page("Page Title");
			var pageId = page.Id;
			_pageRepository.Add(page);

			await _pageService.DeleteAsync(new DeleteCommand(page.Id));

			page = await _pageRepository.GetByIdAsync(pageId);
			Assert.Null(page);
		}

		[Fact]
		public async void DeleteNonExistingAsync()
		{
			await Assert.ThrowsAsync<PageNotFoundException>(
				() => _pageService.DeleteAsync(new DeleteCommand(Guid.Empty))
			);
		}
	}
}
