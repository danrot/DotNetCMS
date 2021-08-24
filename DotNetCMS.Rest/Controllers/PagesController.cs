using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCMS.Rest.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PagesController
	{
		private readonly PageService _pageService;

		public PagesController(PageService pageService)
		{
			_pageService = pageService;
		}

		[HttpGet]
		public async Task<IEnumerable<Page>> GetPages()
		{
			return await _pageService.GetAllAsync();
		}
	}
}
