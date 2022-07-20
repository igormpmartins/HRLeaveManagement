using AutoMapper;
using HR.LeaveManagement.MVC.Contracts;
using HR.LeaveManagement.MVC.Models;
using HR.LeaveManagement.MVC.Services.Base;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HR.LeaveManagement.MVC.Services
{
    public class AuthenticationService : BaseHttpService, Contracts.IAuthenticationService
    {
        private readonly IClient client;
        private readonly ILocalStorageService localStorage;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;
        private readonly JwtSecurityTokenHandler tokenHandler;

        public AuthenticationService(IClient client, ILocalStorageService localStorage, 
            IHttpContextAccessor httpContextAccessor, IMapper mapper) : base (client, localStorage)
        {
            this.client = client;
            this.localStorage = localStorage;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.tokenHandler = new JwtSecurityTokenHandler();
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            try
            {
                AuthRequest authRequest = new () { Email = email, Password = password };
                var authResponse = await client.LoginAsync(authRequest);

                if (!authResponse.Token.IsNullOrEmpty())
                {
                    var tokenContent = tokenHandler.ReadJwtToken(authResponse.Token);
                    var claims = ParseClaims(tokenContent);

                    var user = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
                    var login = httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, user);
                    localStorage.SetStorageValue("token", authResponse.Token);

                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;

        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));
            return claims;
        }

        public async Task Logout()
        {
            localStorage.ClearStorage(new List<string> { "token" });
            await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<bool> Register(RegisterVM registration)
        {
            try
            {
                RegistrationRequest registrationRequest = mapper.Map<RegistrationRequest>(registration);
                var response = await client.RegisterAsync(registrationRequest);

                if (!string.IsNullOrEmpty(response.UserId))
                {
                    await Authenticate(registration.Email, registration.Password);
                    return true;
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
