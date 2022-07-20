using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HR.LeaveManagement.MVC.Middleware
{
    public class RequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILocalStorageService localStorageService;

        public RequestMiddleware(RequestDelegate next, ILocalStorageService localStorageService)
        {
            this.next = next;
            this.localStorageService = localStorageService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var ep = context.Features.Get<IEndpointFeature>()?.Endpoint;
                var authAttr = ep?.Metadata?.GetMetadata<AuthorizeAttribute>();

                if (authAttr != null)
                {
                    var tokenExists = localStorageService.Exists("token");
                    var tokenIsValid = true;

                    if (tokenExists)
                    {
                        var token = localStorageService.GetStorageValue<string>("token");
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var tokenContent = tokenHandler.ReadJwtToken(token);
                        
                        var expiry = tokenContent.ValidTo;
                        if (expiry < DateTime.Now)
                            tokenIsValid = false;

                    }

                    if (!tokenIsValid || !tokenExists)
                    {
                        await SignOutAndRedirect(context);
                        return;
                    }

                    if (authAttr.Roles!= null)
                    {
                        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

                        if (!authAttr.Roles.Contains(userRole))
                        {
                            var path = $"/home/notauthorized";
                            context.Response.Redirect(path);
                            return;
                        }
                    }
                }

                await next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            switch (ex)
            {
                case ApiException:
                    await SignOutAndRedirect(context);
                    break;
                default:
                    var path = $"/Home/Error";
                    context.Response.Redirect(path);
                    break;
            }
        }

        private static async Task SignOutAndRedirect(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var path = $"/users/login";
            context.Response.Redirect(path);
        }
    }
}
