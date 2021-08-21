using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Persistence;

namespace IdentityServer
{
    public static class Startup
    {
        public static void ConfigureIdentity(IServiceCollection service)
        {
            var builder = service.AddIdentityCore<ApplicationUser>();

            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddSignInManager<SignInManager<ApplicationUser>>();

        }
    }
}
