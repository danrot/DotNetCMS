namespace DotNetCMS.Program.Tests;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

class Application : WebApplicationFactory<global::Program>
{
	private string _connectionString;

	public Application(string connectionString)
	{
		_connectionString = connectionString;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.UseSetting("ConnectionStrings:DotNetCMS", _connectionString);
	}
}
