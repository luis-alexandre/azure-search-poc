using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureSearch.UI.Model;
using AzureSearch.UI.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureSearch.UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private CosmosDbHelper _cosmosDbHelper;

        public List<FileMetadata> FileMetadataCollection { get; set; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, CosmosDbHelper cosmosDbHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _cosmosDbHelper = cosmosDbHelper;
        }

        public async Task OnGet()
        {
            FileMetadataCollection = await _cosmosDbHelper.List();
        }

        public async Task<IActionResult> OnPost()
        {
            var fileName = await UploadDocument("pdf-documents");
            await RerunIndexer();

            return Redirect($"/FileMetadata?fileName={fileName}");
        }

        public async Task<string> UploadDocument(string container)
        {
            var fileName = Upload.FileName;

            var blobConnectionString = _configuration["BlobConnString"];
            var blobClient = new BlobClient(blobConnectionString, container, fileName);

            using (var stream = new MemoryStream())
            {
                Upload.CopyTo(stream);

                stream.Position = 0;
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return fileName;
        }

        private async Task RerunIndexer()
        {
            try
            {
                string searchServiceEndPoint = _configuration["SearchServiceEndPoint"];
                string adminApiKey = _configuration["SearchServiceAdminApiKey"];
                string indexerName = _configuration["IndexerName"];

                SearchIndexerClient searchIndexerClient = new SearchIndexerClient(new Uri(searchServiceEndPoint), new AzureKeyCredential(adminApiKey));
                var response = searchIndexerClient.RunIndexer(indexerName);
                var status = searchIndexerClient.GetIndexerStatus(indexerName);

                _logger.LogInformation("Updating index.");
                await Task.Delay(3000);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}