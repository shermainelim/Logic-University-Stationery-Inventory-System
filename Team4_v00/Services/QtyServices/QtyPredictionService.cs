using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;



// This code requires the Nuget package Microsoft.AspNet.WebApi.Client to be installed.
// Instructions for doing this in Visual Studio:
// Tools -> Nuget Package Manager -> Package Manager Console
// Install-Package Microsoft.AspNet.WebApi.Client

namespace Ben_Project.Services.QtyServices
{
    public class QtyPredictionService
    {

        public async Task<string> QtyPredict(string item_category, string item_ID, string date, string IsHoliday)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "Store", item_category
                                            },
                                            {
                                                "Dept", item_ID
                                            },
                                            {
                                                "Date", date
                                            },
                                            {
                                                "IsHoliday", IsHoliday
                                            },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "G+THPwarr2qvfXLEZ3CAgRoUbnTISb/PooHueQOXo2VBmxdx0ybVHDlQHuBmgLYH0ycIOvbH5hqjmZ08mLUBHg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/9c7eacb777ca45079d3a311454a48511/services/c284dfa711c8462488effaea2d008d1f/execute?api-version=2.0&format=swagger");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)

                //HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);

                HttpResponseMessage response = await client
        .PostAsync("", new StringContent(JsonConvert.SerializeObject(scoreRequest), Encoding.UTF8, "application/json"))
        .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    return string.Format("The request failed with status code: {0}", response.StatusCode);

                }
            }
        }
    }
}
