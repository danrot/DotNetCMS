namespace DotNetCMS.Persistence.Memory.Pages;

using DotNetCMS.Domain.Pages;

/// <remarks>
///		This class uses an in-memory dictionary and therefore has to be registered in ASP.NET using the
///		AddSingleton method. Otherwise this class will be regenerated on every request causing it to lose its data.
/// </remarks>
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
