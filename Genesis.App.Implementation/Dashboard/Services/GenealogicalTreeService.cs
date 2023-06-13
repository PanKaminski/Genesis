using AutoMapper;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.Common.Exceptions;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;
using Genesis.DAL.Contract.LoadOptions.Account;
using Genesis.DAL.Contract.UOW;

namespace Genesis.App.Implementation.Dashboard.Services
{
    public class GenealogicalTreeService : IGenealogicalTreeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public GenealogicalTreeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<int> AddTreeAsync(GenealogicalTree tree, bool saveChanges)
        {
            var coatOfArms = tree.CoatOfArms is null ? null : new PictureDto
            {
                Url = tree.CoatOfArms.Url,
                PublicId = tree.CoatOfArms.PublicId,
                IsMain = tree.CoatOfArms.IsMain,
                CreatedTime = DateTime.Now,
            };

            var treeDto = new GenealogicalTreeDto
            {
                Name = tree.Name,
                OwnerId = tree.OwnerId,
                Description = tree.Description,
                CoatOfArms = coatOfArms,
                Modifiers = unitOfWork.AccountsRepository.Get(tree.Modifiers.Select(m => m.Id), true).ToList(),
                Persons = new List<PersonDto> 
                {
                    unitOfWork.PersonsRepository.GetPerson(tree.Persons.First().Id, trackPerson: true) 
                },
            };

            await unitOfWork.GenealogicalTreesRepository.AddAsync(treeDto);

            if (saveChanges) await unitOfWork.CommitAsync();

            return treeDto.Id;
        }

        public async Task UpdateTreeAsync(GenealogicalTree tree, bool saveChanges)
        {
            unitOfWork.PicturesRepository.DeleteByTreeId(tree.Id);

            var coatOfArms = tree.CoatOfArms is null ? null : new PictureDto
            {
                Url = tree.CoatOfArms.Url,
                PublicId = tree.CoatOfArms.PublicId,
                IsMain = tree.CoatOfArms.IsMain,
                CreatedTime = DateTime.Now,
            };

            var treeDto = new GenealogicalTreeDto
            {
                Id = tree.Id,
                Name = tree.Name,
                Description = tree.Description,
                CoatOfArms = coatOfArms,
                Modifiers = unitOfWork.AccountsRepository.Get(tree.Modifiers.Select(m => m.Id), true).ToList(),
            };

            unitOfWork.GenealogicalTreesRepository.Update(treeDto);

            if (saveChanges) await unitOfWork.CommitAsync();
        }

        public IEnumerable<GenealogicalTree> GetAllUserTrees(int userId)
        {
            var account = unitOfWork.AccountsRepository
                .GetById(userId, new List<AccountLoadOptions> 
                { 
                    AccountLoadOptions.WithAvailableTrees,
                    AccountLoadOptions.WithPersonalTrees
                });

            foreach (var ownedTree in MapTrees(account.PersonalTrees))
                yield return ownedTree;
            foreach (var availableTree in MapTrees(account.AvailableTrees))
                yield return availableTree;
        }

        private IEnumerable<GenealogicalTree> MapTrees(IEnumerable<GenealogicalTreeDto> dtos)
        {
            foreach (var dto in dtos)
            {
                var coatOfArms = dto.CoatOfArms is not null ? new Picture(dto.CoatOfArms.Id, dto.CoatOfArms.PublicId, dto.CoatOfArms.Url, true) : null;
                var persons = dto.Persons is not null ? dto.Persons.Select(p => new Person(p.FirstName, p.MiddleName, p.LastName, p.Gender, p.HasLinkToAccount)).ToList() : null;

                yield return new GenealogicalTree(
                    dto.Id,
                    dto.Name, 
                    dto.OwnerId, 
                    dto.Description, 
                    coatOfArms, dto.UpdatedTime, 
                    dto.CreatedTime, 
                    persons
                    );
            }
        }

        public GenealogicalTree GetTreeWithModifiers(int treeId) => 
            mapper.Map<GenealogicalTree>(unitOfWork.GenealogicalTreesRepository
                .GetTree(treeId, false, new List<TreeLoadOptions> { TreeLoadOptions.WithModifiers }));

        public GenealogicalTree GetTreeWithModifiersAndPersons(int treeId) =>
            mapper.Map<GenealogicalTree>(unitOfWork.GenealogicalTreesRepository
                .GetTree(treeId, false, new List<TreeLoadOptions> { TreeLoadOptions.WithModifiers, TreeLoadOptions.WithPersonData }));

        public IEnumerable<GenealogicalTree> GetUsersTreesWithModifiersAndPersons(int userId)
        {
            var trees = unitOfWork.GenealogicalTreesRepository
                .GetAvailableTrees(userId, new List<TreeLoadOptions> { TreeLoadOptions.WithModifiers, TreeLoadOptions.WithPersonData });

            return mapper.Map<IEnumerable<GenealogicalTree>>(trees);
        }

        public async Task<int> GetLastCreatedTreeIdAsync(int userId) =>
            await unitOfWork.GenealogicalTreesRepository.GetLastCreatedTreeIdAsync(userId);
        
        public async Task CreateDraftTreeAsync(GenealogicalTree tree)
        {
            if (!tree.Persons.Any()) 
                throw new GenesisApplicationException("Can't create draft tree without persons.");
            try
            {
                var owner = unitOfWork.AccountsRepository.GetById(tree.OwnerId);
                var person = unitOfWork.PersonsRepository.GetPerson(tree.Persons.First().Id, trackPerson: true);

                var treeDto = new GenealogicalTreeDto
                {
                    Owner = owner,
                    Persons = new List<PersonDto> { person },
                    Name = tree.Name,
                };

                await unitOfWork.GenealogicalTreesRepository.AddAsync(treeDto);
                await unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                unitOfWork.RevertChanges();
                throw;
            }
        }
    }
}
