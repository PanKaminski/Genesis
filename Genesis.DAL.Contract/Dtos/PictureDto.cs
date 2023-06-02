using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos;

public class PictureDto : AuditableEntity
{
    public string Url { get; set; }

    public string PublicId { get; set; }

    public bool IsMain { get; set; }

    public int? HistoricalNotationId { get; set; }
    public HistoricalNotationDto HistoricalNotation { get; set; }

    public int? GenealogicalTreeId { get; set; }
    public GenealogicalTreeDto GenealogicalTree { get; set; }

    public int? PersonId { get; set; }
    public PersonDto Person { get; set; }
}