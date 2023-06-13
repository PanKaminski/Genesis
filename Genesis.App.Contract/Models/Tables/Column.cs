using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Contract.Models.Tables
{
    public class Column
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ColumnType Type { get; set; }

        public EntityType? EntityType { get; set; }
    }
}
