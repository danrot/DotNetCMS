using System;

namespace DotNetCMS.Domain.Pages
{
	public sealed class Page
	{
		public Guid Id { get; } = Guid.NewGuid();

		public string Title { get; private set; }

		public Page(string title)
		{
			Title = title;
		}

		public void ChangeTitle(string title)
		{
			Title = title;
		}
	}
}
