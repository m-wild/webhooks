using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
            services.AddMvc(o =>
                {
                    o.Filters.AddService<TransactionFilter>();
                })
                .AddJsonOptions(jo => jo.SerializerSettings.Converters.Add(new StringEnumConverter()));
            
            services.AddSingleton<IEventQueue, EventQueue>();
            services.AddSingleton<HttpClient>(provider => new HttpClient());

            Dapper.SqlMapper.AddTypeMap(typeof(string), DbType.AnsiString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}
