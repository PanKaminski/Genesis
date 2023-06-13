using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Models;

namespace Genesis.App.Contract.Dashboard.Services
{
    public interface IGenealogicalTreeService
    {
        IEnumerable<GenealogicalTree> GetAllUserTrees(int userId);

        IEnumerable<GenealogicalTree> GetUsersTreesWithModifiersAndPersons(int userId);

        GenealogicalTree GetTreeWithModifiers(int treeId);

        GenealogicalTree GetTreeWithModifiersAndPersons(int treeId);

        Task<int> AddTreeAsync(GenealogicalTree tree, bool SaveChanges);

        Task UpdateTreeAsync(GenealogicalTree tree, bool SaveChanges);

        Task<int> GetLastCreatedTreeIdAsync(int userId);

        Task CreateDraftTreeAsync(GenealogicalTree tree);
    }
}
