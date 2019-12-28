using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SW.ModelApi.SampleWeb.Pages
{
    public class Index1Model : PageModel
    {

        [BindProperty]
        public string UserId { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        async public Task OnPostAsync()
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "samer"),
                new Claim("FullName", "blabla"),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.Role, "Supervisor"),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);



            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                //IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };



            await HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: new ClaimsPrincipal(claimsIdentity),
                properties: authProperties);


        }
    }
}
