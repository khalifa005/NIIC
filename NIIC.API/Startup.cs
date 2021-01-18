using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Activities;
using Application.Interfaces;
using Domains.Identity;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NIIC.API.Mail.BMail;
using NIIC.API.Mail.BMail.MailServices;
using NIIC.API.Mail.MailKit;
using NIIC.API.Middelware;
using NIIC.Application.ApplicationSettings;
using Persistence;
using Scrutor;

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

            #region Mail

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddSingleton<IMailer, Mailer>();

            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            #endregion

            #region Db

            //services.AddDbContext<DataContext>(opt =>
            //{
            //    opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
            //});
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));

            #endregion

            //Application.ConfigureServices.Localization(services);
            services.Configure<Jwt>(Configuration.GetSection("JWT"));

            services.AddMediatR(typeof(GetActivitiesList.Handler).Assembly);

            services.AddControllers(opt =>
            {
                //insure every request require authenticated user 
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddFluentValidation(conf =>
                    conf.RegisterValidatorsFromAssemblyContaining<CreateActivity>());

            services.TryAddSingleton<ISystemClock, SystemClock>();

            var builder = services.AddIdentityCore<AppUser>();

            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();

            //convert string to bytes array Encoding.UTF8.GetBytes("super security key")
            //with this key anyone can generate valid token 
            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super security key"));
            //define JwtBearerDefaults scheme that we use 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    //validate incoming key from token 
                    ValidateIssuerSigningKey = true,
                    //current key
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                    //local host urls issue url and receiving  url
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            services.AddScoped<IJwtGenerator, JwtGenerator>();

            services.Scan(scan =>
            {
                scan.FromAssemblyOf<NoOp>()
                    .AddClasses(classes => classes.WithAttribute<CreateSingletonAttribute>(x => x.IsImplementingInterface == false))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithSingletonLifetime()
                    .AddClasses(classes => classes.WithAttribute<CreateInScopeAttribute>(x => x.IsImplementingInterface == false))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithScopedLifetime()
                    .AddClasses(classes => classes.WithAttribute<CreateTransientAttribute>(x => x.IsImplementingInterface == false))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip).AsSelf().WithTransientLifetime();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //custom filter
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); //check if user exist in the system

            app.UseAuthorization(); // check for user roles  

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Values", action = "Get" });

                endpoints.MapControllers();
            });
        }

        //public static void Localization(IServiceCollection services)
        //{
        //    var supportedCultures = new List<CultureInfo>
        //    {
        //        new CultureInfo("en"),
        //        new CultureInfo("ar"),
        //    };

        //    services.Configure<RequestLocalizationOptions>(options =>
        //    {
        //        options.DefaultRequestCulture = new RequestCulture("en");
        //        options.SupportedCultures = supportedCultures;
        //        options.SupportedUICultures = supportedCultures;
        //        options.RequestCultureProviders.Clear();
        //        options.RequestCultureProviders.Add(new CookieRequestCultureProvider());
        //    });
        //}
    }
}
