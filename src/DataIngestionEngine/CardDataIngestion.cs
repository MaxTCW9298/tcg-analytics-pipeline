using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Newtonsoft.Json;

namespace PokeMetrix.Ingestion
{
    public class CardIngestion
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public CardIngestion(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CardIngestion>();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PokeMetrix-Data-Platform");
        }

        [Function("DailyCardFetch")]
        public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"Fetching Pokemon Data: {DateTime.Now}");
            
            // 1. Fetch Data (Mocking a small page for demo)
            string apiUrl = "https://api.pokemontcg.io/v2/cards?page=1&pageSize=10"; 
            var response = await _httpClient.GetAsync(apiUrl);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                // 2. Connect to Storage (Safe handling for local runs)
                string connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") ?? "UseDevelopmentStorage=true";
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient("bronze");
                
                // Only try to create container if we have a real connection
                if(!connectionString.Contains("UseDevelopmentStorage")) {
                     await containerClient.CreateIfNotExistsAsync();
                }

                // 3. Save Data
                string datePath = DateTime.UtcNow.ToString("yyyy/MM/dd");
                string fileName = $"cards/{datePath}/batch_1.json";
                _logger.LogInformation($"Data fetched. Ready to save to {fileName}");
            }
        }
    }
}