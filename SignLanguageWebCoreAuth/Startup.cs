using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignLanguageWebCoreAuth.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.IO;
using SignLanguageWebCoreAuth.Models;
using SignLanguageWebCoreAuth.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using SignLanguageSimplification.SimplificationAlgorithm;
using SignLanguageWebCoreAuth.SimplificationAlgorithm;
using Synonyms = SignLanguageSimplification.SimplificationAlgorithm.Synonyms;

namespace SignLanguageWebCoreAuth
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        public static string ConnectionString { get; private set; }
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)// optional extra provider
                .AddEnvironmentVariables();
            


            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySQL(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<Users>()
            //    .AddDefaultUI(UIFramework.Bootstrap4)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Admin", policy =>
            //                      policy.RequireClaim("Admin", "true"));
            //});
            services.AddSingleton<IAuthorizationHandler, HasRoleHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("IsItAdmin", policy =>
                    policy.Requirements.Add(new HasRoleRequirement("Admin")));
                options.AddPolicy("IsItAuth", policy =>
                    policy.Requirements.Add(new HasRoleRequirement("User")));
            });
            services.AddAuthentication("Cookies")
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.SlidingExpiration = true;
                    options.ExpireTimeSpan = TimeSpan.FromDays(365);
                    options.Cookie.Expiration = TimeSpan.FromDays(365);
                    options.Cookie.MaxAge = TimeSpan.FromDays(365);
                    options.Cookie.IsEssential = true;
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<IInfinitive, Infinitive>();
            services.AddSingleton<ILemmatization, Lemmatization>();
            services.AddSingleton<IPhraseSynonyms, PhraseSynonyms>();
            services.AddSingleton<ISentenceSplitting, SentenceSplitting>();
            services.AddSingleton<IStopWordsRemoval, StopWordsRemoval>();
            services.AddSingleton<ITenseRecognition, TenseRecognition>();
            services.AddSingleton<ISentenceSubsplitting, SentenceSubSplitting>();
            services.AddSingleton<ISynonyms, Synonyms>();
            services.AddSingleton<IPluralToSingular, PluralToSinular>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            ConnectionString = Configuration["ConnectionStrings:DefaultConnection"];

            loggerFactory.AddFile("/var/sites/signlanguage/logs/sign-language-{Date}.txt");
        }
    }
}
