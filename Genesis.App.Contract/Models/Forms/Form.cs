namespace Genesis.App.Contract.Models.Forms
{
    public class Form
    {
        public IEnumerable<Control> Controls { get; set; }

        public IEnumerable<FormTab> Tabs { get; set; }

        public IEnumerable<ButtonType> ButtonTypes { get; set; }

        public bool IsReadonly { get; set; }
    }
}
