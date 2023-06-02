namespace Genesis.App.Contract.Models.Forms
{
    public class FormTab
    {
        public FormTab(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
