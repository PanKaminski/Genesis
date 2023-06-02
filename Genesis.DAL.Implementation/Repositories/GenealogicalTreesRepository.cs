using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Dtos.Account;
using Genesis.DAL.Contract.LoadOptions;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class GenealogicalTreesRepository : RepositoryBase<GenealogicalTreeDto>, IGenealogicalTreesRepository
    {
        public GenealogicalTreesRepository(GenesisDbContext dbContext) : base(dbContext)
        {
        }

        public ICollection<GenealogicalTreeDto> GetAvailableTrees(int userId, IList<TreeLoadOptions> loadOptions = null) =>  
            PrepareModel(loadOptions)
                .Where(t => t.Modifiers.Any(m => m.Id == userId)).ToList();

        public void CreateInitialTree(PersonDto root)
        {
            var tree = new GenealogicalTreeDto
            {
                Name = "Main",
                Persons = new List<PersonDto> { root },
                Owner =  root.Account,
                CreatedTime = DateTime.Now,
            };

            this.DbContext.Add(tree);
        }

        private IQueryable<GenealogicalTreeDto> PrepareModel(IList<TreeLoadOptions> loadOptions = null)
        {
            IQueryable<GenealogicalTreeDto> model = DbContext.Trees;

            if (loadOptions is null) return model;

            if (loadOptions.Any(lo => lo == TreeLoadOptions.WithPersonData))
                model = model.Include(t => t.Persons);

            if (loadOptions.Any(lo => lo == TreeLoadOptions.WithModifiers))
                model = model.Include(t => t.Modifiers);

            return model;
        }

        public GenealogicalTreeDto GetTree(int treeId, bool trackEntity, IList<TreeLoadOptions> loadOptions = null) 
        {
            var model = PrepareModel(loadOptions);

            if (!trackEntity) model = model.AsNoTracking();
                
            return model.FirstOrDefault(t => t.Id == treeId);
        }
    }
}
