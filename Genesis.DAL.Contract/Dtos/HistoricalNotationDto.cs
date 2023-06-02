using Genesis.Common;

namespace Genesis.DAL.Contract.Dtos;

public class HistoricalNotationDto : ManagedEntity
{
    public string Content { get; set; }

    public AddressDto Place { get; set; }

    public IList<PictureDto> Pictures { get; set; }

    public IList<BiographyDto> Biographies { get; set; }

    public IList<DocumentDto> Documents { get; set; }
}