namespace Medicloud.WebUI.Middlewares
{
	public class JwtTokenMiddleware
	{
		private readonly RequestDelegate _next;

		public JwtTokenMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var cookieToken = context.Request.Cookies["JwtToken"];
			//Console.WriteLine(cookieToken);
			if (!string.IsNullOrEmpty(cookieToken))
			{
			
				context.Request.Headers["Authorization"] = $"Bearer {cookieToken}";
			}
			else
			{
				context.Request.Headers["Authorization"] = $"Bearer ";
			}
			//var path = context.Request.Path;
			//var isApiRequest = path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);

			//if (!isApiRequest)
			//{
			
			//	if (string.IsNullOrEmpty(cookieToken) || cookieToken == "invalid")
			//	{
					
			//		context.Response.Redirect("/Login/Index");
			//		return; 
			//	}
			//}


			await _next(context);
		}
	}
}
