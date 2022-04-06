namespace DotNetCMS.Persistence.Test.Pages;

using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.Memory.Pages;
using System;

public sealed class MemoryPageRepositoryTest : PageRepositoryTest, IDisposable
{
	private readonly IPageRepository _pageRepository = new PageRepository();

	protected override void SaveChanges() {}

	protected override IPageRepository CreatePageRepository()
	{
		return _pageRepository;
	}

	protected override void Clear() {}

	public void Dispose() {}
}
