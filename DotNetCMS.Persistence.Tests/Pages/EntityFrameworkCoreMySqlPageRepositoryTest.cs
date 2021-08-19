using DotNetCMS.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DotNetCMS.Persistence.Test.Pages
{
	public sealed class EntityFrameworkCoreMySqlPageRepositoryTest : EntityFrameworkCorePageRepositoryTest
	{
		private static string _connectionString =
			$"server=localhost;user={Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root"};"
			+ $"password={Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? ""};"
			+ $"database={Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "DotNetCMS_Test"}";

		public EntityFrameworkCoreMySqlPageRepositoryTest()
			: base(
				new DbContextOptionsBuilder<CmsContext>()
					.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
					.Options
			)
		{}
	}
}
