using Genesis.Common;
using Genesis.DAL.Contract.Dtos.Account;

namespace Genesis.DAL.Contract.Dtos;

public class GenealogicalTreeDto : ManagedEntity
{
    public string Name { get; set; }
    public int OwnerId { get; set; }
    public AccountDto Owner { get; set; }
    public IList<PersonDto> Persons { get; set; }
    public IList<PersonRelationDto> Relations { get; set; }
    public IList<AccountDto> Modifiers { get; set; }
    public string Description { get; set; }
    public PictureDto CoatOfArms { get; set; }
}