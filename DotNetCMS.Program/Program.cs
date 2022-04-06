using DotNetCMS.Application.Pages;
using DotNetCMS.Domain.Pages;
using DotNetCMS.Persistence.EntityFrameworkCore;
using DotNetCMS.Persistence.EntityFrameworkCore.Pages;
using DotNetCMS.Persistence.EntityFrameworkCore.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DotNetCMS");

builder.Services.AddControllers(options =>
{
	options.Filters.Add(typeof(TransactionFilter));
});

builder.Services.AddDbContext<CmsContext>(
	options => options.UseMySql(
		connectionString,
		ServerVersion.AutoDetect(connectionString),
		sqlOptions => sqlOptions.MigrationsAssembly("DotNetCMS.Program")
	)
);

builder.Services.AddScoped<IPageRepository, PageRepository>();
builder.Services.AddScoped<PageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.Run();
