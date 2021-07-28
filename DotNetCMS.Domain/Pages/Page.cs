namespace DotNetCMS.Domain.Pages
{
	public class Page
	{
		public string Title { get; private set; }

		public Page(string title)
		{
			Title = title;
		}
	}
}
