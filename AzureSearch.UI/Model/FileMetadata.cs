using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AzureSearch.UI.Model
{
    [Serializable]
    public class FileMetadata
    {
        [JsonPropertyName("id")]
        public string id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("metadata_storage_content_type")]
        public string? ContentType { get; set; }

        [JsonPropertyName("metadata_storage_last_modified")]
        public string? LastModified { get; set; }

        [JsonPropertyName("metadata_storage_size")]
        public int Size { get; set; }

        [JsonPropertyName("metadata_storage_name")]
        public string? Name { get; set; }

        [JsonPropertyName("metadata_creation_date")]
        public DateTime? CreationDate { get; set; }

        [JsonPropertyName("metadata_storage_path")]
        public string? Path { get; set; }

        [JsonPropertyName("text")]
        public List<string>? Text { get; set; }

        [JsonPropertyName("layoutText")]
        public List<string>? LayoutText { get; set; }

        [JsonPropertyName("metadata_title")]
        public string? FileName { get; set; }

        [JsonPropertyName("merged_content")]
        public string? Content { get; set; }

        [JsonPropertyName("people")]
        public List<string>? People { get; set;}

        [JsonPropertyName("organizations")]
        public List<string>? Organizations { get; set; }

        [JsonPropertyName("locations")]
        public List<string>? Locations { get; set; }

        [JsonPropertyName("keyphrases")]
        public List<string>? Keyphrases { get; set; }

        [JsonPropertyName("pii_entities")]
        public List<PiiEntities>? PiiEntities { get; set; }

        [JsonPropertyName("imageCaption")]
        public List<string>? ImageCaptions { get; set; }

        [JsonPropertyName("imageTags")]
        public List<string>? ImageTags { get; set; }
    }

    [Serializable]
    public class PiiEntities
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("subtype")]
        public string SubType { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get;set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set;  }
    }

}
