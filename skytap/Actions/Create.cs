using System;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace SkytapUtilities.Actions
{
    public class Create : ActionBase
    {
        private static Create _instance;

        public static Create Action => _instance ?? (_instance = new Create());

        public JToken Config(string templateId, string name)
        {
            Console.Write("Create a new config from template (" +templateId+") with name "+name);
            var parameters = new[] {new Parameter("name", name), new Parameter("template_id", templateId)};
            var response = MakeRestRequest("configurations.json", Method.POST, parameters);
            Console.WriteLine(".... Done");
            return JToken.Parse(response.Content);
        }
    }
}
