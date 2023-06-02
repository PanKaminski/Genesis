using Newtonsoft.Json.Linq;

namespace Genesis.App.Contract.Models.Forms
{
    public class SelectItem
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public SelectItem(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
