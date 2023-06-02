using Genesis.App.Contract.Models;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Dashboard.Services
{
    public interface IRelationsService
    {
        IEnumerable<PersonRelation> GetRelations(int personId, int? treeId = null, Relation? relType = null);

        void RemovePersonRelations(int personId, bool saveChanges);

        Task AddRelationsAsync(IEnumerable<PersonRelation> relations, int treeId, bool saveChanges = true);
    }
}
