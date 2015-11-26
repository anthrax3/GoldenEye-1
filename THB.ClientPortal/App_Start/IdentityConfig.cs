﻿using System;
using System.Threading.Tasks;
using Backend.Business.Services;
using Frontend.Web.App_Start;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Frontend.Web.Models;
using Ninject;

namespace Frontend.Web
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                //RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        /// <summary>
        /// Finds existing username with password, if not exists checks if external authorization service 
        /// allows to authorize. If yes, creates new user.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override async Task<ApplicationUser> FindAsync(string userName, string password)
        {
            var user = await base.FindAsync(userName, password);

            if (user != null) return user;

            var externalAuthorizationService = NinjectWebCommon.Kernel.Get<IAuthorizationService>();


            if (externalAuthorizationService == null
                || !externalAuthorizationService.Authorize(userName, password))
                return null;

            var externalUser = externalAuthorizationService.Find(userName, password);

            user = new ApplicationUser { UserName = externalUser.UserName, Email = externalUser.Email };

            var result = await CreateAsync(user, password);

            if (!result.Succeeded)
                return null;

            return await base.FindAsync(userName, password);
        }
    }
}
