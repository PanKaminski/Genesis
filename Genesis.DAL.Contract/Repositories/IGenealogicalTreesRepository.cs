using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.LoadOptions;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IGenealogicalTreesRepository
    {
        ICollection<GenealogicalTreeDto> GetAvailableTrees(int userId, IList<TreeLoadOptions> loadOptions = null);

        GenealogicalTreeDto GetTree(int treeId, bool trackEntity, IList<TreeLoadOptions> loadOptions = null);

        void CreateInitialTree(PersonDto root);

        Task AddAsync(GenealogicalTreeDto tree);

        void Update(GenealogicalTreeDto tree);
    }
}
