using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Forms;
using Genesis.Common.Enums;
using Genesis.Common.Extensions;

namespace Genesis.App.Implementation.Forms
{
    public class PersonFormBuilder : FormBuilder<Person>
    {
        private readonly IPersonService personService;
        private readonly IRelationsService relationsService;
        private readonly IPhotoService photoService;
        private readonly PersonRelation personRelation;
        protected override List<FormTab> FormTabs => new List<FormTab>
        {
            new FormTab(1, "Common"),
            new FormTab(2, "Relations"),
            new FormTab(3, "Pictures"),
            new FormTab(4, "Notes"),
        };

        private List<PersonRelation> rootPersonRelations;
        private List<PersonRelation> RootPersonRelations => rootPersonRelations ?? relationsService.GetRelations(linkedPersonId, null).ToList();
        private int linkedPersonId => personRelation.FromPerson is null ? personRelation.ToPersonId : personRelation.FromPersonId;
        private List<PersonRelation> personRelations  = new List<PersonRelation>();

        public PersonFormBuilder(PersonRelation relation, IPersonService personService, IRelationsService relationsService, IPhotoService photoService)
        {
            this.personRelation = relation ?? throw new ArgumentNullException(nameof(relation));
            this.personService = personService;
            this.relationsService = relationsService;
            this.photoService = photoService;
        }

        public async Task SaveFormAsync(Person person, IEnumerable<ControlValue> formValues, IEnumerable<EditPictureRequest> updatedPhotos,
            IEnumerable<int> removedPhotos, IEnumerable<SavePictureRequest> addedPhotos) 
        {
            var relations = new List<PersonRelation>();
            foreach (var formValue in formValues)
            {
                UpdatePersonModel(formValue, person, relations);
            }

            var linkedPerson = personRelation.ToPerson ?? personRelation.FromPerson;
            var treeId = person?.GenealogicalTree?.Id ?? linkedPerson.GenealogicalTree?.Id ?? 0;

            if (person.Id > 0)
            {
                await personService.EditPersonAsync(person);
            }
            else
            {
                person.Id = await personService.AddPersonAsync(person, treeId, true);
            }

            await EditPersonAlbumAsync(person, updatedPhotos, removedPhotos, addedPhotos);

            await relationsService.AddRelationsAsync(relations, treeId, true);
        }

        protected override IEnumerable<Control> CreateFormControls(Person person)
        {
            var linkedPerson = personRelation.ToPerson ?? personRelation.FromPerson;
            return new List<Control>()
            {
                new Control
                {
                    EntityType = ControlEntityType.FirstName,
                    Type = ControlType.TextInput,
                    Name = "First Name",
                    IsRequired = true,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.MiddleName,
                    Type = ControlType.TextInput,
                    Name = "Middle Name",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.LastName,
                    Type = ControlType.TextInput,
                    Name = "Last Name",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.Gender,
                    Type = ControlType.Select,
                    Name = "Gender",
                    IsReadonly = true,
                    IsRequired = true,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.DateOfBirth,
                    Type = ControlType.DatePicker,
                    Name = "Date of Birthday",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.DateOfDeath,
                    Type = ControlType.DatePicker,
                    Name = "Date of Death",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = ControlEntityType.Mother,
                    Type = ControlType.Select,
                    Name = "Mother",
                    IsReadonly = !(linkedPerson is not null && personRelation.ToPerson is not null && personRelation.RelationType == Relation.ChildToParent && linkedPerson.Gender == Gender.Man),
                    TabId = 2,
                },
                new Control
                {
                    EntityType = ControlEntityType.Father,
                    Type = ControlType.Select,
                    Name = "Father",
                    IsReadonly = !(linkedPerson is not null && personRelation.ToPerson is not null && personRelation.RelationType == Relation.ChildToParent && linkedPerson.Gender == Gender.Woman),
                    TabId = 2,
                },
                new Control
                {
                    EntityType = ControlEntityType.Children,
                    Type = ControlType.Select,
                    Name = "Children",
                    IsMulty = true,
                    IsReadonly = !(linkedPerson is not null && personRelation.RelationType == Relation.Partners),
                    TabId = 2,
                },
                new Control
                {
                    EntityType = ControlEntityType.Partners,
                    Type = ControlType.Select,
                    Name = "Partners",
                    IsMulty = true,
                    IsReadonly = true,
                    TabId = 2,
                },
                new Control
                {
                    EntityType = ControlEntityType.Note,
                    Type = ControlType.TextArea,
                    Name = "Note",
                    TabId = 4,
                },
                new Control
                {
                    EntityType = ControlEntityType.Pictures,
                    Type = ControlType.Image,
                    Name = "Photos",
                    TabId = 3,
                },
            };
        }

