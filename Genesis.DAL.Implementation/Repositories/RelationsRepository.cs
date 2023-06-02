using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class RelationsRepository : RepositoryBase<PersonRelationDto>, IRelationsRepository
    {
        public RelationsRepository(GenesisDbContext dbContext) : base(dbContext)
        {
        }

        public async Task AddRelationsAsync(IEnumerable<PersonRelationDto> relations, int treeId)
        {
            var relationsList = relations.ToList();
            relationsList.ForEach(r => 
                {
                    r.GenealogicalTreeId = treeId;
                    r.FromPersonId = r.FromPersonId == 0 ? r.FromPerson.Id : r.FromPersonId;
                    r.ToPersonId = r.ToPersonId == 0 ? r.ToPerson.Id : r.ToPersonId;
                    r.ToPerson = null;
                    r.FromPerson = null;
                    r.CreatedTime = DateTime.Now;
                }
            );

            await DbContext.Relations.AddRangeAsync(relationsList);
        }

        public IEnumerable<PersonRelationDto> GetRelations(int treeId) =>
            DbContext.Relations.AsNoTracking().Where(r => r.GenealogicalTreeId == treeId);

        public IEnumerable<PersonRelationDto> GetRelationsWithPersons(int personId, Relation? relationType = null, int? genealogicalTreeId = null)
        {
            var relations = DbContext.Relations.Include(r => r.FromPerson).Include(r => r.ToPerson).AsNoTracking();

            if (relationType is not null)
                relations = relations.Where(r => r.RelationType == relationType);

            if (genealogicalTreeId is not null)
                relations = relations.Where(r => r.GenealogicalTreeId == genealogicalTreeId);

            return relations.Where(r => r.FromPersonId == personId || r.ToPersonId == personId);
        }

        public void RemovePersonRelations(int personId)
        {
            var relations = DbContext.Relations.Where(r => r.FromPersonId == personId || r.ToPersonId == personId);

            DbContext.Relations.RemoveRange(relations);
        }
    }
}
