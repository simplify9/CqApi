using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SW.ModelApi.SampleWeb.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            var x = HttpContext.User;
        }
    }
}
