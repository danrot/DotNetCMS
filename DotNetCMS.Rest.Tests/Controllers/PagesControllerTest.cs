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
			var response = await _client.GetAsync("/Pages");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var contentDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();

			Assert.Empty(contentDocument!.RootElement.EnumerateArray());
		}

		[Fact]
		public async void GetPagesAsync()
		{
			await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 2"});

			var response = await _client.GetAsync("/Pages");

			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var contentDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();

			var pages = contentDocument!.RootElement.EnumerateArray().ToArray();

			Assert.Equal(2, pages.Length);
			Assert.Contains(pages, page => page.GetProperty("title").GetString() == "Page Title 1");
			Assert.Contains(pages, page => page.GetProperty("title").GetString() == "Page Title 2");
		}

		[Fact]
		public async void GetPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title"});
			var postContentDocument = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
			var pageId = postContentDocument!.RootElement.GetProperty("id").GetGuid();

			var response = await _client.GetAsync($"/Pages/{pageId}");
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);

			var contentDocument = await response.Content.ReadFromJsonAsync<JsonDocument>();
			Assert.Equal(pageId, contentDocument!.RootElement.GetProperty("id").GetGuid());
			Assert.Equal("Page Title", contentDocument.RootElement.GetProperty("title").GetString());
		}

		[Fact]
		public async void GetNonExistingPageAsync()
		{
			var response = await _client.GetAsync($"/Pages/{Guid.NewGuid()}");
			Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
		}

		[Fact]
		public async void PostPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			var postContentDocument = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
			var pageId = postContentDocument!.RootElement.GetProperty("id").GetGuid();

			Assert.Equal(new Uri($"http://localhost/Pages/{pageId}"), postResponse.Headers.Location);
			Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

			Assert.Equal("Page Title 1", postContentDocument.RootElement.GetProperty("title").GetString());

			var getResponse = await _client.GetAsync($"/Pages/{pageId}");

			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
			var getContentDocument = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

			Assert.Equal(pageId, getContentDocument!.RootElement.GetProperty("id").GetGuid());
			Assert.Equal("Page Title 1", getContentDocument.RootElement.GetProperty("title").GetString());
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
			var postContentDocument = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
			var pageId = postContentDocument!.RootElement.GetProperty("id").GetGuid();
			Assert.Equal("Page Title 1", postContentDocument.RootElement.GetProperty("title").GetString());

			var getResponse1 = await _client.GetAsync($"/Pages/{pageId}");
			var getContentDocument1 = await getResponse1.Content.ReadFromJsonAsync<JsonDocument>();
			Assert.Equal("Page Title 1", getContentDocument1!.RootElement.GetProperty("title").GetString());

			var putResponse = await _client.PutAsJsonAsync(
				$"/Pages/{pageId}",
				new { Id = pageId, Title = "Updated Page Title 1"}
			);
			Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);
			var putContentDocument = await putResponse.Content.ReadFromJsonAsync<JsonDocument>();
			Assert.Equal("Updated Page Title 1", putContentDocument!.RootElement.GetProperty("title").GetString());

			var getResponse2 = await _client.GetAsync($"/Pages/{pageId}");
			var getContentDocument2 = await getResponse2.Content.ReadFromJsonAsync<JsonDocument>();
			Assert.Equal("Updated Page Title 1", getContentDocument2!.RootElement.GetProperty("title").GetString());
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
			var postContentDocument = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
			var pageId = postContentDocument!.RootElement.GetProperty("id").GetGuid();

			var getResponse1 = await _client.GetAsync($"/Pages/{pageId}");
			Assert.Equal(HttpStatusCode.OK, getResponse1.StatusCode);

			var deleteResponse = await _client.DeleteAsync($"/Pages/{pageId}");
			Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

			var getResponse2 = await _client.GetAsync($"/Pages/{pageId}");
			Assert.Equal(HttpStatusCode.NotFound, getResponse2.StatusCode);
		}

		[Fact]
		public async void DeleteNonExistingPage()
		{
			var pageId = Guid.NewGuid();

			var deleteResponse = await _client.DeleteAsync($"/Pages/{pageId}");

			Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
		}
	}
}
