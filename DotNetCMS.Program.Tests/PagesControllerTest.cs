namespace DotNetCMS.Program.Tests;

using DotNetCMS.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

public sealed class PagesControllerTest
{
	private static readonly string _connectionString =
		$"server=localhost;user={Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root"};"
		+ $"password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? ""};"
		+ $"database={Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "DotNetCMS_Test"}";

	private readonly HttpClient _client;

	public PagesControllerTest()
	{
		var application = new Application(_connectionString);

		_client = application.CreateClient();

		using (var scope = application.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
		{
			using (var cmsContext = (CmsContext) scope.ServiceProvider.GetRequiredService(typeof(CmsContext))!)
			{
				cmsContext.Database.EnsureDeleted();
				cmsContext.Database.Migrate();
			}
		}
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

		var getResponse =  await _client.GetAsync($"/Pages/{createdPageId}");
		Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
		var loadedPage = await GetPageFromResponse(getResponse);

		Assert.Equal(createdPageId, GetIdFromPage(loadedPage));
		Assert.Equal("Page Title 1", GetTitleFromPage(loadedPage));
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
		return page.RootElement.GetProperty("title").GetString()!;
	}
}
