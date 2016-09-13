using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using SkytapUtilities.Actions;

namespace SkytapUtilities
{
    internal enum CommandActions
    {
        DeleteTemplate,
        DeleteConfig,
        DeleteConfigs,
        NewConfigsAndStart,
        NewConfig,
        CreateTemplate,
        AddVmToConfigFromTemplate,
        FindLatestTemplate,
        AddTemplateToProject
    }
    public static class Program
    {
        private static CommandActions _commandActions;

        private static bool ParseArguments(string[] args)
        {
            if (args.Any(a => a.Equals("help", StringComparison.InvariantCultureIgnoreCase) || a.Equals("?", StringComparison.InvariantCultureIgnoreCase)))
                return false;

            Console.WriteLine("Command line args inputted as such:");
            foreach (var arg in args)
                Console.Write(arg + " ");
            Console.WriteLine();

            foreach (var arg in args)
            {
                string argValue;
                string argName;
                try
                {
                    argName = arg.Split('=')[0].Trim().ToLowerInvariant();
                    argValue = arg.Split('=')[1].Trim();
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("\nError!! You must provide the arguments in the format [argName]=[argValue] or [argName]:[argValue].");
                    return false;
                }

                if (string.IsNullOrEmpty(argName) || string.IsNullOrEmpty(argValue))
                {
                    Console.WriteLine("\nError!! You must provide the arguments in the format [argName]=[argValue] or [argName]:[argValue].");
                    return false;
                }

                // ConfigurationManager.AppSettings[argName] - AppSettings[XXX] is case-insensitive, so it would work.
                switch (argName)
                {
                    case "templateid":
                    case "configid":
                    case "projectid":
                    case "projectidaddto":
                    case "numconfigs":
                        int tempInt;
                        if (int.TryParse(argValue, out tempInt))
                            ConfigurationManager.AppSettings[argName] = argValue;
                        else
                        {
                            Console.WriteLine("\nError!! The argument {0} is invalid! This must be equal to an integer", argName);
                            return false;
                        }
                        break;
                    case "action":
                        switch (argValue.ToLowerInvariant())
                        {
                            case "deleteconfigs":
                                _commandActions = CommandActions.DeleteConfigs;
                                break;
                            case "deleteconfig":
                                _commandActions = CommandActions.DeleteConfig;
                                break;
                            case "deletetemplate":
                                _commandActions = CommandActions.DeleteTemplate;
                                break;
                            case "newconfigsandstart":
                                _commandActions = CommandActions.NewConfigsAndStart;
                                break;
                            case "newconfig":
                                _commandActions = CommandActions.NewConfig;
                                break;
                            case "findlatesttemplate":
                                _commandActions = CommandActions.FindLatestTemplate;
                                break;
                            case "createtemplate":
                                _commandActions = CommandActions.CreateTemplate;
                                break;
                            case "addvmtoconfigfromtemplate":
                                _commandActions = CommandActions.AddVmToConfigFromTemplate;
                                break;
                            case "addtemplatetoproject":
                                _commandActions = CommandActions.AddTemplateToProject;
                                break;
                            default:
                                Console.WriteLine("\nError!! The argument {0} is invalid!", argName);
                                return false;
                        }
                        break;
                    default:
                        {   // catch all - treat it as string
                            ConfigurationManager.AppSettings[argName] = argValue;
                            break;
                        }
                }
            }
            return true;
        }

