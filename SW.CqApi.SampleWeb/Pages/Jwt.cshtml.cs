using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace SW.CqApi.SampleWeb.Pages
{
    public class JwtModel : PageModel
    {

        [BindProperty]
        public string Jwt { get; set; }

        public void OnGet()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6547647654764764767657658658758765876532542"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken("cqapi",
              "cqapi",
              User.Claims ,
              expires: DateTime.Now.AddMinutes(1400),
              signingCredentials: creds);

            Jwt = new JwtSecurityTokenHandler().WriteToken(token);
        }
    
    }
}
