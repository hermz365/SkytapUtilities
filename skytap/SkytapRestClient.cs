using System;
using System.Configuration;
using RestSharp;
using RestSharp.Authenticators;

namespace SkytapUtilities
{
    // Singleton Rest Client class that only connects to Skytap
    public static class SkytapRestClient
    {
        private static RestClient _instance;

        public static RestClient Instance => _instance ?? (_instance = new RestClient
        {
            BaseUrl = new Uri("https://cloud.skytap.com"),
            Authenticator = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["APISecurityToken"])
        });
    }
}


