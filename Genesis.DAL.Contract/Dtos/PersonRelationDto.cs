using Genesis.Common;
using Genesis.Common.Enums;

namespace Genesis.DAL.Contract.Dtos
{
    public class PersonRelationDto : AuditableEntity
    {
        public int FromPersonId { get; set; }
        public PersonDto FromPerson { get; set; }

        public int ToPersonId { get; set; }
        public PersonDto ToPerson { get; set; }

        public int GenealogicalTreeId { get; set; }
        public GenealogicalTreeDto GenealogicalTree { get; set; }

        public Relation RelationType { get; set; }
    }
}
