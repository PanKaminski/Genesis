﻿using Genesis.App.Contract.Dashboard.ApiModels;
using Genesis.App.Contract.Models;

namespace Genesis.App.Contract.Dashboard.Services
{
    public interface IPersonService
    {
        IAsyncEnumerable<TreeNodeResponse> GetTreePersonsAsync(int treeId, string currentUserId);

        IEnumerable<Person> GetPersonsCreatedByUser(int ownerId);

        IEnumerable<Person> GetPersonsWithoutTree(int ownerId);

        Person GetPerson(int id);

        Person GetRootPerson(int accountId);

        Person GetPersonWithGenealogicalTree(int id);

        Person GetPersonWithFullInfo(int id);

        Task<int> AddPersonAsync(Person person, int? treeId, bool saveChanges = false);

        Task EditPersonAsync(Person person, bool saveChanges = false);

        void RemovePerson(int personId, bool saveChanges);

        void RemovePersons(IEnumerable<int> personIds, bool saveChanges);
    }
}
