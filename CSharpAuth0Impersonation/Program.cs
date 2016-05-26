using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;
namespace CSharpAuth0Impersonation
{
    class Program
    {
        static void Main(string[] args)
        {
            var domain = "YOUR_DOMAIN.au.auth0.com";
            var globalClientId = "YOUR_GLOBAL_CLIENT_ID";
            var globalClientSecret = "YOUR_GLOBAL_CLIENT_SECRET";

            var impersonatorId = "USER_ID_OF_THE_IMPERSONATOR";
            var targetUserId = "USER_ID_OF_THE_USER_YOU_ARE_TRYING_TO_IMPERSONATE";
            var targetClientId = "CLIENT_ID_OF_THE_APPLICATION_YOU_WANT_TO_LOGIN_TO";
            var callbackUrl = "http://localhost:3000/callback";
            var impersonationLinkLifetime = 30; // seconds 


            // Authenticate. 
            var client = new HttpClient();
            var message = client.PostAsync(String.Format("https://{0}/oauth/token", domain),
                new StringContent(JsonConvert.SerializeObject(new { client_id = globalClientId, client_secret = globalClientSecret, grant_type = "client_credentials" }), Encoding.UTF8, "application/json")).Result;
            var tokenString = JsonConvert.DeserializeObject<Dictionary<string, string>>(message.Content.ReadAsStringAsync().Result);
            var token = tokenString["access_token"];

            // Get the impersonation url. 
            var impersonationBody = new
            {
                ttl = impersonationLinkLifetime,
                protocol = "oauth2",
                impersonator_id = impersonatorId,
                client_id = targetClientId,
                additionalParameters = new
                {
                    callback_url = callbackUrl
                }
            };
            var request = new HttpRequestMessage() { RequestUri = new Uri(String.Format("https://{0}/users/{1}/impersonate", domain, targetUserId)), Method = HttpMethod.Post };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(impersonationBody), Encoding.UTF8, "application/json");
            var url = client.SendAsync(request).Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(url);
            Trace.WriteLine(url);
            Console.ReadLine();

        }
    }
}
