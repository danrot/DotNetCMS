using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DotNetCMS.Rest.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PagesController : ControllerBase
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

		[HttpGet("{id}")]
		public async Task<ActionResult<Page>> GetPage([FromRoute] GetCommand getCommand)
		{
			try {
				var page = await _pageService.GetAsync(getCommand);

				return page;
			}
			catch (PageNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpPost]
		public ActionResult<Page> PostPage(CreateCommand createCommand)
		{
			var page = _pageService.Create(createCommand);

			return CreatedAtAction(nameof(GetPage), new { id = page.Id }, page);
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<Page>> PutPage(Guid id, [FromBody] UpdateCommand updateCommand)
		{
			if (id != updateCommand.Id)
			{
				return Problem(
					statusCode: (int)HttpStatusCode.BadRequest,
					title: "The ids from the URL and body do not match."
				);
			}

			try {
				var page = await _pageService.UpdateAsync(updateCommand);

				return page;
			}
			catch (PageNotFoundException)
			{
				return NotFound();
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePage([FromRoute] DeleteCommand deleteCommand)
		{
			try {
				await _pageService.DeleteAsync(deleteCommand);

				return NoContent();
			}
			catch (PageNotFoundException)
			{
				return NotFound();
			}
		}
	}
}
