using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.App.Contract.Models
{
    public class PersonRelation : AuditableEntity
    {
        public int FromPersonId { get; set; }
        public Person FromPerson { get; set; }

        public int ToPersonId { get; set; }
        public Person ToPerson { get; set; }

        public Relation RelationType { get; set; }
    }
}
