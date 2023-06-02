using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class Document : ManagedEntity
{
    public string Path { get; set; }

    public string FileType { get; set; }
}