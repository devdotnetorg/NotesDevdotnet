using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Notes.Devdotnet.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Notes.Devdotnet.Server.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Notes.Devdotnet.Server.Services;
using System;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Notes.Devdotnet.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Change folder for appsettings.json
            var builder = new ConfigurationBuilder()
                .AddJsonFile("config/appsettings.json", optional: false, reloadOnChange: true);
            configuration = builder.Build();
            //
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            //add support DataBase
            switch (appSettings.UseDB.ToLower())
            {
                case "postgresql":
                    services.AddEntityFrameworkNpgsql().AddDbContext<ApplicationDbContext>(opt => {
                        opt.UseLazyLoadingProxies()
                        .UseNpgsql(appSettings.DBConection);
                    }, ServiceLifetime.Singleton);
                    break;
                case "mariadb":
                    //TODO: Add MariaDB

                    break;
                default:
                    {
                        Environment.Exit(2);                        
                        break;
                    }
            }
            //add Identity
            services.AddDefaultIdentity<IdentityUser>()
              .AddEntityFrameworkStores<ApplicationDbContext>();
            //Policy for new user
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireUppercase =false;
                // Default SignIn settings.
                options.SignIn.RequireConfirmedEmail = false;
                // Default User settings.
                options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                options.User.RequireUniqueEmail = true;
            });
            // add AuthenticateService
            services.AddScoped<IAuthenticateService, AuthenticationJWTService>();
            //Get IssuerSigningKey from Service AuthenticationJWTService
            // Build an intermediate service provider
            var sp = services.BuildServiceProvider();
            // Resolve the services from the service provider
            var localAuthenticationJWTService = sp.GetService<IAuthenticateService>();
            //Settings jwt authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                // укзывает, будет ли валидироваться издатель при валидации токена
                ValidateIssuer = true,
                // будет ли валидироваться потребитель токена
                ValidateAudience = true,
                // будет ли валидироваться время существования
                ValidateLifetime = true,
                // валидация ключа безопасности
                ValidateIssuerSigningKey = true,
                // строка, представляющая издателя
                ValidIssuer = appSettings.JWT.JwtIssuer,
                // установка потребителя токена
                ValidAudience = appSettings.JWT.JwtAudience,
                // установка ключа безопасности
                IssuerSigningKey = localAuthenticationJWTService.signingKey
            };
        });
            

            //add global Configuration
            //services.AddSingleton<IConfiguration>(Configuration);
            //add Controllers
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            //add Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            //

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
        private void OnShutdown()
        {
            //this code is called when the application stops
        }
    }
}
