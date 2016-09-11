﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace SkytapUtilities.Actions
{
    public class Edit : ActionBase
    {
        private static Edit _instance;

        public static Edit Action => _instance ?? (_instance = new Edit());

        public void AutoSuspend(string configId, int sec)
        {
            Console.Write("Edit a configuration's ("+configId+") auto-suspend setting to " + sec + "sec");
            MakeRestRequest("configurations/" + configId +".json", Method.PUT, new Parameter("suspend_on_idle", sec));
            Console.WriteLine(".... Done");
        }

        public void StartConfig(string configId)
        {
            Console.Write("Start a config - " + configId);
            MakeRestRequest("configurations/" + configId + ".json", Method.PUT, new Parameter("runstate", "running"));
            Console.WriteLine(".... Done");
        }
        
    }
}