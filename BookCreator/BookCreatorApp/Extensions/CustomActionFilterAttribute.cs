namespace BookCreatorApp.Extensions
{
	using BookCreator.Services.Utilities;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Filters;
	using Microsoft.AspNetCore.Routing;

	public class CustomActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			//TODO keep in mind some unofficial codes!
			var code = context.HttpContext.Response.StatusCode;

			if (code >= 400)
			{
				context.Result = new RedirectToRouteResult(new RouteValueDictionary
				{
					{ "controller", "Home" },
					{ "action", "Error" },
					{"area",""}
				});
			}
		}

		public override void OnResultExecuted(ResultExecutedContext context)
		{
			var code = context.HttpContext.Response.StatusCode;
			if (code >= 400)
			{
				context.HttpContext.Response.Redirect(GlobalConstants.RouteConstants.ErrorPageRoute);
			}
		}
	}
}