        private static bool Validate()
        {
            var ret = true;

            if (ConfigurationManager.AppSettings["username"] == null)
            {
                Console.WriteLine("Skytap username is required in App.config");
                ret = false;
            }

            if (ConfigurationManager.AppSettings["APISecurityToken"] == null)
            {
                Console.WriteLine("Skytap APISecurityToken is required in App.config");
                ret = false;
            }

            switch (_commandActions)
            {
                case CommandActions.DeleteTemplate:
                    if (ConfigurationManager.AppSettings["ProjectID"] == null || ConfigurationManager.AppSettings["TemplateName"] == null)
                    {
                        Console.WriteLine("For DeleteTemplate action - ProjectID and TemplateName are both required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.NewConfig:
                    if (ConfigurationManager.AppSettings["TemplateID"] == null || ConfigurationManager.AppSettings["ProjectIDAddTo"] == null ||
                        ConfigurationManager.AppSettings["ConfigName"] == null)
                    {
                        Console.WriteLine("For NewConfig action - TemplateID, ProjectIDAddTo, NumConfig, and ConfigPrefixName are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.NewConfigsAndStart:
                    if (ConfigurationManager.AppSettings["TemplateID"] == null || ConfigurationManager.AppSettings["ProjectIDAddTo"] == null ||
                        ConfigurationManager.AppSettings["NumConfigs"] == null || ConfigurationManager.AppSettings["ConfigPrefixName"] == null)
                    {
                        Console.WriteLine("For NewConfigsAndStart action - TemplateID, ProjectIDAddTo, NumConfig, and ConfigPrefixName are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.DeleteConfigs:
                    if (ConfigurationManager.AppSettings["ProjectID"] == null || ConfigurationManager.AppSettings["ConfigPrefixName"] == null)
                    {
                        Console.WriteLine("For DeleteConfigs action - ProjectID and ConfigPrefixName are both required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.FindLatestTemplate:
                    if (ConfigurationManager.AppSettings["ProjectID"] == null || ConfigurationManager.AppSettings["TemplateSearchTerms"] == null ||
                        ConfigurationManager.AppSettings["PropName"] == null || ConfigurationManager.AppSettings["PropertiesFilePath"] == null)
                    {
                        Console.WriteLine("For NewConfig action - ProjectID, TemplateSearchTerms, PropName, and PropertiesFilePath are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.CreateTemplate:
                    if (ConfigurationManager.AppSettings["ConfigId"] == null || ConfigurationManager.AppSettings["ProjectID"] == null ||
                        ConfigurationManager.AppSettings["ConfigName"] == null)
                    {
                        Console.WriteLine("For CreateTemplate action - ProjectID, ConfigId, and ConfigName are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.AddVmToConfigFromTemplate:
                    if (ConfigurationManager.AppSettings["ConfigId"] == null || ConfigurationManager.AppSettings["TemplateId"] == null)
                    {
                        Console.WriteLine("For AddVmToConfigFromTemplate action - ConfigId and TemplateId are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.DeleteConfig:
                    if (ConfigurationManager.AppSettings["ConfigId"] == null)
                    {
                        Console.WriteLine("For DeleteConfig action - ConfigId is required in the commandline arg.");
                        ret = false;
                    }
                    break;
                case CommandActions.AddTemplateToProject:
                    if (ConfigurationManager.AppSettings["ProjectId"] == null || ConfigurationManager.AppSettings["TemplateId"] == null)
                    {
                        Console.WriteLine("For AddTemplateToProject action - ProjectID and TemplateId are all required in the commandline arg.");
                        ret = false;
                    }
                    break;
                default:
                    Console.WriteLine("Action is required for running SkytapUtilities");
                    ret = false;
                    break;
            }
            return ret;
        }

        private static bool Init()
        {
            if (_commandActions == CommandActions.FindLatestTemplate)
            {
                var dir = Path.GetDirectoryName(ConfigurationManager.AppSettings["propertiesfilepath"]);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if(File.Exists(ConfigurationManager.AppSettings["propertiesfilepath"]))
                    File.Delete(ConfigurationManager.AppSettings["propertiesfilepath"]);
            }
            return true;
        }

        static int Main(string[] args)
        {
            Console.WriteLine("Current Step: Parse commendline arguments.");
            if (!ParseArguments(args))
                return -1;

            Console.WriteLine("Current Step: Make sure condition is valid.");
            if (!Validate())
                return -1;

            Console.WriteLine("Current Step: Init Skytap Utilities.");
            if (!Init())
                return -1;

            JToken token;
            JArray jArray;

            switch (_commandActions)
            {
                case CommandActions.DeleteTemplate:
                {
                    Console.WriteLine("Current Step: Delete template(s) based on template name in specified project.");
                    jArray = QueryInfo.Action.GetTemplatesInfo(ConfigurationManager.AppSettings["projectid"]);
                    var templateId = Helpers.GetIdsByName(jArray, ConfigurationManager.AppSettings["TemplateName"]);
                    foreach (var tempId in templateId)
                        Delete.Action.Template(tempId);
                    break;
                }
                case CommandActions.DeleteConfigs:
                    Console.WriteLine("Current Step: Delete Config(s) based on name prefix in specified project.");
                    jArray = QueryInfo.Action.GetConfigsInfo(ConfigurationManager.AppSettings["projectid"]);
                    var configsId = Helpers.GetIdsByName(jArray, ConfigurationManager.AppSettings["ConfigPrefixName"], false);
                    foreach (var id in configsId)
                        Delete.Action.Config(id);
                    break;
                case CommandActions.NewConfig:
                    {
                        Console.WriteLine("Current Step: Create new configurations.");
                        var configName = ConfigurationManager.AppSettings["ConfigName"];
                        token = Create.Action.Config(ConfigurationManager.AppSettings["TemplateID"], configName);
                        Add.Action.ConfigToProject(token.Value<string>("id"), ConfigurationManager.AppSettings["ProjectIDAddTo"]);
                        break;
                    }
                case CommandActions.NewConfigsAndStart:
                {
                    Console.WriteLine("Current Step: Create new configurations.");
                    var ids = new List<string>();
                    var configName = ConfigurationManager.AppSettings["ConfigPrefixName"];
                    
                    var num = int.Parse(ConfigurationManager.AppSettings["numconfigs"]);
                    
                    for (var i = 0; i < num; i++)
                    {
                        var name = num == 1 ? configName : configName + " " + (i + 1);
                        token = Create.Action.Config(ConfigurationManager.AppSettings["TemplateID"], name);
                        Add.Action.ConfigToProject(token.Value<string>("id"), ConfigurationManager.AppSettings["ProjectIDAddTo"]);
                        ids.Add(token.Value<string>("id"));
                        Edit.Action.AutoSuspend(token.Value<string>("id"), 14400);  // Make Auto suspend to 4hrs
                    }
                    foreach (var id in ids)
                    {// start all the VMs
                        Edit.Action.StartConfig(id);
                    }
                    Helpers.WaitForConfigsNotBusy(ids.ToList());
                    break;
                }
                case CommandActions.FindLatestTemplate:
                    Console.WriteLine("Current Step: Find template Id in specified project with search terms");
                    jArray = QueryInfo.Action.GetTemplatesInfo(ConfigurationManager.AppSettings["projectid"]);
                    var tempIdToFile = Helpers.GetIdsLatestWithSearchTerms(jArray, ConfigurationManager.AppSettings["TemplateSearchTerms"].Split(','));
                    var lineToWrite = ConfigurationManager.AppSettings["PropName"]+"="+tempIdToFile;
                    File.WriteAllText(ConfigurationManager.AppSettings["PropertiesFilePath"], lineToWrite);
                    break;
                case CommandActions.CreateTemplate:
                    Console.WriteLine("Current Step: Create Template from Config");
                    Edit.Action.SuspendConfig(ConfigurationManager.AppSettings["configid"]);
                    Helpers.WaitForConfigsNotBusy(new List<string>() { ConfigurationManager.AppSettings["configid"] });

                    token = Create.Action.Template(ConfigurationManager.AppSettings["configid"], ConfigurationManager.AppSettings["ConfigName"]);
                    Add.Action.TemplateToProject(token.Value<string>("id"), ConfigurationManager.AppSettings["ProjectID"]);
                    break;
                case CommandActions.AddVmToConfigFromTemplate:
                    Console.WriteLine("Current Step: Add a VM from Template to a Config");
                    Edit.Action.AddVMsFromTemplateToConfig(ConfigurationManager.AppSettings["configid"], ConfigurationManager.AppSettings["templateId"]);
                    break;
                case CommandActions.DeleteConfig:
                    Console.WriteLine("Current Step: Delete a config");
                    Delete.Action.Config(ConfigurationManager.AppSettings["configid"]);
                    break;
                case CommandActions.AddTemplateToProject:
                    Console.WriteLine("Current Step: Add Template to a Project");
                    Add.Action.TemplateToProject(ConfigurationManager.AppSettings["templateId"], ConfigurationManager.AppSettings["projectid"]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return 0;
        }
    }
}
