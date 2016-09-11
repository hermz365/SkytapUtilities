using System;
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
            IRestResponse response;
            var request = new RestRequest(resource, method);

            foreach (var p in parameters)
                request.AddParameter(p.Name, p.Value);

            while (true)
            {// Include retry-logic
                response = SkytapRestClient.Instance.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                    break;

                // Resource is busy wait http://help.skytap.com/api-busy-bp.html 
                if ((int) response.StatusCode == 429 || (int) response.StatusCode == 423 || (int)response.StatusCode == 422)
                {
                    Console.WriteLine("Resource is busy. Wait for 1 min");
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                    continue;
                }

                // Everything else. Throw exception!
                throw new HttpRequestException("Return code : " + response.StatusCode);
            }
            
            return response;
        }
    }
}
