using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;

namespace YnabMcpServer.Tools
{
    public class YnabApiTester
    {
        public static async Task TestYnabApiConnection(string apiKey)
        {
            Console.WriteLine("Testing YNAB API connection...");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Test User Info endpoint
            try
            {
                Console.WriteLine("Fetching user info...");
                var userResponse = await httpClient.GetAsync("https://api.youneedabudget.com/v1/user");
                await LogResponse(userResponse, "User Info");

                if (userResponse.IsSuccessStatusCode)
                {
                    // If user info succeeded, try getting budgets
                    Console.WriteLine("\nFetching budgets...");
                    var budgetsResponse = await httpClient.GetAsync("https://api.youneedabudget.com/v1/budgets");
                    await LogResponse(budgetsResponse, "Budgets");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static async Task LogResponse(HttpResponseMessage response, string endpointName)
        {
            Console.WriteLine($"{endpointName} Response Status: {(int)response.StatusCode} {response.StatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Data:");

                // Pretty print the JSON
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
                    var formattedJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(formattedJson);
                }
                catch
                {
                    // Fall back to raw content if JSON parsing fails
                    Console.WriteLine(content);
                }
            }
        }

        // Can be called from Program.cs for testing
        public static void RunTest()
        {
            Console.WriteLine("Enter your YNAB API Key:");
            var apiKey = Console.ReadLine();

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("API key is required.");
                return;
            }

            TestYnabApiConnection(apiKey).GetAwaiter().GetResult();
        }
    }
}
