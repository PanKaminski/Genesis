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

        private List<PersonRelation> rootPersonRelations;
        private List<PersonRelation> RootPersonRelations => rootPersonRelations ?? relationsService.GetRelations(linkedPersonId, null).ToList();
        private int linkedPersonId => personRelation.FromPerson is null ? personRelation.ToPersonId : personRelation.FromPersonId;
        private List<PersonRelation> personRelations = new List<PersonRelation>();

        public PersonFormBuilder(IPersonService personService, IRelationsService relationsService, IPhotoService photoService)
            : this(new PersonRelation(), personService, relationsService,photoService) { }

        public PersonFormBuilder(PersonRelation relation, IPersonService personService, IRelationsService relationsService, IPhotoService photoService)
        {
            this.personRelation = relation;
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
            var treeId = person?.GenealogicalTree?.Id ?? linkedPerson?.GenealogicalTree?.Id;

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

        protected override List<FormTab> BuildFormTabs(Person person)
        {
            var tabs = new List<FormTab>
            {
                new FormTab(1, "Common"),
                new FormTab(3, "Pictures"),
                new FormTab(4, "Notes"),
            };

            if (!IsNewPersonWithoutRelation(person))
            {
                tabs.Add(new FormTab(2, "Relations"));
            }

            return tabs.OrderBy(t => t.Id).ToList();
        }

        protected override IEnumerable<Control> CreateFormControls(Person person)
        {
            var linkedPerson = personRelation.ToPerson ?? personRelation.FromPerson;
            var controls = new List<Control>()
            {
                new Control
                {
                    EntityType = EntityType.FirstName,
                    Type = ControlType.TextInput,
                    Name = "First Name",
                    IsRequired = true,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.MiddleName,
                    Type = ControlType.TextInput,
                    Name = "Middle Name",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.LastName,
                    Type = ControlType.TextInput,
                    Name = "Last Name",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.Gender,
                    Type = ControlType.Select,
                    Name = "Gender",
                    IsReadonly = Enum.IsDefined(typeof(Gender), (byte)person.Gender),
                    IsRequired = true,
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.DateOfBirth,
                    Type = ControlType.DatePicker,
                    Name = "Date of Birthday",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.DateOfDeath,
                    Type = ControlType.DatePicker,
                    Name = "Date of Death",
                    TabId = 1,
                },
                new Control
                {
                    EntityType = EntityType.Note,
                    Type = ControlType.TextArea,
                    Name = "Note",
                    TabId = 4,
                },
                new Control
                {
                    EntityType = EntityType.Pictures,
                    Type = ControlType.Image,
                    Name = "Photos",
                    TabId = 3,
                },
            };

            if (!IsNewPersonWithoutRelation(person))
            {
                controls.AddRange(new[] {
                    new Control
                    {
                        EntityType = EntityType.Mother,
                        Type = ControlType.Select,
                        Name = "Mother",
                        IsReadonly = !IsNewPersonWithoutRelation(person)
                        && !(HasRelation() && personRelation.ToPerson is not null && personRelation.RelationType == Relation.ChildToParent && linkedPerson.Gender == Gender.Man),
                        TabId = 2,
                    },
                    new Control
                    {
                        EntityType = EntityType.Father,
                        Type = ControlType.Select,
                        Name = "Father",
                        IsReadonly =!IsNewPersonWithoutRelation(person)
                        && !(HasRelation() && personRelation.ToPerson is not null && personRelation.RelationType == Relation.ChildToParent && linkedPerson.Gender == Gender.Woman),
                        TabId = 2,
                    },
                    new Control
                    {
                        EntityType = EntityType.Children,
                        Type = ControlType.Select,
                        Name = "Children",
                        IsMulty = true,
                        IsReadonly = !IsNewPersonWithoutRelation(person)
                        && !(HasRelation() && personRelation.RelationType == Relation.Partners),
                        TabId = 2,
                    },
                    new Control
                    {
                        EntityType = EntityType.Partners,
                        Type = ControlType.Select,
                        Name = "Partners",
                        IsMulty = true,
                        IsReadonly = !IsNewPersonWithoutRelation(person),
                        TabId = 2,
                    },
                });
            }

            return controls;
        }

        protected override void PrepareModel(Person person)
        {
            if (!IsNewPerson(person))
            {
                personRelations = relationsService
                    .GetRelations(person.Id, person.GenealogicalTreeId)
                    .ToList();
            }
        }

        protected override object GetControlValue(Control control, Person person)
        {
            switch (control.EntityType)
            {
                case EntityType.FirstName:
                    return person?.FirstName ?? string.Empty;
                case EntityType.MiddleName:
                    return person?.MiddleName ?? string.Empty;
                case EntityType.LastName:
                    return person?.LastName ?? string.Empty;
                case EntityType.Gender:
                    var isValud = Enum.IsDefined(typeof(Gender), (byte)person.Gender);
                    return isValud ? ((byte)person.Gender).ToString() : null;
                case EntityType.DateOfBirth:
                    return person?.Biography is not null ? person.Biography.BirthDate : null;
                case EntityType.DateOfDeath:
                    return person?.Biography is not null ? person.Biography.DeathDate : null;
                case EntityType.Note:
                    return person?.Biography?.Info is not null ? person.Biography.Info : string.Empty;
                case EntityType.Mother:
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
                case EntityType.Father:
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
                case EntityType.Partners:
                    {
                        if (personRelation.RelationType == Relation.Partners && linkedPersonId > 0)
                        {
                            return new string[] { linkedPersonId.ToString() };
                        }

                        return personRelations.Where(r => r.FromPersonId == person?.Id && r.RelationType == Relation.Partners)
                            .Select(pr => pr.ToPersonId == person?.Id ? pr.FromPersonId.ToString() : pr.ToPersonId.ToString());
                    }
                case EntityType.Children:
                    {
                        var children = personRelations.Where(r => r.ToPersonId == person?.Id &&
                            r.RelationType == Relation.ChildToParent).Select(r => r.FromPersonId.ToString()).ToList();

                        if (personRelation.RelationType == Relation.ChildToParent && personRelation.FromPerson is { })
                        {
                            children.Add(linkedPersonId.ToString());
                        }

                        return children.Any() ? children : null;
                    }
                case EntityType.Pictures when person?.Photos is not null:
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

        protected override List<SelectItem> GetComboItems(EntityType itemCode, Person person)
        {
            var items = new List<SelectItem>();
            var linkedPerson = personRelation.ToPerson ?? personRelation.FromPerson;

            switch (itemCode)
            {
                case EntityType.Gender:
                    items = new List<SelectItem>()
                    {
                        new SelectItem(Gender.Man.ToString(), ((byte)Gender.Man).ToString()),
                        new SelectItem(Gender.Woman.ToString(), ((byte)Gender.Woman).ToString())
                    };
                    break;
                case EntityType.Father:
                case EntityType.Mother:
                    var gender = itemCode == EntityType.Father ? Gender.Man : Gender.Woman;
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
                case EntityType.Children:
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
                case EntityType.Partners:
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

            if (!IsNewPerson(person)) buttons.Add(ButtonType.Delete);
            buttons.Add(ButtonType.Save);

            return buttons;
        }

        private void UpdatePersonModel(ControlValue value, Person person, List<PersonRelation> relations)
        {
            switch (value.EntityType)
            {
                case EntityType.FirstName when value.TryGet(out string firstName):
                    person.FirstName = firstName;
                    break;
                case EntityType.LastName when value.TryGet(out string lastName):
                    person.LastName = lastName;
                    break;
                case EntityType.MiddleName when value.TryGet(out string middleName):
                    person.MiddleName = middleName;
                    break;
                case EntityType.Gender when value.TryGet(out string genderValue) && byte.TryParse(genderValue, out var gender):
                    person.Gender = (Gender)gender;
                    break;
                case EntityType.DateOfBirth when value.TryGet(out DateTime birthDate):
                    person.Biography.BirthDate = birthDate;
                    break;
                case EntityType.DateOfDeath when value.TryGet(out DateTime deathDate):
                    person.Biography.DeathDate = deathDate;
                    break;
                case EntityType.Father when person.Id == 0 && value.TryGet(out string fatherIdValue) && int.TryParse(fatherIdValue, out var fatherId):
                    relations.Add(new PersonRelation
                    {
                        FromPerson = person,
                        ToPersonId = fatherId,
                        RelationType = Relation.ChildToParent,
                    });
                    break;
                case EntityType.Mother when person.Id == 0 && value.TryGet(out string motherIdValue) && int.TryParse(motherIdValue, out var motherId):
                    relations.Add(new PersonRelation
                    {
                        FromPerson = person,
                        ToPersonId = motherId,
                        RelationType = Relation.ChildToParent,
                    });
                    break;
                case EntityType.Children when person.Id == 0 && value.TryGet(out List<string> childrenIds) && childrenIds is not null:
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
                case EntityType.Partners when person.Id == 0 && value.TryGet(out List<string> partnerIds) && partnerIds is not null:
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
                case EntityType.Country:
                    break;
                case EntityType.Note when value.TryGet(out string info):
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

        public bool IsNewPerson(Person person) => person.Id <= 0;
        public bool HasRelation() => linkedPersonId > 0;
        public bool IsNewPersonWithoutRelation(Person person) => IsNewPerson(person) && !HasRelation();
    }
}