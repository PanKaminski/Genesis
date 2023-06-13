namespace Genesis.App.Contract.Models;

public class Address
{
    public int Id { get; set; }
    
    public string Country { get; set; }
    
    public string Region { get; set; }
    
    public string Settlement { get; set; }
    
    public string Street { get; set; }

    public IList<HistoricalNotation> Notations { get; set; }

    public IList<Biography> Biographies { get; set; }

    public string GetFullAddress() => 
        $"{Country ?? string.Empty}, {Settlement ?? string.Empty}, {Street ?? string.Empty}"
        .Trim(new char[] { ' ', ','});
}