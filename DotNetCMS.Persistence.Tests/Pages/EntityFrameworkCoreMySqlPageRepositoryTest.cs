using DotNetCMS.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DotNetCMS.Persistence.Test.Pages
{
	public sealed class EntityFrameworkCoreMySqlPageRepositoryTest : EntityFrameworkCorePageRepositoryTest
	{
		private static string _connectionString = "server=localhost;user=root;database=DotNetCMS_Test";

		public EntityFrameworkCoreMySqlPageRepositoryTest()
			: base(
				new DbContextOptionsBuilder<CmsContext>()
					.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString))
					.Options
			)
		{}
	}
}
