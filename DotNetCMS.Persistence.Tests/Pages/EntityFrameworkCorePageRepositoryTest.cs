using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.EntityFrameworkCore;
using DotNetCMS.Persistence.EntityFrameworkCore.Pages;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace DotNetCMS.Persistence.Test.Pages
{
	public abstract class EntityFrameworkCorePageRepositoryTest
	{
		private DbContextOptions<CmsContext> _options;

		protected EntityFrameworkCorePageRepositoryTest(DbContextOptions<CmsContext> options)
		{
			_options = options;

			using (var context = new CmsContext(_options))
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();
			}
		}

		[Fact]
		public async void Add()
		{
			Guid pageId1;
			Guid pageId2;

			using (var context = new CmsContext(_options))
			{
				var pageRepository = new PageRepository(context);

				var page1 = new Page("Page Title 1");
				pageId1 = page1.Id;
				var page2 = new Page("Page Title 2");
				pageId2 = page2.Id;

				pageRepository.Add(page1);
				pageRepository.Add(page2);

				context.SaveChanges();
			}

			using (var context = new CmsContext(_options))
			{
				var pageRepository = new PageRepository(context);
				Assert.Equal("Page Title 1", (await pageRepository.GetByIdAsync(pageId1)).Title);
				Assert.Equal("Page Title 2", (await pageRepository.GetByIdAsync(pageId2)).Title);
			}
		}
	}
}
