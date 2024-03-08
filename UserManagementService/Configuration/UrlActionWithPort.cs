using System;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementService.Configuration
{
    public static class UrlExtensions
    {
        public static string ActionWithPort(this IUrlHelper urlHelper, string action, string controller, object values = null)
        {
            var port = urlHelper.ActionContext.HttpContext.Connection.LocalPort; // Retrieve the port number
            var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            var host = urlHelper.ActionContext.HttpContext.Request.Host.Host;

            // Generate the URL using Url.Action and append the port number
            var urlWithoutPort = urlHelper.Action(action, controller, values);
            // Construct the URL with the port number
            var urlWithPort = $"{scheme}://{host}:{port}{urlWithoutPort}";

            // Make sure to remove the trailing slash if present
            var formattedUrl = urlWithPort.TrimEnd('/');

            return formattedUrl;
        }
        public static string BuildUrl(this IUrlHelper urlHelper, string url, object values = null)
        {
            var port = urlHelper.ActionContext.HttpContext.Connection.LocalPort; // Retrieve the port number
            var scheme = urlHelper.ActionContext.HttpContext.Request.Scheme;
            var host = urlHelper.ActionContext.HttpContext.Request.Host.Host;
            var urlWithPort = $"{scheme}://{host}:{port}{url}";
            var formattedUrl = urlWithPort.TrimEnd('/');

            return formattedUrl;
        }
    }
}

