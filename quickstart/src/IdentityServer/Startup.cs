// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);


            services.AddAuthentication()
                //Google Login Handler
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "<insert here>";
                    options.ClientSecret = "<insert here>";
                })

                //Azure Login Handler https://www.ashleyhollis.com/how-to-configure-azure-active-directory-with-identityserver4
                .AddOpenIdConnect("aad", "Azure AD", options =>
                 {
                     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                     options.SignOutScheme = IdentityServerConstants.SignoutScheme;

                     options.Authority = "https://login.windows.net/<Directory (tenant) ID>";
                     options.ClientId = "<Your Application (client) ID>";
                     options.ResponseType = OpenIdConnectResponseType.IdToken;
                     options.CallbackPath = "/signin-aad";
                     options.SignedOutCallbackPath = "/signout-callback-aad";
                     options.RemoteSignOutPath = "/signout-aad";
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         NameClaimType = "name",
                         RoleClaimType = "role"
                     };
                 });


        }



        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to add MVC
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseIdentityServer();

            // uncomment, if you want to add MVC
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
