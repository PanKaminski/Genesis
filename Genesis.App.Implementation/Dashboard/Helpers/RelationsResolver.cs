using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos;

namespace Genesis.App.Implementation.Dashboard.Helpers
{
    internal static class RelationsResolver
    {
        public static int? GetParent(int childId, IEnumerable<PersonRelationDto> relations, 
            IEnumerable<PersonDto> familyMembers, Gender gender)
        {
            var parentRels = relations.Where(r => r.FromPersonId == childId && r.RelationType == Relation.ChildToParent);
            var parents = familyMembers.Where(p => parentRels.Any(r => r.ToPersonId == p.Id));

            return parents.FirstOrDefault(p => p.Gender == gender)?.Id;
        }
    }
}