        protected override void PrepareModel(Person person)
        {
            if (person.Id > 0)
            {
                personRelations = relationsService
                    .GetRelations(person.Id, person.GenealogicalTree.Id)
                    .ToList();
            }
        }

        protected override object GetControlValue(Control control, Person person)
        {
            switch (control.EntityType)
            {
                case ControlEntityType.FirstName:
                    return person?.FirstName ?? string.Empty;
                case ControlEntityType.MiddleName:
                    return person?.MiddleName ?? string.Empty;
                case ControlEntityType.LastName:
                    return person?.LastName ?? string.Empty;
                case ControlEntityType.Gender:
                    return person?.Gender is not null ? ((byte)person.Gender).ToString() : ((byte)person.Gender).ToString();
                case ControlEntityType.DateOfBirth:
                    return person?.Biography is not null ? person.Biography.BirthDate : null;
                case ControlEntityType.DateOfDeath:
                    return person?.Biography is not null ? person.Biography.DeathDate : null;
                case ControlEntityType.Note:
                    return person?.Biography?.Info is not null ? person.Biography.Info : string.Empty;
                case ControlEntityType.Mother:
                    {
                        if (personRelation.RelationType == Relation.ChildToParent && personRelation.ToPerson
                            is { Gender: Gender.Woman })
                        {
                            return linkedPersonId.ToString();
                        }

                        if (personRelations.TryGetSingleValue(r => r.FromPersonId == person?.Id &&
                            r.RelationType == Relation.ChildToParent && r.ToPerson.Gender == Gender.Woman, out PersonRelation parentRel))
                            return parentRel.ToPersonId.ToString();
                        return null;
                    }
                case ControlEntityType.Father:
                    {
                        if (personRelation.RelationType == Relation.ChildToParent && 
                            personRelation.ToPerson is { Gender: Gender.Man })
                        {
                            return linkedPersonId.ToString();
                        }

                        if (personRelations.TryGetSingleValue(r => r.FromPersonId == person?.Id &&
                            r.RelationType == Relation.ChildToParent && r.ToPerson.Gender == Gender.Man, out PersonRelation parentRel))
                            return parentRel.ToPersonId.ToString();
                        return null;
                    }
                case ControlEntityType.Partners:
                    {
                        if (personRelation.RelationType == Relation.Partners && linkedPersonId > 0)
                        {
                            return new string[] { linkedPersonId.ToString() };
                        }

                        return personRelations.Where(r => r.FromPersonId == person?.Id && r.RelationType == Relation.Partners)
                            .Select(pr => pr.ToPersonId == person?.Id ? pr.FromPersonId.ToString() : pr.ToPersonId.ToString());
                    }
                case ControlEntityType.Children:
                    {
                        var children = personRelations.Where(r => r.ToPersonId == person?.Id &&
                            r.RelationType == Relation.ChildToParent).Select(r => r.FromPersonId.ToString()).ToList();

                        if (personRelation.RelationType == Relation.ChildToParent && personRelation.FromPerson is { })
                        {
                            children.Add(linkedPersonId.ToString());
                        }

                        return children.Any() ? children : null;
                    }
                case ControlEntityType.Pictures when person?.Photos is not null:
                    {
                        return person.Photos.Select(ph => new PictureResponse
                        {
                            Id = ph.Id,
                            Url = ph.Url,
                            PublicId = ph.PublicId
                        });
                    }
                default: return null;
            }
        }

