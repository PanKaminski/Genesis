using Newtonsoft.Json;

namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class TreeNodeResponse
    {
        public int Id { get; set; }

        [JsonProperty("mid")]
        public int? MotherId { get; set; }

        [JsonProperty("fid")]
        public int? FatherId { get; set; }

        [JsonProperty("pids")]
        public IEnumerable<int> SpouseIds { get; set; }

        [JsonProperty("childrenIds")]
        public IEnumerable<int> ChildrenIds { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        [JsonProperty("img")]
        public string Image { get; set; }
    }
}
