﻿using System.Web.Http;
using GoldenEye.Frontend.Core.Web.Filters;
using GoldenEye.Shared.Core.Configuration;
using Microsoft.Owin.Security.OAuth;

namespace GoldenEye.Frontend.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // Enforce HTTPS
            if (!ConfigHelper.IsInTestMode)
                config.Filters.Add(new RequireHttpsAttribute());
        }
    }
}
