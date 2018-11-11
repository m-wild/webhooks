using System.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using Producer.Infrastructure;
using Producer.Repositories;
using Producer.Services;

namespace Producer
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
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IEventService, EventService>();

            services.AddScoped<TransactionFilter>();
            services.AddMvc(o => { o.Filters.AddService<TransactionFilter>(); });
            
            services.AddSingleton<IEventQueue, EventQueue>();
            services.AddSingleton<HttpClient>(provider => new HttpClient());

            Dapper.SqlMapper.AddTypeMap(typeof(string), DbType.AnsiString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IEventQueue eventQueue)
        {
            var _ = eventQueue.ToString(); // need to ensure the event queue instance is created on startup....
            
            app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}
