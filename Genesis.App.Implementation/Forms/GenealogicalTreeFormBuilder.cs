using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Implementation.Forms
{
    public class GenealogicalTreeFormBuilder : FormBuilder<GenealogicalTree>
    {
        private readonly IAccountService accountService;
        private readonly int currentUserId;

        public GenealogicalTreeFormBuilder(IAccountService accountService, int currentUserId)
        {
            this.accountService = accountService;
        }

        protected override List<FormTab> FormTabs => new List<FormTab>
        {
            new FormTab(1, "Common"),
        };

        protected override IEnumerable<Control> CreateFormControls()
        {
            return new List<Control>
            {
                new Control
                {
                    EntityType = ControlEntityType.Name,
                    Type = ControlType.TextInput,
                    Name = "Name",
                    IsRequired = true,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.Modifiers,
                    Type = ControlType.Select,
                    Name = "Modifiers",
                    IsRequired = true,
                    TabId = 1,
                }
            };
        }

        protected override List<ButtonType> GetButtonTypes(GenealogicalTree model)
        {
            var buttons = new List<ButtonType> { ButtonType.Close };

            if (model is not null && model.Id > 0) buttons.Add(ButtonType.Delete);
            buttons.Add(ButtonType.Save);

            return buttons;
        }

        protected override List<SelectItem> GetComboItems(ControlEntityType itemCode, GenealogicalTree model)
        {
            var items = new List<SelectItem>();

            switch (itemCode)
            {
                case ControlEntityType.Modifiers:
                    var connections = accountService.GetConnections(currentUserId).ToList();
                    items.AddRange(connections.Select(c => new SelectItem(c.RootPerson.FullName, c.Id.ToString())));
                    break;
                default: break;
            }

            return items;
        }

        protected override object GetControlValue(Control control, GenealogicalTree model)
        {
            switch (control.EntityType)
            {
                case ControlEntityType.Modifiers when model?.Modifiers is not null:
                    return model.Modifiers.Select(m => m.Id.ToString()).ToList();
                case ControlEntityType.Name:
                    return model?.Name;
                default:
                    throw new KeyNotFoundException("Invalid entity type for tree form");
            }
        }
    }
}
