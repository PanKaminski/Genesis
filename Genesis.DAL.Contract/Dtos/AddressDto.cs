namespace Genesis.DAL.Contract.Dtos;

public class AddressDto
{
    public int Id { get; set; }
    
    public string CountryCode { get; set; }
    
    public string Region { get; set; }
    
    public string SettlementCode { get; set; }
    
    public IList<HistoricalNotationDto> Notations { get; set; }

    public IList<BiographyDto> BirthdayAttachedBiographies { get; set; }

    public IList<BiographyDto> DayOfDeathAttachedBiographies { get; set; }
}