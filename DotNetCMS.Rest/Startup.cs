using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.EntityFrameworkCore;
using DotNetCMS.Persistence.EntityFrameworkCore.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCMS.Rest
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			string connectionString = _configuration.GetConnectionString("DotNetCMS");

			services.AddControllers();
			services.AddDbContext<CmsContext>(
				options => options.UseMySql(
					connectionString,
					ServerVersion.AutoDetect(connectionString),
					sqlOptions => sqlOptions.MigrationsAssembly("DotNetCMS.Rest")
				)
			);
			services.AddScoped<IPageRepository, PageRepository>();
			services.AddScoped<PageService>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CmsContext context)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			context.Database.Migrate();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
