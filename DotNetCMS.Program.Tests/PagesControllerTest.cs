using DotNetCMS.Persistence.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace DotNetCMS.Program.Tests
{
	public sealed class PagesControllerMySqlTest
	{
		private static readonly string _connectionString =
			$"server=localhost;user={Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root"};"
			+ $"password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? ""};"
			+ $"database={Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "DotNetCMS_Test"}";

		private readonly TestServer _server;

		private readonly HttpClient _client;

		public PagesControllerMySqlTest()
		{
			_server = new TestServer(
				new WebHostBuilder()
					.UseSetting("ConnectionStrings:DotNetCMS", _connectionString)
					.UseStartup<Startup>()
			);

			_client = _server.CreateClient();

			using (var cmsContext = (CmsContext) _server.Host.Services.GetService(typeof(CmsContext)))
			{
				cmsContext.Database.EnsureDeleted();
				cmsContext.Database.Migrate();
			}
		}

		[Fact]
		public async void PostPageAsync()
		{
			var postResponse = await _client.PostAsJsonAsync("/Pages", new { Title = "Page Title 1"});
			var postContentDocument = await postResponse.Content.ReadFromJsonAsync<JsonDocument>();
			var pageId = postContentDocument.RootElement.GetProperty("id").GetGuid();

			Assert.Equal(new Uri($"http://localhost/Pages/{pageId}"), postResponse.Headers.Location);
			Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

			Assert.Equal("Page Title 1", postContentDocument.RootElement.GetProperty("title").GetString());

			var getResponse = await _client.GetAsync($"/Pages/{pageId}");

			Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
			var getContentDocument = await getResponse.Content.ReadFromJsonAsync<JsonDocument>();

			Assert.Equal(pageId, getContentDocument.RootElement.GetProperty("id").GetGuid());
			Assert.Equal("Page Title 1", getContentDocument.RootElement.GetProperty("title").GetString());
		}
	}
}
