using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
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

        public static void EnsureAllConfigsAreRunning(List<string> ids)
        {
            var i = 0;
            var configsTobeQuery = ids.ToList();

            while (true)
            {
                var configsToStart = new List<string>();
                foreach (var id in configsTobeQuery)
                {
                    var c = QueryInfo.Action.Config(id);
                    Console.WriteLine("name: " + c.Value<string>("name") + " runstate: " + c.Value<string>("runstate"));
                    if (!c.Value<string>("runstate").Equals("running", StringComparison.CurrentCultureIgnoreCase))
                        configsToStart.Add(id);
                }

                if (configsToStart.Count == 0)
                {
                    Console.WriteLine("All configs started");
                    break;
                }
                else
                {
                    foreach (var s in configsToStart)
                    {
                        Edit.Action.StartConfig(s);
                    }
                    WaitForConfigsNotBusy(configsToStart);
                }
            }
        }

        public static void WaitForConfigsNotBusy(List<string> ids)
        {
            var i = 0;

            while (true)
            {
                List<string> busyConfigs, notBusyConfigs;
                Helpers.QueryNotBusyRunStateConfigs(ids, out busyConfigs, out notBusyConfigs);
                if (notBusyConfigs.Count != ids.Count)
                {
                    ids = busyConfigs;

                    if (busyConfigs.Count == 1)
                    {// When there is only one left. Poll every 30 sec 
                        Console.WriteLine(busyConfigs.Count + " config is still busy. Wait for 30sec");
                        Thread.Sleep(new TimeSpan(0, 0, 30));
                    }
                    else
                    {// One machine / minute
                        Console.WriteLine(busyConfigs.Count + " configs are still busy. Wait for " + busyConfigs.Count + " mins");
                        Thread.Sleep(new TimeSpan(0, busyConfigs.Count, 0));
                    }
                }
                else
                {
                    Console.WriteLine("All configs not busy.");
                    break;
                }

                if (i == int.MaxValue)
                {
                    Console.WriteLine("Have to get out at some point. Can't stay in this loop forever.");
                    throw new TimeoutException();
                }
                i++;
            }
        }

        private static void QueryNotBusyRunStateConfigs(List<string> configsTobeQuery, out List<string> busyConfigs, out List<string> notBusyConfigs)
        {
            busyConfigs = new List<string>();
            notBusyConfigs = new List<string>();

            foreach (var id in configsTobeQuery)
            {
                var c = QueryInfo.Action.Config(id);
                Console.WriteLine("name: " + c.Value<string>("name") + " runstate: " + c.Value<string>("runstate"));
                if (c.Value<string>("runstate").Equals("busy", StringComparison.CurrentCultureIgnoreCase))
                    busyConfigs.Add(id);
                else
                    notBusyConfigs.Add(id);
            }
        }
    }
}
