using AzureSearch.UI.Model;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace AzureSearch.UI.Util
{
    public class CosmosDbHelper
    {
        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The container we will create.
        private Microsoft.Azure.Cosmos.Container container;

        private string databaseId = "FilePoc";
        private string containerId = "FileMetadata";

        private IConfiguration _configuration;

        public CosmosDbHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<FileMetadata>> List()
        {
            Microsoft.Azure.Cosmos.Container container = await GetContainer();

            using FeedIterator<FileMetadata> feed = container.GetItemQueryIterator<FileMetadata>(queryText: "SELECT * FROM FileMetadata");
            List<FileMetadata> result = new List<FileMetadata>();

            while (feed.HasMoreResults)
            {
                FeedResponse<FileMetadata> response = await feed.ReadNextAsync();

                // Iterate query results
                foreach (FileMetadata item in response)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private async Task<Microsoft.Azure.Cosmos.Container> GetContainer()
        {
            var connectionString = _configuration["CosmosDbConnectionString"];

            cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    Indented = true,
                }
            });

            container = cosmosClient.GetContainer(databaseId, containerId);
            return container;
        }

        public async Task Add(FileMetadata fileMetadata)
        {
            var container = await GetContainer();
            await container.CreateItemAsync<FileMetadata>(item: fileMetadata);
        }

    }
}
