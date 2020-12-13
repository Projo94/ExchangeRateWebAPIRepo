using ExchangeRateCase.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ExchangeRateCase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddControllers();

            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson();

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddScoped<IApiCall, ApiCall>();

            services.AddScoped<IHistoricalExchangeRateService, HistoricalExchangeRateService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("MyPolicy");

            app.Use(async (context, next) =>
            {
                // Add Header
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";

                // Call next middleware
                await next.Invoke();
            });

            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
