using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Models;

namespace Genesis.App.Contract.Dashboard.Services
{
    public interface IGenealogicalTreeService
    {
        TreesListResponse GetAllUserTreesTrees(string userId);

        IEnumerable<GenealogicalTree> GetUsersTreesWithModifiersAndPersons(int userId);

        GenealogicalTree GetTreeWithModifiers(int treeId);
    }
}
