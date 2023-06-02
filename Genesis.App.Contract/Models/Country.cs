using Newtonsoft.Json;

namespace Genesis.App.Contract.Models;

public class Country
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}