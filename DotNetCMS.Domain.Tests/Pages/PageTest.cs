using DotNetCMS.Domain.Pages;
using System;
using Xunit;

namespace DotNetCMS.Domain.Tests.Pages
{
	public sealed class PageTest
	{
		[Theory]
		[InlineData("Page Title 1"), InlineData("Page Title 2")]
		public void ConstructWithTitle(string title)
		{
			var page = new Page(title);

			Assert.Equal(title, page.Title);
			Assert.NotEqual(Guid.Empty, page.Id);
		}

		[Fact]
		public void ChangeTitle()
		{
			var page = new Page("Page Title");
			Assert.Equal("Page Title", page.Title);

			page.ChangeTitle("Updated Page Title");
			Assert.Equal("Updated Page Title", page.Title);
		}
	}
}
