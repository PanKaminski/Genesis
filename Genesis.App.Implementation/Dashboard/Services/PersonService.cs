using AutoMapper;
using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Implementation.Dashboard.Helpers;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;
using Genesis.DAL.Contract.UOW;

namespace Genesis.App.Implementation.Dashboard.Services
{
    public class PersonService : IPersonService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public PersonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<Person> GetPersonsWithoutTree(int ownerId)
        {
            if (ownerId < 1) throw new GenesisApplicationException("Invalid user id");

            return mapper.Map<IEnumerable<Person>>(unitOfWork.PersonsRepository.GetPersonsWithoutTree(ownerId, false));
        }

        public async IAsyncEnumerable<TreeNodeResponse> GetTreePersonsAsync(int treeId, string currentUserId)
        {
            if (treeId <= 0) throw new ArgumentException($"Tree id = {treeId}", nameof(treeId));
            if (string.IsNullOrEmpty(currentUserId)) throw new ArgumentException("Invalid user identifier", nameof(currentUserId));

            var relations = unitOfWork.RelationsRepository.GetRelations(treeId);
            var persons = unitOfWork.PersonsRepository.GetTreePersons(treeId).ToList();

            foreach (var person in persons)
            {
                var spouseIds = relations.Where(r => r.RelationType == Relation.Partners
                        && (r.FromPersonId == person.Id || r.ToPersonId == person.Id))
                    .Select(r => r.FromPersonId == person.Id ? r.ToPersonId : r.FromPersonId);
                var childrenIds = relations.Where(r => r.RelationType == Relation.ChildToParent && r.ToPersonId == person.Id)
                    .Select(r => r.FromPersonId);

                yield return new TreeNodeResponse
                {
                    Id = person.Id,
                    FatherId = RelationsResolver.GetParent(person.Id, relations, persons, Gender.Man),
                    MotherId = RelationsResolver.GetParent(person.Id, relations, persons, Gender.Woman),
                    Name = $"{person.FirstName} {person.LastName}",
                    Gender = person.Gender.GetClientView(),
                    SpouseIds = spouseIds,
                    ChildrenIds = childrenIds,
                    Image = person.Photos.FirstOrDefault(ph => ph.IsMain)?.Url,
                };
            }
        }

        public Person GetPerson(int id)
        {
            var personDto = this.unitOfWork.PersonsRepository.GetPerson(id);

            return mapper.Map<Person>(personDto);
        }

        public Person GetPersonWithGenealogicalTree(int id)
        {
            var personDto = this.unitOfWork.PersonsRepository.GetPerson(id, new List<PersonLoadOptions> 
            {
                PersonLoadOptions.WithGenealogicalTree 
            });

            return mapper.Map<Person>(personDto);
        }

        public Person GetPersonWithFullInfo(int id)
        {
            var personDto = this.unitOfWork.PersonsRepository.GetPerson(id, new List<PersonLoadOptions> 
            { 
                PersonLoadOptions.Full,
            });

            return mapper.Map<Person>(personDto);
        }

        public async Task<int> AddPersonAsync(Person person, int treeId, bool saveChanges = false)
        {
            var dto = mapper.Map<PersonDto>(person);
            dto.GenealogicalTreeId = treeId;
            this.unitOfWork.PersonsRepository.Add(dto);

            if (saveChanges) await unitOfWork.CommitAsync();

            return dto.Id;
        }

        public async Task EditPersonAsync(Person person, bool saveChanges = false)
        {
            var personDto = this.unitOfWork.PersonsRepository.GetPerson(person.Id, new List<PersonLoadOptions>
            {
                PersonLoadOptions.WithBiography 
            }, true);

            personDto.FirstName = person.FirstName;
            personDto.LastName = person.LastName;
            personDto.MiddleName = person.MiddleName;
            personDto.Biography.BirthDate = person.Biography.BirthDate;
            personDto.Biography.DeathDate = person.Biography.DeathDate;
            personDto.Biography.Info = person.Biography.Info;

            if (saveChanges) await unitOfWork.CommitAsync();
        }

        public Person GetRootPerson(int accountId)
        {
            return mapper.Map<Person>(unitOfWork.PersonsRepository.GetByAccount(accountId));
        }

        public void RemovePerson(int personId, bool saveChanges)
        {
            try
            {
                unitOfWork.PersonsRepository.RemovePerson(personId);

                if (saveChanges) unitOfWork.Commit();
            }
            catch (Exception)
            {
                unitOfWork.RevertChanges();
                throw;
            }
        }
    }
}
