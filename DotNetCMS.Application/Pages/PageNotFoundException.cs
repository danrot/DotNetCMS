using System;

namespace DotNetCMS.Application.Pages
{
	public sealed class PageNotFoundException : Exception
	{
		public PageNotFoundException(Guid id) : base($"The page with the ID {id} does not exist") { }
	}
}
