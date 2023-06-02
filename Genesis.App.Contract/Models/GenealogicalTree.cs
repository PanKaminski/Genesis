using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class GenealogicalTree : ManagedEntity
{
    public string Name { get; set; }
    public int OwnerId { get; set; }
    public Account Owner { get; set; }
    public IList<Person> Persons { get; set; }
    public IList<Account> Modifiers { get; set; }
    public string Description { get; set; }
    public Picture CoatOfArms { get; set; }
}