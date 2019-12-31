using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SW.CqApi;


namespace SW.CqApi.UnitTests
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddControllers().AddApplicationPart(typeof(CqApiController).Assembly);
            services.AddCqApi(typeof(TestStartup).Assembly);
            services.AddAuthentication().
                AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = "cqapi",
                        ValidAudience = "cqapi",

                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("6547647654764764767657658658758765876532542"))
                    };
                });
            //services.AddHttpClient<Resources.Car.ApiClient>();

            //var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, "samer"),
            //    new Claim("FullName", "blabla"),
            //    new Claim(ClaimTypes.Role, "mapi.sw.modelapi.samplemodel.carservice.*"),
            //    new Claim(ClaimTypes.Role, "Supervisor"),
            //};

            //var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //services.AddMapiMockCallContext(new ClaimsPrincipal(claimsIdentity));
            //services.AddMapiCallContext();  


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
