using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KoboldSharp
{
    public class KoboldClient
    {
        private readonly HttpClient _client;
        private readonly string _baseUri;

        public KoboldClient(string baseUri)
        {
            _client = new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(5)
            };
            _baseUri = baseUri;
        }

        public KoboldClient(string baseUri, HttpClient client)
        {
            _client = client;
            _baseUri = baseUri;
        }

        public async Task<ModelOutput> Generate(GenParams parameters)
        {
            var payload = new StringContent(parameters.GetJson(), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{_baseUri}/api/v1/generate", payload);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            var output = JsonSerializer.Deserialize<ModelOutput>(content);

            // If TrimStop is true, trim the output at the first occurrence of any stop sequence
            if (parameters.TrimStop && parameters.StopSequence != null && parameters.StopSequence.Count > 0)
            {
                foreach (var result in output.Results)
                {
                    foreach (var stopSeq in parameters.StopSequence)
                    {
                        int index = result.Text.IndexOf(stopSeq);
                        if (index >= 0)
                        {
                            result.Text = result.Text.Substring(0, index);
                        }
                    }
                }
            }

            return output;
        }


        public async Task<ModelOutput> Check()
        {
            var payload = new StringContent(string.Empty);
            var response = await _client.PostAsync($"{_baseUri}/api/extra/generate/check", payload);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            return JsonSerializer.Deserialize<ModelOutput>(content);

        }

        public async void Abort()
        {
            var payload = new StringContent(string.Empty);
            var response = await _client.PostAsync($"{_baseUri}/api/v1/abort", payload);
            await response.Content.ReadAsStringAsync();
        }
    }
}

