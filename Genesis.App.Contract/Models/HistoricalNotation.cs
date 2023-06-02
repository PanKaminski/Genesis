using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class HistoricalNotation : ManagedEntity
{
    public string Content { get; set; }

    public Address Place { get; set; }

    public IList<Picture> Pictures { get; set; }

    public IList<Person> Members { get; set; }

    public IList<Document> Documents { get; set; }
}