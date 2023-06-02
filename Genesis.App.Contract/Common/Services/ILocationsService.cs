using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Contract.Common.Services;

public interface ILocationsService
{
    Task<IEnumerable<SelectItem>> GetAllCountriesAsync();
    Task<IEnumerable<SelectItem>> GetCitiesAsync(string countryCode);
}