using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Authentication;
using Genesis.App.Contract.Models.Forms;

namespace Genesis.App.Implementation.Forms
{
    public class GenealogicalTreeFormBuilder : FormBuilder<GenealogicalTree>
    {
        private readonly IAccountService accountService;
        private readonly IGenealogicalTreeService treeService;
        private readonly int currentUserId;

        public GenealogicalTreeFormBuilder(IAccountService accountService,
            IGenealogicalTreeService treeService, int currentUserId)
        {
            this.accountService = accountService;
            this.currentUserId = currentUserId;
            this.treeService = treeService;
        }

        protected override List<FormTab> FormTabs => new List<FormTab>
        {
            new FormTab(1, "Common"),
        };

        public async Task SaveFormAsync(GenealogicalTree tree, IEnumerable<ControlValue> formValues, SavePictureRequest picture)
        {
            if (picture is not null)
            {
                tree.CoatOfArms = new Picture(picture.Url, picture.PublicId, true);
            }

            foreach (var formValue in formValues)
            {
                UpdateTreeModel(formValue, tree);
            }

            if (tree.Id > 0)
            {
                await treeService.UpdateTreeAsync(tree, true);
            }
            else
            {
                tree.OwnerId = currentUserId;
                var id = await treeService.AddTreeAsync(tree, true);
                tree.Id = id;
            }
        }

        protected override IEnumerable<Control> CreateFormControls(GenealogicalTree tree)
        {
            return new List<Control>
            {
                new Control
                {
                    EntityType = ControlEntityType.Name,
                    Type = ControlType.TextInput,
                    Name = "Name",
                    IsRequired = true,
                    IsReadonly = tree is not null && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.Modifiers,
                    Type = ControlType.Select,
                    Name = "Modifiers",
                    IsRequired = false,
                    IsReadonly = tree is not null && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.Pictures,
                    Type = ControlType.Image,
                    Name = "Photos",
                    IsReadonly = tree is not null && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
            };
        }

        protected override List<ButtonType> GetButtonTypes(GenealogicalTree tree)
        {
            var buttons = new List<ButtonType> { ButtonType.Close };

            if (tree is not null && tree.Id > 0 && tree.OwnerId == currentUserId) buttons.Add(ButtonType.Delete);
            if (tree is null || tree.Id < 1 || tree.OwnerId == currentUserId) buttons.Add(ButtonType.Save);

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

        protected override object GetControlValue(Control control, GenealogicalTree tree)
        {
            switch (control.EntityType)
            {
                case ControlEntityType.Modifiers when tree?.Modifiers is not null:
                    return tree.Modifiers.Select(m => m.Id.ToString()).ToList();
                case ControlEntityType.Name:
                    return tree?.Name;
                case ControlEntityType.Pictures when tree?.CoatOfArms is not null:
                    {
                        var ph = tree.CoatOfArms;
                        return new PictureResponse
                        {
                            Id = ph.Id,
                            Url = ph.Url,
                            PublicId = ph.PublicId
                        };
                    }
                default:
                    throw new KeyNotFoundException("Invalid entity type for tree form");
            }
        }

        private void UpdateTreeModel(ControlValue value, GenealogicalTree tree)
        {
            switch (value.EntityType)
            {
                case ControlEntityType.Modifiers when value.TryGet(out List<string> modifiersIds) && modifiersIds is not null:
                    tree.Modifiers = modifiersIds.Select(id => int.TryParse(id, out var idNumber) ? new Account(idNumber) : null)
                        .Where(acc => acc is not null).ToList();
                    break;
                case ControlEntityType.Name when value.TryGet(out string treeName):
                    tree.Name = treeName;
                    break;
                default:
                    break;
            }
        }
    }
}
