using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Azure.Search.Documents;
using Azure;
using AzureSearch.UI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AzureSearch.UI.Util;

namespace AzureSearch.UI.Pages
{
    public class FileMetadataModel : PageModel
    {
        private readonly ILogger<FileMetadataModel> _logger;
        private readonly IConfiguration _configuration;
        private CosmosDbHelper _cosmosDbHelper;

        public FileMetadata FileMetadataDetails { get; set; }

        public bool ReadOnly { get; set; } = false;

        public FileMetadataModel(ILogger<FileMetadataModel> logger, IConfiguration configuration, CosmosDbHelper cosmosDbHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _cosmosDbHelper = cosmosDbHelper;
        }

        public async Task OnGet(string fileName, string view)
        {
            var searchClient = CreateSearchIndexClient();
            FileMetadataDetails = await RunQueries(searchClient, fileName);
            ReadOnly = string.IsNullOrEmpty(view) ? false : true;
        }

        public async Task<IActionResult> OnPost(string fileName)
        {
            var searchClient = CreateSearchIndexClient();
            FileMetadataDetails = await RunQueries(searchClient, fileName);

            await _cosmosDbHelper.Add(FileMetadataDetails);
            return Redirect("/");
        }

        private SearchClient CreateSearchIndexClient()
        {
            string searchServiceEndPoint = _configuration["SearchServiceEndPoint"];
            string adminApiKey = _configuration["SearchServiceAdminApiKey"];

            SearchIndexClient indexClient = new SearchIndexClient(new Uri(searchServiceEndPoint), new AzureKeyCredential(adminApiKey));

            string indexName = _configuration["SearchIndexName"];
            SearchClient searchClient = indexClient.GetSearchClient(indexName);

            return searchClient;
        }

        private async Task<FileMetadata> RunQueries(SearchClient searchClient, string fileName)
        {
            int attemptsCount = 0;
            int limit = 10;

            FileMetadata fileMetadata = null;

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            while (attemptsCount < limit)
            {
                attemptsCount++;

                SearchOptions options;
                SearchResults<FileMetadata> results;

                options = new SearchOptions();

                results = searchClient.Search<FileMetadata>(fileNameWithoutExtension, options);
                var result = results.GetResults().FirstOrDefault();

                if (result != null && result.Document != null)
                {
                    fileMetadata = result.Document;
                    break;
                }

                await Task.Delay(5000);
                if (attemptsCount % 3 == 0 && fileName.Contains(".jpg"))
                {
                    fileNameWithoutExtension = fileName;
                }
            }

            return fileMetadata;
        }
    }
}
