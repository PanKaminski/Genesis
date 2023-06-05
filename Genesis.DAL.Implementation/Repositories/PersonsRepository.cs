using Genesis.Common.Exceptions;
using Genesis.Common.Extensions;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class PersonsRepository : RepositoryBase<PersonDto>, IPersonsRepository
    {
        public PersonsRepository(GenesisDbContext dbContext) : base(dbContext)
        {
        }

        public void Add(PersonDto person) 
        {
            person.CreatedTime = DateTime.Now;

            if (person.Biography is not null) person.Biography.CreatedTime = DateTime.Now;

            DbContext.Add(person); 
        }

        public IEnumerable<PersonDto> GetTreePersons(int treeId)
        {
            return this.DbContext.Persons
                .Include(p => p.RelationsAsRoot)
                .Include(p => p.Photos)
                .AsNoTracking()
                .Where(p => p.GenealogicalTreeId == treeId);
        }

        public PersonDto GetPerson(int id, List<PersonLoadOptions> loadOptions = null, bool trackPerson = false)
        {
            var model = this.PrepareModel(loadOptions);

            if (!trackPerson)
            {
                model = model.AsNoTracking();
            }

            if (model.TryGetSingleValue(p => p.Id == id, out PersonDto person))
            {
                return person;
            }

            throw new GenesisDalException("Person is not found", nameof(id));
        }

        private IQueryable<PersonDto> PrepareModel(List<PersonLoadOptions> loadOptions)
        {
            IQueryable<PersonDto> model = DbContext.Persons;

            if (loadOptions is null) return model;

            if (loadOptions.Any(opt => opt == PersonLoadOptions.WithBiography || opt == PersonLoadOptions.Full))
            {
                model = model.Include(p => p.Biography).ThenInclude(b => b.BirthDate);
                model = model.Include(p => p.Biography).ThenInclude(b => b.BirthPlace);
            }

            if (loadOptions.Any(opt => opt == PersonLoadOptions.WithPictures || opt == PersonLoadOptions.Full))
            {
                model = model.Include(p => p.Photos);
            }

            if (loadOptions.Any(opt => opt == PersonLoadOptions.WithGenealogicalTree || opt == PersonLoadOptions.Full))
            {
                model = model.Include(p => p.GenealogicalTree);
            }

            return model;

        }

        public PersonDto GetByAccount(int accountId, List<PersonLoadOptions> loadOptions = null)
        {
            return PrepareModel(loadOptions).FirstOrDefault(p => p.AccountId == accountId && p.HasLinkToAccount);
        }

        public void RemovePerson(int personId)
        {
            if (!DbContext.Persons.TryGetSingleValue(p => p.Id == personId, out var person))
            {
                throw new GenesisApplicationException("Unable to find person with specified id");
            }

            DbContext.Biographies.Remove(DbContext.Biographies.First(b =>  b.PersonId == personId));

            DbContext.Persons.Remove(person);
        }
    }
}
