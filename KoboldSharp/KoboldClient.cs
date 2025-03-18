using System.Text;
using System.Text.Json;

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
            StringContent payload = new StringContent(parameters.GetJson(), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync($"{_baseUri}/api/v1/generate", payload);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            ModelOutput? output = JsonSerializer.Deserialize<ModelOutput>(content);

            // If TrimStop is true, trim the output at the first occurrence of any stop sequence
            if (parameters.TrimStop && parameters.StopSequence != null && parameters.StopSequence.Count > 0)
            {
                foreach (Result result in output.Results)
                {
                    foreach (string stopSeq in parameters.StopSequence)
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
            StringContent payload = new StringContent(string.Empty);
            HttpResponseMessage response = await _client.PostAsync($"{_baseUri}/api/extra/generate/check", payload);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            content = content.Trim();
            return JsonSerializer.Deserialize<ModelOutput>(content);

        }

        public async void Abort()
        {
            StringContent payload = new StringContent(string.Empty);
            HttpResponseMessage response = await _client.PostAsync($"{_baseUri}/api/extra/abort", payload);
            await response.Content.ReadAsStringAsync();
        }

        public async Task<PerformanceData> GetPerformanceAsync()
        {
            HttpResponseMessage response = await _client.GetAsync($"{_baseUri}/api/extra/perf");
            string data = await response.Content.ReadAsStringAsync();
            PerformanceData? perfData = JsonSerializer.Deserialize<PerformanceData>(data);
            if (perfData == null)
            {
                return new PerformanceData();
            }
            return perfData;
        }
    }
}

