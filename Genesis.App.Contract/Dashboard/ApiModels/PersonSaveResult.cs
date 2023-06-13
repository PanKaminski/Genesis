using Genesis.App.Contract.Models.Tables;

namespace Genesis.App.Contract.Dashboard.ApiModels
{
    public class PersonSaveResult
    {
        public TreeNodeResponse Node { get; set; }

        public Row Row { get; set; }
    }
}
