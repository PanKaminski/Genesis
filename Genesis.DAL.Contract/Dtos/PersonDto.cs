using Genesis.Common;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos.Account;

namespace Genesis.DAL.Contract.Dtos;

public class PersonDto : ManagedEntity
{
    public PersonDto()
    {
    }

    public PersonDto(string firstName, string lastName, string middleName, int gender, bool isRoot)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Gender = (Gender)gender;
        Biography = new BiographyDto();
        HasLinkToAccount = isRoot;
    }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public Gender Gender { get; set; }

    public int AccountId { get; set; }
    public AccountDto Account { get; set; }

    public bool HasLinkToAccount { get; set; }

    public int? GenealogicalTreeId { get; set; }
    public GenealogicalTreeDto GenealogicalTree { get; set; }

    public IList<DocumentDto> RelatedDocuments { get; set; }

    public IList<PersonRelationDto> RelationsAsRoot { get; set; }
    public IList<PersonRelationDto> RelationsAsDependent { get; set; }

    public IList<PictureDto> Photos { get; set; }

    public BiographyDto Biography { get; set; }

    public string GetFullName() => $"{FirstName} {MiddleName ?? string.Empty} {LastName ?? string.Empty}".TrimEnd();
}