using Genesis.App.Contract.Models.Authentication;
using Genesis.Common;

namespace Genesis.App.Contract.Models;

public class GenealogicalTree : ManagedEntity
{
    public GenealogicalTree() 
    {
        Persons = new List<Person>();
        Modifiers = new List<Account>();
    }

    public GenealogicalTree(int id, string name, int ownerId, string description, 
        Picture coatOfArms, DateTime? lastUpdate, DateTime created,  IList<Person> persons = null, IList<Account> modifiers = null)
    {
        Id = id;
        Name = name;
        OwnerId = ownerId;
        Description = description;
        CoatOfArms = coatOfArms;
        Persons = persons;
        Modifiers = modifiers;
        UpdatedTime = lastUpdate;
        CreatedTime = created;
    }

    public string Name { get; set; }
    public int OwnerId { get; set; }
    public Account Owner { get; set; }
    public IList<Person> Persons { get; set; }
    public IList<Account> Modifiers { get; set; }
    public string Description { get; set; }
    public Picture CoatOfArms { get; set; }
}