        protected override List<SelectItem> GetComboItems(ControlEntityType itemCode, Person person)
        {
            var items = new List<SelectItem>();
            var linkedPerson = personRelation.ToPerson ?? personRelation.FromPerson;

            switch (itemCode)
            {
                case ControlEntityType.Gender:
                    items = new List<SelectItem>() 
                    { 
                        new SelectItem(Gender.Man.ToString(), ((byte)Gender.Man).ToString()),
                        new SelectItem(Gender.Woman.ToString(), ((byte)Gender.Woman).ToString())
                    };
                    break;
                case ControlEntityType.Father:
                case ControlEntityType.Mother:
                    var gender = itemCode == ControlEntityType.Father ? Gender.Man : Gender.Woman;
                    if (person.Id > 0)
                    {
                        items = personRelations.Where(r => r.RelationType == Relation.ChildToParent && r.FromPersonId == person.Id
                            && r.ToPerson.Gender == gender).Select(r => new SelectItem(r.ToPerson.FullName, r.ToPersonId.ToString())).ToList();
                    }
                    else if (personRelation.RelationType == Relation.ChildToParent
                        && linkedPerson.Gender == gender && personRelation.ToPersonId == linkedPersonId)
                    {
                        items = new List<SelectItem>()
                        {
                            new(linkedPerson.FullName, linkedPersonId.ToString()),
                        };
                    }
                    else if (linkedPerson.Gender == gender.GetOppositeGender())
                    {
                        items = RootPersonRelations.Where(r => r.RelationType == Relation.Partners)
                            .Select(r => r.FromPersonId == linkedPerson.Id ? new SelectItem(r.ToPerson.FullName, r.ToPersonId.ToString())
                            : new SelectItem(r.FromPerson.FullName, r.FromPersonId.ToString())).ToList();
                    }
                    break;
                case ControlEntityType.Children:
                    List<PersonRelation> childrenRels = null;

                    if (person.Id > 0)
                    {
                        childrenRels = personRelations.Where(r => r.RelationType == Relation.ChildToParent && r.ToPersonId == person.Id)
                            .ToList();
                    }
                    else if (personRelation.RelationType == Relation.Partners)
                    {
                        childrenRels = RootPersonRelations.Where(r => r.RelationType == Relation.ChildToParent
                            && r.ToPersonId == linkedPersonId && relationsService.GetRelations(r.FromPersonId, null, Relation.ChildToParent)
                                .Where(chr => chr.FromPersonId == chr.FromPersonId).Count() == 1)
                            .ToList();
                    }
                    else
                    {
                        childrenRels = new List<PersonRelation> 
                        { 
                            new () 
                            { 
                                FromPerson = linkedPerson, 
                                ToPerson = person, 
                                RelationType = Relation.ChildToParent,
                                FromPersonId = linkedPerson.Id,
                            } 
                        };
                    }

                    items = childrenRels.Select(r => new SelectItem(r.FromPerson.FullName, r.FromPersonId.ToString())).ToList();
                    break;
                case ControlEntityType.Partners:
                    if (person.Id > 0)
                    {
                        items = personRelations.Where(r => r.RelationType == Relation.Partners)
                            .Select(r => r.FromPersonId == person.Id ? new SelectItem(r.ToPerson.FullName, r.ToPersonId.ToString())
                                : new SelectItem(r.FromPerson.FullName, r.FromPersonId.ToString()))
                            .ToList();
                    }
                    else if (personRelation.RelationType == Relation.Partners)
                    {
                        items = new List<SelectItem> { new(linkedPerson.FullName, linkedPersonId.ToString()) };
                    }
                    break;
                default:
                    break;
            }

            return items;
        }

