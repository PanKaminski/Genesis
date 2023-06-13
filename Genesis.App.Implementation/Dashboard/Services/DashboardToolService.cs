using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Forms;
using Genesis.App.Contract.Models.Responses;
using Genesis.App.Contract.Models.Tables;
using Genesis.App.Implementation.Forms;
using Genesis.App.Implementation.Tables;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;
using System.Globalization;
using System.Xml.Linq;

namespace Genesis.App.Implementation.Dashboard.Services
{
    public class DashboardToolService
    {
        private readonly IPersonService personService;
        private readonly IRelationsService relationsService;
        private readonly IGenealogicalTreeService treeService;
        private readonly IPhotoService photoService;
        private readonly IAccountService accountService;

        public DashboardToolService(IPersonService personService, IRelationsService relationsService, 
            IPhotoService photoService, IGenealogicalTreeService treeService, IAccountService accountService)
        {
            this.personService = personService;
            this.relationsService = relationsService;
            this.photoService = photoService;
            this.treeService = treeService;
            this.accountService = accountService;
        }

        public TreesListResponse GetAllUserTrees(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User identifier is null or empty.");

            if (!int.TryParse(userId, out var idNumber))
                throw new GenesisApplicationException("User id is not number");

            var treesList = new TreesListResponse();
            foreach (var tree in treeService.GetAllUserTrees(idNumber))
            {
                treesList.Add(
                    tree.Id,
                    tree.Persons.Count,
                    tree.Name,
                    tree.UpdatedTime ?? tree.CreatedTime,
                    tree.OwnerId == idNumber
                );
            }

            return treesList;
        }

        public Form GetPersonForm(PersonFormParams formParams)
        {
            if (formParams.NewRelation is null && formParams.Id is null)
                return GetDraftPersonForm();

            var person = formParams.Id is not null ? personService.GetPersonWithFullInfo(formParams.Id.Value) 
                : new Person() { Gender = formParams.Gender, };
            var linkedPersonId = formParams.PersonRelationFrom is null ? formParams.PersonRelationTo : formParams.PersonRelationFrom;
            var linkedPerson = linkedPersonId is not null ? personService.GetPerson(linkedPersonId.Value) : new Person();

            var personRelation = new PersonRelation()
            {
                FromPersonId = formParams.PersonRelationFrom is null ? default : linkedPerson.Id,
                FromPerson = formParams.PersonRelationFrom is null ? null : linkedPerson,
                ToPersonId = formParams.PersonRelationTo is null ? default : linkedPerson.Id,
                ToPerson = formParams.PersonRelationTo is null ? null : linkedPerson,
                RelationType = formParams.NewRelation is null ? default : formParams.NewRelation.Value
            };

            var builder = new PersonFormBuilder(personRelation, personService, relationsService, photoService);

            return builder.BuildForm(person);
        }

        public async Task<ServerResponse<PersonSaveResult>> SavePersonFormAsync(PersonEditModel editModel, string currentAccountId)
        {
            try
            {
                if (!int.TryParse(currentAccountId, out int accountId) || accountId < 1)
                {
                    throw new ArgumentException("Invalid user", nameof(currentAccountId));
                }

                var person = editModel.PersonEditorInfo.Id is not null ? personService.GetPersonWithFullInfo(editModel.PersonEditorInfo.Id.Value)
                    : new Person { Gender = editModel.PersonEditorInfo.Gender, AccountId = accountId };
                var linkedPersonId = editModel.PersonEditorInfo.PersonRelationFrom is null ? editModel.PersonEditorInfo.PersonRelationTo 
                    : editModel.PersonEditorInfo.PersonRelationFrom;
                var linkedPerson = linkedPersonId is not null ? personService.GetPersonWithGenealogicalTree(linkedPersonId.Value)
                    : new Person();

                var personRelation = new PersonRelation()
                {
                    FromPersonId = editModel.PersonEditorInfo.PersonRelationFrom is null ? default : linkedPerson.Id,
                    FromPerson = editModel.PersonEditorInfo.PersonRelationFrom is null ? null : linkedPerson,
                    ToPersonId = editModel.PersonEditorInfo.PersonRelationTo is null ? default : linkedPerson.Id,
                    ToPerson = editModel.PersonEditorInfo.PersonRelationTo is null ? null : linkedPerson,
                    RelationType = editModel.PersonEditorInfo.NewRelation is null ? default : editModel.PersonEditorInfo.NewRelation.Value
                };

                var builder = new PersonFormBuilder(personRelation, personService, relationsService, photoService);

                await builder.SaveFormAsync(person, editModel.FormValues, editModel.UpdatedPhotos, editModel.RemovedPhotos, editModel.AddedPhotos);
                var saveResult = new PersonSaveResult
                {
                    Node = await CreateTreeNodeAsync(person),
                    Row = GeneratePersonRow(person, accountId),
                };

                return new ServerResponse<PersonSaveResult>(ResultCode.Done, saveResult , "Person succefully saved");
            }
            catch(GenesisApplicationException ex)
            {
                return new ServerResponse<PersonSaveResult>(ResultCode.Failed, null, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServerResponse<PersonSaveResult>(ResultCode.Failed, null, "Person save failed");
            }
        }

        public ServerResponse DeletePerson(int personId)
        {
            try
            {
                relationsService.RemovePersonRelations(personId, false);
                photoService.RemovePersonPictures(personId, false);
                personService.RemovePerson(personId, true);

                return new ServerResponse(ResultCode.Done, "Person was deleted");
            }
            catch (Exception exc)
            {
                return new ServerResponse(ResultCode.Failed, exc.Message);
            }
        }

        public ServerResponse DeletePersons(IEnumerable<int> ids)
        {
            try
            {
                relationsService.RemovePersonsRelations(ids, false);
                photoService.RemovePersonsPictures(ids, false);
                personService.RemovePersons(ids, true);

                return new ServerResponse(ResultCode.Done, "Person was deleted");
            }
            catch (Exception exc)
            {
                return new ServerResponse(ResultCode.Failed, exc.Message);
            }
        }

        public bool CanAccessTree(int treeId, string currentUserId)
        {
            if (!int.TryParse(currentUserId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentUserId));
            }

            var tree = treeService.GetTreeWithModifiers(treeId);

            return tree is not null && (tree.OwnerId == accountId || tree.Modifiers.Any(m => m.Id == accountId));
        }

