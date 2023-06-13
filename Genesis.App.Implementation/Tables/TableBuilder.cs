using Genesis.App.Contract.Models.Tables;


namespace Genesis.App.Implementation.Tables
{
    public abstract class TableBuilder<T>
    {
        public Table GenerateTable(IList<T> data)
        {
            var columns = GetColumns();
            columns.Insert(0, CreateSelectColumn());
            var rows = GenerateRows(data, columns);

            return new Table
            {
                Rows = rows,
                Columns = columns,
            };
        }

        protected IEnumerable<Row> GenerateRows(IList<T> data, List<Column> columns)
        {
            for (int i = 0; i < data.Count; i++)
            {
                yield return GenerateRow(data[i], columns, i);
            }
        }

        public abstract List<Column> GetColumns();

        public virtual Row GenerateRow(T model, List<Column> columns, int index)
        {
            var cells = columns.Select(c => GetCell(model, c));

            return new Row
            { 
                Id = GetRowId(model, index), 
                Cells = cells, 
                IsRemovable = IsRemovableRow(model),
            };
        }

        protected abstract bool IsRemovableRow(T model);

        protected abstract Cell GetCell(T model, Column column);

        protected abstract int GetRowId(T model, int index);

        private Column CreateSelectColumn() => new Column
        {
            Id = 0,
            Name = "-",
            Type = ColumnType.CheckBox,
        };
    }
}
