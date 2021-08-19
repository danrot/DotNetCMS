using DotNetCMS.Domain.Pages;
using System;
using Xunit;

namespace DotNetCMS.Persistence.Test.Pages
{
	public abstract class PageRepositoryTest
	{
		protected abstract IPageRepository CreatePageRepository();

		protected abstract void SaveChanges();

		protected abstract void Clear();

		[Fact]
		public async void Add()
		{
			var pageRepository = CreatePageRepository();

			var page1 = new Page("Page Title 1");
			Guid pageId1 = page1.Id;
			var page2 = new Page("Page Title 2");
			Guid pageId2 = page2.Id;

			pageRepository.Add(page1);
			pageRepository.Add(page2);

			SaveChanges();
			Clear();

			pageRepository = CreatePageRepository();
			Assert.Equal("Page Title 1", (await pageRepository.GetByIdAsync(pageId1)).Title);
			Assert.Equal("Page Title 2", (await pageRepository.GetByIdAsync(pageId2)).Title);
		}
	}
}