        public Form GetTreeForm(int treeId, string currentUserId)
        {
            if (!int.TryParse(currentUserId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentUserId));
            }

            var tree = treeId == default ? new GenealogicalTree() : treeService.GetTreeWithModifiers(treeId);

            var builder = new GenealogicalTreeFormBuilder(accountService, treeService, personService, accountId);

            return builder.BuildForm(tree);
        }

        public async Task<ServerResponse<GenealogicalTreeItemResponse>> SaveTreeFormAsync(TreeEditorInfo data, string currentUserId)
        {
            if (!int.TryParse(currentUserId, out int accountId) || accountId < 1)
            {
                throw new ArgumentException("Invalid user", nameof(currentUserId));
            }

            var tree = data.TreeId == default ? new GenealogicalTree() : treeService.GetTreeWithModifiersAndPersons(data.TreeId);

            var builder = new GenealogicalTreeFormBuilder(accountService, treeService, personService, accountId);
            await builder.SaveFormAsync(tree, data.Values, data.Picture);

            var timeUpdate = tree.UpdatedTime ?? tree.CreatedTime;
            var newTreeCard = new GenealogicalTreeItemResponse
            {
                Id = tree.Id,
                Name = tree.Name,
                PersonsCount = data.TreeId > 0 ? tree.Persons.Count : 1,
                IsOwned = true,
                LastUpdate = timeUpdate.ToString("MM/dd/yyyy h:mm tt"),
            };

            return new ServerResponse<GenealogicalTreeItemResponse>(ResultCode.Done, newTreeCard);
        }

        public Form GetDraftPersonForm()
        {
            var person = new Person();
            var builder = new PersonFormBuilder(personService, relationsService, photoService);

            return builder.BuildForm(person);
        }

        private async Task<TreeNodeResponse> CreateTreeNodeAsync(Person person)
        {
            var avatar = await photoService.GetPersonAvatarAsync(person.Id);
            var relations = relationsService.GetRelations(person.Id, person.GenealogicalTree?.Id);
            var node = new TreeNodeResponse
            {
                Id = person.Id,
                Name = person.GetTreeNodeName(),
                Gender = person.Gender.GetClientView(),
                Image = avatar?.Url,
            };

            var spouses = new List<int>();
            var children = new List<int>();

            foreach (var relation in relations)
            {
                switch (relation.RelationType)
                {
                    case Relation.ChildToParent when relation.FromPersonId == person.Id:
                        if (relation.ToPerson.Gender == Gender.Woman)
                            node.MotherId = relation.ToPersonId;
                        else
                            node.FatherId = relation.ToPersonId;
                        break;
                    case Relation.ChildToParent when relation.ToPersonId == person.Id:
                        children.Add(relation.FromPersonId);
                        break;
                    case Relation.Partners:
                        spouses.Add(relation.FromPersonId == person.Id ? relation.ToPersonId : relation.FromPersonId);
                        break;
                    default:
                        throw new GenesisApplicationException("Invalid genealogical tree relation");
                }
            }

            node.SpouseIds = spouses;
            node.ChildrenIds = children;

            return node;
        }

        private Row GeneratePersonRow(Person person, int currentUser)
        {
            var tableBuilder = new PersonsTableBuilder(treeService, currentUser);
            var columns = tableBuilder.GetColumns();

            return tableBuilder.GenerateRow(person, columns, person.Id);
        }
    }
}
