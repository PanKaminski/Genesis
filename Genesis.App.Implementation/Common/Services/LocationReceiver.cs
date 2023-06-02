using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models;
using Newtonsoft.Json.Linq;

namespace Genesis.App.Implementation.Authentication.Services;

public class LocationReceiver : ILocationReceiver
{
    private const string Api = "https://wft-geo-db.p.rapidapi.com/v1/geo";
    
    private readonly HttpClient httpClient;

    public LocationReceiver(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<Country>> GetCountriesAsync()
    {
        var jsonObject = await GetGeoData("/countries?offset=10");
        var countries = jsonObject is { } ?
            jsonObject.Children().Select(c => c.ToObject<Country>()) : new List<Country>();

        return countries;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync(string countryCode)
    {
        var jsonObject = await GetGeoData($"/cities?countryIds={countryCode}");
        var cities = jsonObject is { } ?
            jsonObject.Children().Select(c => c.ToObject<City>()) : new List<City>();

        return cities;
    }

    public async Task<City> GetCityDetailsAsync(int cityId) => (await GetGeoData($"/cities/{cityId}"))?.ToObject<City>();

    private async Task<JToken> GetGeoData(string path)
    {
        var request = CreateRequestMessage(path);
        
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        return JObject.Parse(body)["data"];
    }

    private HttpRequestMessage CreateRequestMessage(string path) => new()
    {
        Method = HttpMethod.Get,
        RequestUri = new Uri(Api + path),
        
        Headers =
        {
            { "X-RapidAPI-Key", "2784faac60msh442834adae43258p110a3bjsndb73bec1ddbd" },
            { "X-RapidAPI-Host", "wft-geo-db.p.rapidapi.com" },
        },
    };
}