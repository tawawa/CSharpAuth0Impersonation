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
            var domain = "vjayaram.au.auth0.com";
            var globalClientId = "oXP14y4oJP6rlKYShx2WlJO6pEw1fLXr";
            var globalClientSecret = "rKPbr5Petl3sPe3aY3EMRDp4mI9jM4fSk-0lTPaa2YPYjuvrED6_L5rnNK8J31g5";

            var impersonatorId = "auth0|56ce383580ef393b7c76387f";
            var targetUserId = "auth0|56ccc6faab485b8749f3adc0";
            var targetClientId = "S2iThBUtkas0ZqHNwfRK2pUUsRAsVXmf";
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
