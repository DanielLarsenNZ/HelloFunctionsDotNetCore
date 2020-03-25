using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelloFunctionsDotNetCore
{
    public static class GetHealth
    {
        private static readonly Lazy<HttpClient> _lazyHttp = new Lazy<HttpClient>();
        private static HttpClient Http => _lazyHttp.Value;

        [FunctionName(nameof(GetHealth))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var result = new Dictionary<string, object>();

            result.Add("Request.Host", req.Host);
            result.Add(
                "Request.HttpContext.Connection.RemoteIpAddress",
                req.HttpContext.Connection.RemoteIpAddress.ToString());
            result.Add(
                "Request.HttpContext.Connection.LocalIpAddress",
                req.HttpContext.Connection.LocalIpAddress.ToString());

            await PingUrl(Http, result, new Uri("https://www.microsoft.com/"));

            return new JsonResult(result);
        }

        private static async Task PingUrl(HttpClient http, Dictionary<string, object> result, Uri uri)
        {
            //TODO: reusing HttpClient here is not very efficient?

            var responseResult = new Dictionary<string, object>();
            responseResult.Add("URL", uri.ToString());

            try
            {
                var response = await http.GetAsync(uri);
                responseResult.Add("Response.StatusCode", response.StatusCode);
                responseResult.Add("Response.ReasonPhrase", response.ReasonPhrase);
                responseResult.Add("Response.IsSuccessStatusCode", response.IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                responseResult.Add("Exception.GetType().FullName", ex.GetType().FullName);
                responseResult.Add("Exception.Message", ex.Message);
            }

            result.Add(uri.Host, responseResult);
        }
    }
}
