using System.Text.Json.Serialization;

namespace UrlScraper.Api.Models
{
    public class StandardResponse<T>
    {
        public bool Successful { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorResponse Error{ get; set; }     
        public T Payload { get; set; }
    }
}