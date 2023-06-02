using AutoMapper;
using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.Common.Exceptions;
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
