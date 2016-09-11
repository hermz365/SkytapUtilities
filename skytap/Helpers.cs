using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using SkytapUtilities.Actions;

namespace SkytapUtilities
{
    public static class Helpers
    {
        public static List<string> GetIdsByName(JArray arr, string nameToMatch, bool isExactMatch = true)
        {
            return arr.Children<JObject>()
                .Where(o => isExactMatch
                            ? o["name"].ToString().Equals(nameToMatch, StringComparison.CurrentCultureIgnoreCase)
                            : o["name"].ToString().StartsWith(nameToMatch, StringComparison.CurrentCultureIgnoreCase))
                .Select(o => o["id"].ToString())
                .ToList();
        }

        public static string GetIdsLatestWithSearchTerms(JArray arr, string[] searchTerms)
        {
            var tempList = new List<int>();
            foreach (var t in arr)
            {
                var name = t["name"].ToString().ToLowerInvariant();
                if (searchTerms.All(term => name.Contains(term.ToLowerInvariant())))
                    tempList.Add(int.Parse(t["id"].ToString()));
            }

            if (tempList.Count == 0)
            {
                throw new Exception("Template not found!");
            }

            return tempList.Max().ToString();
        }

        public static void QueryNotBusyRunStateConfigs(List<string> configsTobeQuery, out List<string> busyConfigs, out List<string> notBusyConfigs)
        {
            busyConfigs = new List<string>();
            notBusyConfigs = new List<string>();

            foreach (var id in configsTobeQuery)
            {
                var c = QueryInfo.Action.Config(id);
                Console.WriteLine("name: " + c.Value<string>("name") + " runstate: " + c.Value<string>("runstate"));
                if(c.Value<string>("runstate").Equals("busy", StringComparison.CurrentCultureIgnoreCase))
                    busyConfigs.Add(id);
                else
                    notBusyConfigs.Add(id);
            }
        }
    }
}
