using System.Text.Json.Serialization;

namespace AzureSearchConsoleApp
{
    public class FileStorageResults
    {
        [JsonPropertyName("metadata_storage_content_type")]
        public string ContentType { get; set; }

        [JsonPropertyName("metadata_storage_size")]
        public int Size { get; set; }

        [JsonPropertyName("metadata_storage_name")]
        public string Name { get; set; }

        [JsonPropertyName("metadata_creation_date")]
        public DateTime CreationDate { get; set; }

        [JsonPropertyName("text")]
        public List<string> Text { get; set; }

        [JsonPropertyName("layoutText")]
        public List<string> LayoutText { get; set; }

        [JsonPropertyName("metadata_title")]
        public string FileName { get; set; }

        [JsonPropertyName("merged_content")]
        public string Content { get; set; }
    }
}
