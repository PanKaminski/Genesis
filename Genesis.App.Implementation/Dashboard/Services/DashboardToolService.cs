using Genesis.App.Contract.Authentication.Services;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Forms;
using Genesis.App.Contract.Models.Responses;
using Genesis.App.Implementation.Forms;
using Genesis.Common.Enums;
using Genesis.Common.Exceptions;

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

        public Form GetPersonForm(PersonFormParams formParams)
        {
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

        public async Task<ServerResponse<TreeNodeResponse>> SavePersonFormAsync(PersonEditModel editModel, string currentAccountId)
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

                return new ServerResponse<TreeNodeResponse>(ResultCode.Done, 
                    await CreateTreeNodeAsync(person), "Person succefully saved");
            }
            catch(GenesisApplicationException ex)
            {
                return new ServerResponse<TreeNodeResponse>(ResultCode.Failed, null, ex.Message);
            }
            catch (Exception ex)
            {
                return new ServerResponse<TreeNodeResponse>(ResultCode.Failed, null, "Person save failed");
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

            var builder = new GenealogicalTreeFormBuilder(accountService, treeService, accountId);

            return builder.BuildForm(tree);
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
    }
}
