namespace Genesis.App.Contract.Models.Tables
{
    public class Row
    {
        public int Id { get; set; }

        public IEnumerable<Cell> Cells { get; set; }

        public bool IsRemovable { get; set; } = true;

        public bool CanCopy { get; set; } = false;
    }
}
