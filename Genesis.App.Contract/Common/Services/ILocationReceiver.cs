using Genesis.App.Contract.Models;

namespace Genesis.App.Contract.Common.Services;

public interface ILocationReceiver
{
    Task<IEnumerable<Country>> GetCountriesAsync();
    Task<IEnumerable<City>> GetCitiesAsync(string countryCode);
    Task<City> GetCityDetailsAsync(int cityId);
}