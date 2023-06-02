using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos;

public class BiographyDto : ManagedEntity
{
    public string Info { get; set; }

    public IList<HistoricalNotationDto> Events { get; set; }

    public DateTime? BirthDate { get; set; }
    
    public DateTime? DeathDate { get; set; }

    public int? BirthPlaceId { get; set; }
    public AddressDto BirthPlace { get; set; }

    public int? DeathPlaceId { get; set; }
    public AddressDto DeathPlace { get; set; }

    public int? PersonId { get; set; }
    public PersonDto Person { get; set; }
}