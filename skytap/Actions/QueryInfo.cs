using System;
using Newtonsoft.Json.Linq;

namespace SkytapUtilities.Actions
{
    public class QueryInfo : ActionBase
    {
        private static QueryInfo _instance;

        public static QueryInfo Action => _instance ?? (_instance = new QueryInfo());

        public JArray GetTemplatesInfo(string projectId)
        {
            Console.Write("Requesting the list of templates in project " + projectId);
            var response = MakeRestRequest("projects/" + projectId + "/templates");
            Console.WriteLine(".... Done");
            return JArray.Parse(response.Content);
        }

        public JArray GetConfigsInfo(string projectId)
        {
            Console.Write("Requesting the list of configs in project " + projectId);
            var response = MakeRestRequest("projects/" + projectId + "/configurations");
            Console.WriteLine(".... Done");
            return JArray.Parse(response.Content);
        }

        public JToken Config(string configId)
        {
            Console.Write("Query a config - "+configId);
            var response = MakeRestRequest("configurations/"+configId+".json");
            Console.WriteLine(".... Done");
            return JToken.Parse(response.Content);
        }
    }
}
