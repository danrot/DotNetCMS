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
		public async void GetAllEmpty()
		{
			Assert.Empty(await _pageService.GetAllAsync());
		}

		[Fact]
		public async void GetAll()
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
	}
}
