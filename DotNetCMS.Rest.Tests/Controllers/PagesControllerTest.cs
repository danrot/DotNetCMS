using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.Memory.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace DotNetCMS.Program.Tests
{
	public sealed class PagesControllerTest
	{
		private readonly TestServer _server;

		private readonly HttpClient _client;

		public PagesControllerTest()
		{
			_server = new TestServer(
				new WebHostBuilder()
					.ConfigureServices(services =>
					{
						services
							.AddControllers()
							.AddApplicationPart(Assembly.Load("DotNetCMS.Rest"));

						services.AddSingleton<IPageRepository, PageRepository>();
						services.AddScoped<PageService>();
					})
					.Configure(app =>
					{
						app.UseRouting();
						app.UseEndpoints(endpoints =>  endpoints.MapControllers());
					})
			);

			_client = _server.CreateClient();
		}

		[Fact]
		public async void GetPagesEmptyAsync()
		{
			var getResponse = await _client.GetAsync("/Pages");
			var loadedPages = await GetPagesFromResponse(getResponse);
			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

			Assert.Empty(loadedPages!.RootElement.EnumerateArray());
		}

		[Fact]
		public async void GetPagesAsync()
		{
			await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 2"});

			var getResponse = await _client.GetAsync("/Pages");
			var loadedPages = await GetPagesFromResponse(getResponse);
			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

			var pages = loadedPages!.RootElement.EnumerateArray().ToArray();

			Assert.Equal(2, pages.Length);
			Assert.Contains(pages, page => GetTitleFromPage(page) == "Page Title 1");
			Assert.Contains(pages, page => GetTitleFromPage(page) == "Page Title 2");
		}

		[Fact]
		public async void GetPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title"});
			var createdPage = await GetPageFromResponse(postResponse);
			var createdPageId = GetIdFromPage(createdPage);

			var getResponse = await _client.GetAsync($"/Pages/{createdPageId}");
			var loadedPage = await GetPageFromResponse(getResponse);

			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
			Assert.Equal(createdPageId, GetIdFromPage(loadedPage));
			Assert.Equal("Page Title", GetTitleFromPage(loadedPage));
		}

		[Fact]
		public async void GetNonExistingPageAsync()
		{
			var getResponse = await _client.GetAsync($"/Pages/{Guid.NewGuid()}");
			Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
		}

		[Fact]
		public async void PostPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			var createdPage = await GetPageFromResponse(postResponse);
			var createdPageId = GetIdFromPage(createdPage);

			Assert.Equal(new Uri($"http://localhost/Pages/{createdPageId}"), postResponse.Headers.Location);
			Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

			Assert.Equal("Page Title 1", GetTitleFromPage(createdPage));

			var getResponse = await _client.GetAsync($"/Pages/{createdPageId}");
			var loadedPage = await GetPageFromResponse(getResponse);
			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

			Assert.Equal(createdPageId, GetIdFromPage(loadedPage));
			Assert.Equal("Page Title 1", GetTitleFromPage(loadedPage));
		}

		[Fact]
		public async void PostPageWithoutTitleAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new {});
			Assert.Equal(HttpStatusCode.BadRequest, postResponse.StatusCode);
		}

		[Fact]
		public async void PutPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			var createdPage = await GetPageFromResponse(postResponse);
			var pageId = GetIdFromPage(createdPage);
			Assert.Equal("Page Title 1", GetTitleFromPage(createdPage));

			var getResponse1 = await _client.GetAsync($"/Pages/{pageId}");
			var loadedPage1 = await GetPageFromResponse(getResponse1);
			Assert.Equal("Page Title 1", GetTitleFromPage(loadedPage1));

			var putResponse = await _client.PutAsJsonAsync(
				$"/Pages/{pageId}",
				new { Id = pageId, Title = "Updated Page Title 1"}
			);
			var updatedPage = await GetPageFromResponse(putResponse);
			Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
			Assert.Equal("Updated Page Title 1", GetTitleFromPage(updatedPage));

			var getResponse2 = await _client.GetAsync($"/Pages/{pageId}");
			var loadedPage2 = await GetPageFromResponse(getResponse2);
			Assert.Equal("Updated Page Title 1", GetTitleFromPage(loadedPage2));
		}

		[Fact]
		public async void PutPageWithDifferentIdsAsync()
		{
			var putResponse = await _client.PutAsJsonAsync(
				$"/Pages/{Guid.NewGuid()}",
				new { Id = Guid.NewGuid(), Title = "Updated Page Title 1"}
			);
			Assert.Equal(HttpStatusCode.BadRequest, putResponse.StatusCode);
		}

		[Fact]
		public async void PutNonExistingPage()
		{
			var pageId = Guid.NewGuid();
			var putResponse = await _client.PutAsJsonAsync(
				$"/Pages/{pageId}",
				new { Id = pageId, Title = "Updated Page Title"}
			);

			Assert.Equal(HttpStatusCode.NotFound, putResponse.StatusCode);
		}

		[Fact]
		public async void DeletePageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			var createdPage = await GetPageFromResponse(postResponse);
			var createdPageId = GetIdFromPage(createdPage);

			var getResponse1 = await _client.GetAsync($"/Pages/{createdPageId}");
			Assert.Equal(HttpStatusCode.OK, getResponse1.StatusCode);

			var deleteResponse = await _client.DeleteAsync($"/Pages/{createdPageId}");
			Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

			var getResponse2 = await _client.GetAsync($"/Pages/{createdPageId}");
			Assert.Equal(HttpStatusCode.NotFound, getResponse2.StatusCode);
		}

		[Fact]
		public async void DeleteNonExistingPage()
		{
			var pageId = Guid.NewGuid();
			var deleteResponse = await _client.DeleteAsync($"/Pages/{pageId}");

			Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
		}

		private async Task<JsonDocument> GetPagesFromResponse(HttpResponseMessage response)
		{
			return (await response.Content.ReadFromJsonAsync<JsonDocument>())!;
		}

		private async Task<JsonDocument> GetPageFromResponse(HttpResponseMessage response)
		{
			return (await response.Content.ReadFromJsonAsync<JsonDocument>())!;
		}

		private Guid GetIdFromPage(JsonDocument page)
		{
			return page.RootElement.GetProperty("id").GetGuid();
		}

		private string GetTitleFromPage(JsonDocument page)
		{
			return GetTitleFromPage(page.RootElement);
		}

		private string GetTitleFromPage(JsonElement page)
		{
			return page.GetProperty("title").GetString()!;
		}
	}
}
