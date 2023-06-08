using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IPersonsRepository
    {
        void Add(PersonDto person);

        IEnumerable<PersonDto> GetTreePersons(int treeId);

        IEnumerable<PersonDto> GetPersonsWithoutTree(int ownerId, bool trackEntities, List<PersonLoadOptions> loadOptions = null);

        IEnumerable<PersonDto> GetPersons(int ownerId, bool trackEntities, List<PersonLoadOptions> loadOptions = null);

        PersonDto GetByAccount(int accountId, List<PersonLoadOptions> loadOptions = null);

        PersonDto GetPerson(int id, List<PersonLoadOptions> loadOptions = null, bool trackPerson = false);

        void RemovePerson(int personId);
    }
}
