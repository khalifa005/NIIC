using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Domains.Identity;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NIIC.API.BMail;
using NIIC.API.BMail.MailServices;
using NIIC.API.MailKit;
using NIIC.API.Middelware;
using Persistence;

namespace NIIC.API
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

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddSingleton<IMailer, Mailer>();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            //services.AddDbContext<DataContext>(opt =>
            //{
            //    opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            //});
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));

            Application.ConfigureServices.Localization(services);

            services.AddMediatR(typeof(GetActivitiesList.Handler).Assembly);

            services.AddControllers()
                .AddFluentValidation(conf=>
                    conf.RegisterValidatorsFromAssemblyContaining<CreateActivity>());

            services.TryAddSingleton<ISystemClock, SystemClock>();
            var builder = services.AddIdentityCore<AppUser>();

            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Values", action = "Get" });

                endpoints.MapControllers();
            });
        }

        public static void Localization(IServiceCollection services)
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en"),
                new CultureInfo("ar"),
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Clear();
                options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
            });
        }
    }
}
