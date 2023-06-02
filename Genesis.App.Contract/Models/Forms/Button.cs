namespace Genesis.App.Contract.Models.Forms
{
    public class Button
    {
        public Button(ButtonType type, string name, bool isDisabled = false) =>
            (Type, Name, IsDisabled) = (type, name, isDisabled);

        public ButtonType Type { get; set; }

        public string Name { get; set; }

        public bool IsDisabled { get; set; }
    }
}