        protected override List<ButtonType> GetButtonTypes(Person person)
        {
            var buttons = new List<ButtonType> { ButtonType.Close };

            if (person is not null) buttons.Add(ButtonType.Delete);
            buttons.Add(ButtonType.Save);

            return buttons;
        }

        private void UpdatePersonModel(ControlValue value, Person person, List<PersonRelation> relations)
        {
            switch (value.EntityType)
            {
                case ControlEntityType.FirstName when value.TryGet(out string firstName):
                    person.FirstName = firstName;
                    break;
                case ControlEntityType.LastName when value.TryGet(out string lastName):
                    person.LastName = lastName;
                    break;
                case ControlEntityType.MiddleName when value.TryGet(out string middleName):
                    person.MiddleName = middleName;
                    break;
                case ControlEntityType.Gender when value.TryGet(out string genderValue) && byte.TryParse(genderValue, out var gender):
                    person.Gender =  (Gender)gender;
                    break;
                case ControlEntityType.DateOfBirth when value.TryGet(out DateTime birthDate):
                    person.Biography.BirthDate = birthDate;
                    break;
                case ControlEntityType.DateOfDeath when value.TryGet(out DateTime deathDate):
                    person.Biography.DeathDate = deathDate;
                    break;
                case ControlEntityType.Father when person.Id == 0 && value.TryGet(out string fatherIdValue) && int.TryParse(fatherIdValue, out var fatherId):
                    relations.Add(new PersonRelation
                    {
                        FromPerson = person,
                        ToPersonId = fatherId,
                        RelationType = Relation.ChildToParent,
                    });
                    break;
                case ControlEntityType.Mother when person.Id == 0 && value.TryGet(out string motherIdValue) && int.TryParse(motherIdValue, out var motherId):
                    relations.Add(new PersonRelation
                    {
                        FromPerson = person,
                        ToPersonId = motherId,
                        RelationType = Relation.ChildToParent,
                    });
                    break;
                case ControlEntityType.Children when person.Id == 0 && value.TryGet(out List<string> childrenIds) && childrenIds is not null:
                    foreach (var childIdValue in childrenIds)
                    {
                        if (int.TryParse(childIdValue, out var childId))
                        {
                            relations.Add(new PersonRelation
                            {
                                ToPerson = person,
                                FromPersonId = childId,
                                RelationType = Relation.ChildToParent,
                            });
                        }
                    }
                    break;
                case ControlEntityType.Partners when person.Id == 0 && value.TryGet(out List<string> partnerIds) && partnerIds is not null:
                    foreach (var partnerIdValue in partnerIds)
                    {
                        if (int.TryParse(partnerIdValue, out var partnerId))
                        {
                            relations.Add(new PersonRelation
                            {
                                ToPerson = person,
                                FromPersonId = partnerId,
                                RelationType = Relation.Partners,
                            });
                        }
                    }
                    break;
                case ControlEntityType.Country:
                    break;
                case ControlEntityType.Note when value.TryGet(out string info):
                    person.Biography.Info = info;
                    break;
                default:
                    break;
            }
        }

        private async Task EditPersonAlbumAsync(Person person, IEnumerable<EditPictureRequest> updatedPhotos,
            IEnumerable<int> removedPhotos, IEnumerable<SavePictureRequest> addedPhotos)
        {
            if (removedPhotos.Any())
            {
                photoService.RemovePictures(removedPhotos, false);
            }

            if (updatedPhotos.Any())
            {
                photoService.MakeAvatar(person.Id, updatedPhotos.First().Id, false);
            }

            if (addedPhotos.Any())
            {
                var picturesList = addedPhotos.Select(p => new Picture(p.Url, p.PublicId, p.IsMain)).ToList();
                await photoService.SavePersonPictures(person.Id, picturesList, false);
            }
        }
    }
}
