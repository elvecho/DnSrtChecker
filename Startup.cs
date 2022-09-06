using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DnSrtChecker.Persistence;
using DnSrtChecker.MappingProfile;
using AutoMapper;
using DnSrtChecker.Models;
using Newtonsoft.Json;
using System.Globalization;
using DnSrtChecker.Services;
using DnSrtChecker.ModelsHelper;

namespace DnSrtChecker
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

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });

            services.AddScoped<ITransmissionRepository, TransmissionRepository>();
            services.AddScoped<ITransactionRtErrorRepository, TransactionRtErrorRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRtServerRepository, RtServerRepository>();
            services.AddScoped<IStoreGroupRepository, StoreGroupRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<RT_ChecksContext>(options =>
            
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.CommandTimeout(300))
                
        
                );//, options => options.EnableRetryOnFailure())
            services.Configure<Properties>(Configuration.GetSection("Properties"));
            services.AddOptions();
            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<RT_ChecksContext>()
                .AddDefaultTokenProviders();
            services.AddTransient<EmailHelper>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .AddRazorPagesOptions(options => {
                    options.Conventions.AddPageRoute("/Home/Index", "");
                    //options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var cultureInfo = new CultureInfo("it-IT"); 
            cultureInfo.NumberFormat.CurrencyDecimalSeparator = ",";
            cultureInfo.NumberFormat.CurrencyDecimalDigits = 2;
            cultureInfo.NumberFormat.NumberGroupSeparator = " ";
            cultureInfo.NumberFormat.CurrencyGroupSeparator = ".";
          
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
         

            services.AddMvc();
            services.AddMemoryCache();
            services.AddAutoMapper(typeof(Startup));
            var config = new AutoMapper.MapperConfiguration(cfg =>
                                                            cfg.AddProfile(new MappingEntity()));
            var mapper = config.CreateMapper();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/Identity/Account/Login");
            services.AddControllersWithViews().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                app.UseExceptionHandler("/Home/Index");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseCookiePolicy();

            Seed.SeedUsers(userManager, roleManager);
        }
    }
}
