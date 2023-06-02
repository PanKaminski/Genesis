using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class Biography : ManagedEntity
{
    public Person Person { get; set; }

    public string Info { get; set; }

    public IList<HistoricalNotation> Events { get; set; }

    public DateTime? BirthDate { get; set; }
    
    public DateTime? DeathDate { get; set; }

    public Address BirthPlace { get; set; }

    public Address DeathPlace { get; set; }
}