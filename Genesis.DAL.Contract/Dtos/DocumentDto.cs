using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos;

public class DocumentDto : ManagedEntity
{
    public string Path { get; set; }

    public string FileType { get; set; }

    public HistoricalNotationDto HistoricalNotation { get; set; }

    public IList<PersonDto> Persons { get; set; }
}