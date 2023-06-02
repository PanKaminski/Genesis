using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Implementation.Common.Services
{
    public class LocationsService : ILocationsService
    {
        private readonly ILocationReceiver locationReceiver;

        public LocationsService(ILocationReceiver locationReceiver)
        {
            this.locationReceiver = locationReceiver ?? throw new ArgumentNullException(nameof(locationReceiver));
        }

        public async Task<IEnumerable<SelectItem>> GetAllCountriesAsync()
        {
            var countries = await locationReceiver.GetCountriesAsync();

            return countries.Select(country => new SelectItem(country.Name, country.Code));
        }

        public async Task<IEnumerable<SelectItem>> GetCitiesAsync(string countryCode)
        {
            ArgumentNullException.ThrowIfNull(countryCode);

            var cities = await locationReceiver.GetCitiesAsync(countryCode);

            return cities.Select(city => new SelectItem(city.Name, city.Id.ToString()));
        }
    }
}
