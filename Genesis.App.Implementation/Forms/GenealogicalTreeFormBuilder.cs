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
        private readonly IPersonService personService;

        public GenealogicalTreeFormBuilder(IAccountService accountService, IGenealogicalTreeService treeService, 
            IPersonService personService, int currentUserId)
        {
            this.accountService = accountService;
            this.currentUserId = currentUserId;
            this.treeService = treeService;
            this.personService = personService;
        }

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

        protected override List<FormTab> BuildFormTabs(GenealogicalTree model) => new List<FormTab>
        {
            new FormTab(1, "Common"),
            new FormTab(2, "Notes"),
        };

        protected override IEnumerable<Control> CreateFormControls(GenealogicalTree tree)
        {
            var controls = new List<Control>
            {
                new Control
                {
                    EntityType = EntityType.Name,
                    Type = ControlType.TextInput,
                    Name = "Name",
                    IsRequired = true,
                    IsReadonly = tree.Id > 0 && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.Modifiers,
                    Type = ControlType.Select,
                    Name = "Modifiers",
                    IsRequired = false,
                    IsReadonly = tree.Id > 0 && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.Pictures,
                    Type = ControlType.SingleAvatar,
                    Name = "Avatar",
                    IsReadonly = tree.Id > 0 && tree.OwnerId != currentUserId,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.Note,
                    Type = ControlType.TextArea,
                    Name = "Description",
                    IsReadonly = tree.Id > 0 && tree.OwnerId != currentUserId,
                    TabId = 2,
                },
            };

            if (tree.Id < 1)
            {
                controls.Add(new Control
                {
                    EntityType = EntityType.RootPerson,
                    Type = ControlType.Select,
                    Name = "Root Person",
                    IsRequired = true,
                    TabId = 1,
                });
            }

            return controls;
        }

        protected override List<ButtonType> GetButtonTypes(GenealogicalTree tree)
        {
            var buttons = new List<ButtonType> { ButtonType.Close };

            if (tree is not null && tree.Id > 0 && tree.OwnerId == currentUserId) buttons.Add(ButtonType.Delete);
            if (tree is null || tree.Id < 1 || tree.OwnerId == currentUserId) buttons.Add(ButtonType.Save);

            return buttons;
        }

        protected override List<SelectItem> GetComboItems(EntityType itemCode, GenealogicalTree model)
        {
            var items = new List<SelectItem>();

            switch (itemCode)
            {
                case EntityType.Modifiers:
                    var connections = accountService.GetConnections(currentUserId).ToList();
                    items.AddRange(connections.Select(c => new SelectItem(c.GetRootPerson().FullName, c.Id.ToString())));
                    break;
                case EntityType.RootPerson:
                    var persons = personService.GetPersonsWithoutTree(currentUserId).ToList();
                    items.AddRange(persons.Select(p => new SelectItem(p.FullName, p.Id.ToString())));
                    break;
                default: break;
            }

            return items;
        }

        protected override object GetControlValue(Control control, GenealogicalTree tree)
        {
            switch (control.EntityType)
            {
                case EntityType.Modifiers when tree?.Modifiers is not null:
                    return tree.Modifiers.Select(m => m.Id.ToString()).ToList();
                case EntityType.Name:
                    return tree?.Name;
                case EntityType.Pictures when tree?.CoatOfArms is not null:
                    {
                        var ph = tree.CoatOfArms;
                        return new PictureResponse
                        {
                            Id = ph.Id,
                            Url = ph.Url,
                            PublicId = ph.PublicId
                        };
                    }
                case EntityType.Note when tree?.Description is not null:
                    return tree.Description;
                default:
                    return null;
            }
        }

        private void UpdateTreeModel(ControlValue value, GenealogicalTree tree)
        {
            switch (value.EntityType)
            {
                case EntityType.Modifiers when value.TryGet(out List<string> modifiersIds) && modifiersIds is not null:
                    tree.Modifiers = modifiersIds.Select(id => int.TryParse(id, out var idNumber) ? new Account(idNumber) : null)
                        .Where(acc => acc is not null).ToList();
                    break;
                case EntityType.Name when value.TryGet(out string treeName):
                    tree.Name = treeName;
                    break;
                case EntityType.RootPerson when value.TryGet(out string personId) && personId is not null:
                    if (int.TryParse(personId, out var idNumber)) 
                        tree.Persons = new List<Person> { new Person() { Id = idNumber } };
                    break;
                default:
                    break;
            }
        }
    }
}
