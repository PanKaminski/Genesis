using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IRelationsRepository
    {
        Task AddRelationsAsync(IEnumerable<PersonRelationDto> relations, int treeId);

        IEnumerable<PersonRelationDto> GetRelations(int treeId);

        IEnumerable<PersonRelationDto> GetRelationsWithPersons(int personId, Relation? relationType = null, int? genealogicalTreeId = null);

        void RemovePersonRelations(int personId);
    }
}
