using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SampleDeliveryService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleDeliveryService.Services;
using Microsoft.AspNetCore.Localization;

namespace SampleDeliveryService
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
            // ✅ Add culture fallback
            var defaultCulture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            services.Configure<RequestLocalizationOptions>(opts => {
                opts.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
                opts.SupportedCultures = localizationOptions.SupportedCultures;
                opts.SupportedUICultures = localizationOptions.SupportedUICultures;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("LocalAzure",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost", "http://<App Service URL>")
                               .WithMethods("GET");
                    });
            });

            services.AddTransient<TokenAuthorizationProvider>();

            services.AddControllers();
            services.AddMvc().AddRazorRuntimeCompilation();

            // 🧪 Use fake Cosmos service for now
            services.AddSingleton<ICosmosDbService, FakeCosmosDbService>();

            // Uncomment for actual CosmosDB
            // services.AddSingleton<ICosmosDbService>(
            //     InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    IssuerSigningKey = TokenAuthorizationProvider.CreateSecurityKey(),
                    ValidIssuer = TokenAuthorizationProvider.Issuer,
                    ValidAudience = TokenAuthorizationProvider.Audience
                };
            });

            services.AddAuthorization(options =>
            {
                AuthorizationPolicyBuilder builder = new AuthorizationPolicyBuilder("Bearer");
                options.AddPolicy("SessionToken", builder.RequireAuthenticatedUser().Build());
            });
        }

        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;

            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // ✅ Apply culture localization
            var localizationOptions = app.ApplicationServices.GetService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Orders/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Orders}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
