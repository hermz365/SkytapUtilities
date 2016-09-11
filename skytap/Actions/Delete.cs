using System;
using RestSharp;

namespace SkytapUtilities.Actions
{
    public class Delete : ActionBase
    {
        private static Delete _instance;

        public static Delete Action => _instance ?? (_instance = new Delete());

        public void Template(string templateId)
        {
            Console.Write("Deleting the template - " + templateId);
            MakeRestRequest("templates/" + templateId, Method.DELETE);
            Console.WriteLine(".... Done");
        }

        public void Config(string configId)
        {
            Console.Write("Deleting the config - "+ configId);
            MakeRestRequest("configurations/" + configId, Method.DELETE);
            Console.WriteLine(".... Done");
        }
    }
}
