using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using RestSharp;

namespace SkytapUtilities.Actions
{
    public class Parameter
    {
        public readonly string Name;
        public readonly object Value;

        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }

    public abstract class ActionBase
    {
        internal IRestResponse MakeRestRequest(string resource, Method method = Method.GET, params Parameter[] parameters)
        {
            int timeout = int.Parse(ConfigurationManager.AppSettings["Timeout"]);
            IRestResponse response;
            var request = new RestRequest(resource, method);

            foreach (var p in parameters)
                request.AddParameter(p.Name, p.Value);

            var i = 0;
            while (true)
            {// Include retry-logic
                response = SkytapRestClient.Instance.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                    break;

                if(i == timeout)
                    throw new TimeoutException();

                // Resource is busy wait http://help.skytap.com/api-busy-bp.html 
                if ((int) response.StatusCode == 429 || (int) response.StatusCode == 423 || (int)response.StatusCode == 422)
                {
                    Console.WriteLine("Resource is busy. Wait for 30 sec");
                    Thread.Sleep(new TimeSpan(0, 0, 30));
                    i++;
                    continue;
                }

                // Everything else. Throw exception!
                throw new HttpRequestException("Return code : " + response.StatusCode);
            }
            
            return response;
        }
    }
}
