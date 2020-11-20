using BotBuilder.Storage.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace StateManagementBot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddSingleton<IBotFrameworkHttpAdapter, ErrorLogginAdapter>();

            // Create state store using the MongoDB storage component.
            var client = new MongoDB.Driver.MongoClient("mongodb://localhost/");
            var storage = new MongoBotStateStore(new MongoStateStoreSettings(client));


            services.AddSingleton(new UserState(storage));

            services.AddSingleton(new ConversationState(storage));

            services.AddTransient<IBot, StateManagementBot>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

        }
    }
}
