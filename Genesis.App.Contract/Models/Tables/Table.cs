namespace Genesis.App.Contract.Models.Tables
{
    public class Table
    {
        public IEnumerable<Row> Rows { get; set; }

        public IEnumerable<Column> Columns { get; set; }
    }
}
