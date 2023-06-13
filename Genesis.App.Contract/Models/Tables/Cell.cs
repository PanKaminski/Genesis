using Newtonsoft.Json.Linq;

namespace Genesis.App.Contract.Models.Tables
{
    public class Cell
    {
        public int ColumnId { get; set; }

        public JToken Value { get; set; }
    }
}
