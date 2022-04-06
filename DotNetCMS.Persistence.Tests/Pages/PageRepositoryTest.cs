namespace DotNetCMS.Persistence.Test.Pages;

using DotNetCMS.Domain.Pages;
using System;
using Xunit;

public abstract class PageRepositoryTest
{
	protected abstract IPageRepository CreatePageRepository();

	protected abstract void SaveChanges();

	protected abstract void Clear();

	[Fact]
	public async void GetByNonExistingId()
	{
		var pageRepository = CreatePageRepository();

		Assert.Null(await pageRepository.GetByIdAsync(Guid.NewGuid()));
	}

	[Fact]
	public async void GetAllEmpty()
	{
		var pageRepository = CreatePageRepository();

		Assert.Empty(await pageRepository.GetAllAsync());
	}

	[Fact]
	public async void GetAllNonEmpty()
	{
		var pageRepository = CreatePageRepository();
		var page1 = CreatePage(pageRepository, "Page Title 1");
		var page2 = CreatePage(pageRepository, "Page Title 2");

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		var pages = await pageRepository.GetAllAsync();

		Assert.Equal(2, pages.Count);
		Assert.Contains(pages, page => page.Title == "Page Title 1");
		Assert.Contains(pages, page => page.Title == "Page Title 2");
	}

	[Fact]
	public async void Add()
	{
		var pageRepository = CreatePageRepository();
		var page1 = CreatePage(pageRepository, "Page Title 1");
		var page2 = CreatePage(pageRepository, "Page Title 2");

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		Assert.Equal("Page Title 1", (await pageRepository.GetByIdAsync(page1.Id))!.Title);
		Assert.Equal("Page Title 2", (await pageRepository.GetByIdAsync(page2.Id))!.Title);
	}

	[Fact]
	public async void Update()
	{
		var pageRepository = CreatePageRepository();
		var page1 = CreatePage(pageRepository, "Page Title 1");
		var page2 = CreatePage(pageRepository, "Page Title 2");

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		var updatePage1 = await pageRepository.GetByIdAsync(page1.Id);
		updatePage1!.ChangeTitle("Updated Page Title 1");

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		Assert.Equal("Updated Page Title 1", (await pageRepository.GetByIdAsync(page1.Id))!.Title);
		Assert.Equal("Page Title 2", (await pageRepository.GetByIdAsync(page2.Id))!.Title);
	}

	[Fact]
	public async void Delete()
	{
		var pageRepository = CreatePageRepository();

		var page1 = CreatePage(pageRepository, "Page Title 1");
		var page2 = CreatePage(pageRepository, "Page Title 2");

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		var deletePage1 = await pageRepository.GetByIdAsync(page1.Id);
		pageRepository.Remove(deletePage1!);

		SaveChanges();
		Clear();

		pageRepository = CreatePageRepository();
		Assert.Null(await pageRepository.GetByIdAsync(page1.Id));
		Assert.Equal("Page Title 2", (await pageRepository.GetByIdAsync(page2.Id))!.Title);
	}

	private Page CreatePage(IPageRepository pageRepository, string title)
	{
		var page = new Page(title);
		pageRepository.Add(page);

		return page;
	}
}
