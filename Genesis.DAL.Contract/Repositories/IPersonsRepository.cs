using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IPersonsRepository
    {
        void Add(PersonDto person);

        IEnumerable<PersonDto> GetTreePersons(int treeId);

        PersonDto GetByAccount(int accountId, List<PersonLoadOptions> loadOptions = null);

        PersonDto GetPerson(int id, List<PersonLoadOptions> loadOptions = null, bool trackPerson = false);

        void RemovePerson(int personId);
    }
}
