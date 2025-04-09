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
            // ✅ Add safe culture fallback to avoid CultureNotFoundException
            var defaultCulture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                opts.DefaultRequestCulture = localizationOptions.DefaultRequestCulture;
                opts.SupportedCultures = localizationOptions.SupportedCultures;
                opts.SupportedUICultures = localizationOptions.SupportedUICultures;

                // ❗ Optional: Clear culture detection from headers if you're still getting bad inputs
                // opts.RequestCultureProviders.Clear();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("LocalAzure", builder =>
                {
                    builder.WithOrigins("http://localhost", "http://<App Service URL>")
                           .WithMethods("GET");
                });
            });

            services.AddTransient<TokenAuthorizationProvider>();
            services.AddControllers();
            services.AddMvc().AddRazorRuntimeCompilation();

            // 🔧 Temporary fallback service
            services.AddSingleton<ICosmosDbService, FakeCosmosDbService>();

            // ✅ Enable this when using actual Cosmos DB
            // services.AddSingleton<ICosmosDbService>(
            //     InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());

            services.AddAuthentication("Bearer").AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    IssuerSigningKey = TokenAuthorizationProvider.CreateSecurityKey(),
                    ValidIssuer = TokenAuthorizationProvider.Issuer,
                    ValidAudience = TokenAuthorizationProvider.Audience
                };
            });

            services.AddAuthorization(options =>
            {
                var builder = new AuthorizationPolicyBuilder("Bearer");
                options.AddPolicy("SessionToken", builder.RequireAuthenticatedUser().Build());
            });
        }

        private static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection["DatabaseName"];
            string containerName = configurationSection["ContainerName"];
            string account = configurationSection["Account"];
            string key = configurationSection["Key"];

            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            // ✅ Register culture fallback
            var localizationOptions = app.ApplicationServices
                .GetService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
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
