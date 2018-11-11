using System.Net.Http;
using consumer.Repositories;
using Consumer.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace consumer
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
            services.AddScoped<IDatabase, Database>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();


            services.AddSingleton<HttpClient>(provider => new HttpClient());
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
           
            app.UseMvc();
        }
    }
}
