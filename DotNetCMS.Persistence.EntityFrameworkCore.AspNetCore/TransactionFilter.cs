using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace DotNetCMS.Persistence.EntityFrameworkCore.AspNetCore
{
	public sealed class TransactionFilter : IAsyncActionFilter
	{
		private readonly CmsContext _cmsContext;

		public TransactionFilter(CmsContext cmsContext)
		{
			_cmsContext = cmsContext;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext actionContext, ActionExecutionDelegate next)
		{
			await next();
			// TODO add an integration test to see if this actually causes an error response
			// not possible right now because no mocking library allows mock sealed classes
			await _cmsContext.SaveChangesAsync();
		}
	}
}
