using System;
using RestSharp;

namespace SkytapUtilities.Actions
{
    public class Add : ActionBase
    {
        private static Add _instance;

        public static Add Action => _instance ?? (_instance = new Add());

        public void ConfigToProject(string configId, string projectId)
        {
            Console.Write("Add a configuration ("+configId+") to a project - "+projectId);
            MakeRestRequest("projects/"+projectId+"/configurations/"+configId, Method.POST);
            Console.WriteLine(".... Done");
        }

        public void TemplateToProject(string templateId, string projectId)
        {
            Console.Write("Add a template (" + templateId + ") to a project - " + projectId);
            MakeRestRequest("projects/" + projectId + "/templates/" + templateId, Method.POST);
            Console.WriteLine(".... Done");
        }
    }
}
