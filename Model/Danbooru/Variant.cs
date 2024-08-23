using System.Text.Json.Serialization;

namespace GrailBot.Model.Danbooru;

public class Variant
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("width")]
    public int? Width { get; set; }

    [JsonPropertyName("height")]
    public int? Height { get; set; }

    [JsonPropertyName("file_ext")]
    public string FileExt { get; set; }
}

