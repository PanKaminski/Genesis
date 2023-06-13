using Newtonsoft.Json.Linq;

namespace Genesis.App.Contract.Models.Forms
{
    public class Control
    {
        public EntityType EntityType { get; set; }

        public ControlType Type { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsRequired { get; set; }

        public bool IsMulty { get; set; }

        public int TabId { get; set; }

        public IEnumerable<SelectItem> Items { get; set; }
    }
}
