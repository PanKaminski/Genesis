using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class Picture : AuditableEntity
{
    public string Url { get; set; }

    public string PublicId { get; set; }

    public bool IsMain { get; set; }

    public Picture()
    {
    }

    public Picture(int id, string url, string publicId, bool isMain)
    {
        Id = id;
        Url = url;
        PublicId = publicId;
        IsMain = isMain;
    }

    public Picture(string url, string publicId, bool isMain = false) : this(default, url, publicId, isMain)
    {
        CreatedTime = DateTime.Now;
    }
}