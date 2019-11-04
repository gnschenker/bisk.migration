using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Bisk.Messages;

namespace Bisk.Migration
{
    public class DisciplineSelectedHandler : IMessageHandler<string, DisciplineSelected>, IDisposable
    {
        private string BASE_URI = "http://localhost:5555";
        private HttpClient client = new HttpClient();

        public DisciplineSelectedHandler()
        {
            BASE_URI = Environment.GetEnvironmentVariable("BOAS_API_URL") ?? BASE_URI;
            client = new HttpClient();
            client.BaseAddress = new Uri(BASE_URI);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public async Task Handles(string key, DisciplineSelected message)
        {
            Console.WriteLine($"Got message: {message}");

            var payload = new {
                ApplicationId = message.ApplicationId,
                DsciplineId = message.DisciplineId
            };
            var body = JsonSerializer.Serialize(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            Console.WriteLine($"Payload is {body}");
            var response = await client.PostAsync("/api/disciplines", content);
        }
    }
}