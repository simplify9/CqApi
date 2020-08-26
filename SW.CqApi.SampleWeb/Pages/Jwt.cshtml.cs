using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SW.HttpExtensions;

namespace SW.CqApi.SampleWeb.Pages
{
    public class JwtModel : PageModel
    {
        private readonly JwtTokenParameters jwtTokenParameters;

        public JwtModel(JwtTokenParameters jwtTokenParameters)
        {
            this.jwtTokenParameters = jwtTokenParameters;
        }

        [BindProperty]
        public string Jwt { get; set; }

        public void OnGet()
        {
            Jwt = jwtTokenParameters.WriteJwt((ClaimsIdentity)User.Identity);
        }
    
    }
}
