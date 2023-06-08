using AutoMapper;
using Genesis.App.Contract.Dashboard.ApiModels;
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
                Name = tree.Name,
                Description = tree.Description,
                CoatOfArms = coatOfArms,
                Modifiers = unitOfWork.AccountsRepository.Get(tree.Modifiers.Select(m => m.Id), true).ToList(),
            };

            unitOfWork.GenealogicalTreesRepository.Update(treeDto);

            if (saveChanges) await unitOfWork.CommitAsync();
        }

        public TreesListResponse GetAllUserTreesTrees(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User identifier is null or empty.");

            if (!int.TryParse(userId, out var idNumber))
                throw new GenesisApplicationException("User id is not number");
            
            var account = unitOfWork.AccountsRepository
                .GetById(idNumber, new List<AccountLoadOptions> 
                { 
                    AccountLoadOptions.WithAvailableTrees,
                    AccountLoadOptions.WithPersonalTrees
                });

            var treesList = new TreesListResponse();

            foreach (var tree in account.PersonalTrees)
            {
                treesList.Add(
                    tree.Id,
                    tree.Persons.Count,
                    tree.Name,
                    tree.UpdatedTime ?? tree.CreatedTime,
                    true
                );
            }

            foreach (var tree in account.AvailableTrees)
            {
                treesList.Add(
                    tree.Id, 
                    tree.Persons.Count, 
                    tree.Name, 
                    tree.UpdatedTime ?? tree.CreatedTime
                );
            }

            return treesList;
        }

        public GenealogicalTree GetTreeWithModifiers(int treeId) => 
            mapper.Map<GenealogicalTree>(unitOfWork.GenealogicalTreesRepository
                .GetTree(treeId, false, new List<TreeLoadOptions> { TreeLoadOptions.WithModifiers }));

        public IEnumerable<GenealogicalTree> GetUsersTreesWithModifiersAndPersons(int userId)
        {
            var trees = unitOfWork.GenealogicalTreesRepository
                .GetAvailableTrees(userId, new List<TreeLoadOptions> { TreeLoadOptions.WithModifiers, TreeLoadOptions.WithPersonData });

            return mapper.Map<IEnumerable<GenealogicalTree>>(trees);
        }
    }
}
