using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models.Forms;
using Microsoft.AspNetCore.Mvc;

namespace Genesis.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsService locationsService;

        public LocationsController(ILocationsService locationsService)
        {
            this.locationsService = locationsService ?? throw new ArgumentNullException(nameof(locationsService));
        }

        [HttpGet]
        public async Task<IEnumerable<SelectItem>> GetCountriesListAsync() => await locationsService.GetAllCountriesAsync();

        [HttpGet]
        public async Task<IEnumerable<SelectItem>> GetCitiesListAsync(string countryCode) =>
            await locationsService.GetCitiesAsync(countryCode);
    }
}
