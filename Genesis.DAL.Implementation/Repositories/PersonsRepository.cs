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

        public PersonDto GetPerson(int id, PersonLoadOptions? loadOptions = null, bool trackPerson = false)
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

        private IQueryable<PersonDto> PrepareModel(PersonLoadOptions? loadOptions) =>
            loadOptions switch
            {
                PersonLoadOptions.WithBiography => DbContext.Persons
                    .Include(t => t.Biography),
                PersonLoadOptions.WithPictures => DbContext.Persons
                    .Include(t => t.Photos),
                PersonLoadOptions.WithGenealogicalTree => DbContext.Persons
                    .Include(t => t.GenealogicalTree),
                PersonLoadOptions.Full => DbContext.Persons
                    .Include(p => p.Biography).ThenInclude(b => b.BirthPlace)
                    .Include(p => p.Biography).ThenInclude(b => b.DeathPlace)
                    .Include(p => p.GenealogicalTree)
                    .Include(p => p.Photos),
                _ => DbContext.Persons,
            };

        public PersonDto GetByAccount(int accountId)
        {
            return DbContext.Persons.FirstOrDefault(p => p.AccountId == accountId);
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
