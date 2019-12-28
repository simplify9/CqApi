using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;




namespace SW.CqApi.SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(config => 
            {
                config.JsonSerializerOptions.PropertyNamingPolicy = null;
            }) ;
            //services.AddI18n();
            services.AddCqApi();
            //services.AddMapiCallContext(); 
            services.AddRazorPages();
            ;
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
                AddCookie(options => 
                {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseStaticFiles();

            app.UseSwaggerUI(c =>
            {


                 c.SwaggerEndpoint("/cqapi/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                 
            });
        }
    }
}
