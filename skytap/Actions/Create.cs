using System;
using System.Configuration;
using System.IO;
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
            var jsonDir = ConfigurationManager.AppSettings["SaveToJsonDir"];
            var parameters = new[] {new Parameter("name", name), new Parameter("template_id", templateId)};
            var response = MakeRestRequest("configurations.json", Method.POST, parameters);
            if (jsonDir != null)
                File.WriteAllText(Path.Combine(jsonDir, name + ".json"), response.Content);
            Console.WriteLine(".... Done");
            return JToken.Parse(response.Content);
        }

        public JToken Template(string configId, string name)
        {
            Console.Write("Create a new template from config (" + configId + ") with name " + name);
            var jsonDir = ConfigurationManager.AppSettings["SaveToJsonDir"];
            var parameters = new[] { new Parameter("name", name), new Parameter("configuration_id", configId) };
            var response = MakeRestRequest("templates.json", Method.POST, parameters);
            if (jsonDir != null)
                File.WriteAllText(Path.Combine(jsonDir, name + ".json"), response.Content);
            Console.WriteLine(".... Done");
            return JToken.Parse(response.Content);
        }
    }
}
