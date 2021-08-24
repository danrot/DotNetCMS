using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.Memory.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Xunit;

namespace DotNetCMS.Rest.Tests.Controllers
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

						services.AddScoped<IPageRepository, PageRepository>();
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
		public async void GetPages()
		{
			var response = await _client.GetAsync("/Pages");

			response.EnsureSuccessStatusCode();
			var contentDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

			Assert.Empty(contentDocument.RootElement.EnumerateArray());
		}
	}
}
