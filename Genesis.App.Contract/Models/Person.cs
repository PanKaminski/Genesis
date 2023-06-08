using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Models;

public class Person : ManagedEntity
{
    public Person()
    {
        Biography = new Biography();
    }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }

    public int AccountId { get; set; }
    public Account Account { get; set; }

    public bool HasLinkToAccount { get; set; }

    public GenealogicalTree GenealogicalTree { get; set; }

    public IList<Document> RelatedDocuments { get; set; }

    public IList<Picture> Photos { get; set; }

    public Biography Biography { get; set; }

    public string FullName => $"{FirstName} {MiddleName ?? string.Empty} {LastName ?? string.Empty}".TrimEnd();

    public string GetTreeNodeName() => $"{FirstName} {LastName ?? string.Empty}".TrimEnd();
}