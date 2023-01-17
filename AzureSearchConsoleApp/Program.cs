// See https://aka.ms/new-console-template for more information
using Azure.Search.Documents.Indexes;
using Azure;
using Microsoft.Extensions.Configuration;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using AzureSearchConsoleApp;
using System;

internal class Program
{
    private static void Main(string[] args)
    {
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        IConfigurationRoot configuration = builder.Build();

        RerunIndexer(configuration);
        SearchIndexClient indexClient = CreateSearchIndexClient(configuration);

        string indexName = configuration["SearchIndexName"];
        SearchClient searchClient = indexClient.GetSearchClient(indexName);

        SearchClient indexClientForQueries = CreateSearchClientForQueries(indexName, configuration);

        RunQueries(indexClientForQueries);
    }

    private static void RerunIndexer(IConfigurationRoot configuration)
    {
        string searchServiceEndPoint = configuration["SearchServiceEndPoint"];
        string adminApiKey = configuration["SearchServiceAdminApiKey"];
        string indexerName = configuration["IndexerName"];

        SearchIndexerClient searchIndexerClient = new SearchIndexerClient(new Uri(searchServiceEndPoint), new AzureKeyCredential(adminApiKey));
        var response = searchIndexerClient.RunIndexer(indexerName);

        var status = searchIndexerClient.GetIndexerStatus(indexerName);

        Console.WriteLine(response.Status);
        Console.WriteLine(status);
    }

    private static SearchIndexClient CreateSearchIndexClient(IConfigurationRoot configuration)
    {
        string searchServiceEndPoint = configuration["SearchServiceEndPoint"];
        string adminApiKey = configuration["SearchServiceAdminApiKey"];

        SearchIndexClient indexClient = new SearchIndexClient(new Uri(searchServiceEndPoint), new AzureKeyCredential(adminApiKey));
        return indexClient;
    }

    private static SearchClient CreateSearchClientForQueries(string indexName, IConfigurationRoot configuration)
    {
        string searchServiceEndPoint = configuration["SearchServiceEndPoint"];
        string queryApiKey = configuration["SearchServiceQueryApiKey"];

        SearchClient searchClient = new SearchClient(new Uri(searchServiceEndPoint), indexName, new AzureKeyCredential(queryApiKey));
        return searchClient;
    }

    private static void RunQueries(SearchClient searchClient)
    {
        SearchOptions options;
        SearchResults<FileStorageResults> results;

        options = new SearchOptions();
        //options.Select.Contains("THIS PLAN LIES WITHIN THE GREATER VANCOUVER REGIONAL DISTRICT");

        results = searchClient.Search<FileStorageResults>("\"POSTING PLAN OF LOT 376\"", options);

        WriteDocuments(results);
    }

    private static void WriteDocuments(SearchResults<FileStorageResults> searchResults)
    {
        foreach (SearchResult<FileStorageResults> result in searchResults.GetResults())
        {
            Console.WriteLine(result.Document.ContentType);
            Console.WriteLine(result.Document.Size);
            Console.WriteLine(result.Document.Name);
            Console.WriteLine(result.Document.CreationDate);
            Console.WriteLine(string.Join("-", result.Document.Text));
            Console.WriteLine(string.Join("-", result.Document.LayoutText));
            Console.WriteLine(result.Document.FileName);
            Console.WriteLine(result.Document.Content);
        }

        Console.WriteLine();
    }
}