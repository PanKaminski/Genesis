using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Responses;
using Genesis.App.Contract.Models.Tables;
using Genesis.App.Implementation.Tables;
using Genesis.Common.Exceptions;

namespace Genesis.App.Implementation.DataManager
{
    public class DataManagerToolService
    {
        private IGenealogicalTreeService treeService;
        private IPersonService personService;

        public DataManagerToolService(IGenealogicalTreeService treeService, IPersonService personService)
        {
            this.treeService = treeService;
            this.personService = personService;
        }

        public Table GetPersonsTable(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentException("User identifier is null or empty.");

            if (!int.TryParse(currentUserId, out var userIdValue))
                throw new GenesisApplicationException("User id is not number");

            var tableBuilder = new PersonsTableBuilder(treeService, userIdValue);
            var persons = personService.GetPersonsCreatedByUser(userIdValue).ToList();

            return tableBuilder.GenerateTable(persons);
        }

        public async Task<ServerResponse<Row>> CreateDraftTreeAsync(int personId, string currentUserId)
        {
            try
            {
                if (string.IsNullOrEmpty(currentUserId))
                    throw new ArgumentException("User identifier is null or empty.");

                if (!int.TryParse(currentUserId, out var userIdValue))
                    throw new GenesisApplicationException("User id is not number");

                var lastTreeId = await treeService.GetLastCreatedTreeIdAsync(userIdValue);
                var name = $"Tree #{lastTreeId++}";
                var tree = new GenealogicalTree
                {
                    OwnerId = userIdValue,
                    Name = name,
                    Persons = new List<Person> { new Person { Id = personId } },
                };

                await treeService.CreateDraftTreeAsync(tree);

                return new ServerResponse<Row>(ResultCode.Done, 
                    CreatePersonRow(personId, userIdValue), "Tree successfully created");
            }
            catch (Exception)
            {
                return new ServerResponse<Row>(ResultCode.Failed, null, "Failed to create draft tree");
            }
        }

        private Row CreatePersonRow(int personId, int currentUserId)
        {
            var person = personService.GetPerson(personId);
            var tableBuilder = new PersonsTableBuilder(treeService, currentUserId);

            var columns = tableBuilder.GetColumns();
            return tableBuilder.GenerateRow(person, columns, personId);
        }
    }
}
