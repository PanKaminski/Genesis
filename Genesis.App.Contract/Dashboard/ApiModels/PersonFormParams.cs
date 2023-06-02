using Genesis.Common.Enums;

namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class PersonFormParams
    {
        public int? Id { get; set; }

        public Relation? NewRelation { get; set; }

        public int? PersonRelationFrom { get; set; }

        public int? PersonRelationTo { get; set; }

        public Gender Gender { get; set; }
    }
}
