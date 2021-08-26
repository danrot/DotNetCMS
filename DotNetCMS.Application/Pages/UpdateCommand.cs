using System;

namespace DotNetCMS.Application.Pages
{
	public sealed record UpdateCommand(Guid Id, string Title);
}
