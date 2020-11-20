using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Entities;
using Wd3w.AspNetCore.EasyTesting.SampleApi.Services;

namespace Wd3w.AspNetCore.EasyTesting.SampleApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<SampleRepository>();
            services.AddDbContext<SampleDb>();
            services.AddSingleton(new DbContextOptionsBuilder<SampleDb>()
                .UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw")
                .Options);
            
            services.AddAuthentication("Bearer")
                .AddJwtBearer(options =>
                {
                    options.SaveToken = false;
                    options.ClaimsIssuer = "http://localhost";
                    options.IncludeErrorDetails = true;
                    options.Audience = "http://localhost";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "http://localhost",
                        ValidAudience = "http://localhost",
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ABCDABCDABCDABCDABCDABCDABCDABCD")),
                    };
                })
                // .AddTokenAuthenticationScheme<CustomTokenAuthService>("Bearer", new TokenAuthenticationConfiguration
                // {
                //     Realm = "https://www.test.com/sign-in",
                //     TokenLength = 20
                // })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}