using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.App.Contract.Models.Forms;
using Genesis.App.Contract.Models.Tables;
using Genesis.Common.Enums;

namespace Genesis.App.Implementation.Tables
{
    public class PersonsTableBuilder : TableBuilder<Person>
    {
        private List<GenealogicalTree> userTrees;
        private int currentUserId;

        public PersonsTableBuilder(IGenealogicalTreeService treeService, int currentUserId)
        {
            userTrees = treeService.GetAllUserTrees(currentUserId).ToList();
            this.currentUserId = currentUserId;
        }
        public override List<Column> GetColumns()
        {
            return new List<Column>
            {
                new Column
                {
                    Id = 1,
                    Name = "Avatar",
                    Type = ColumnType.Image,
                    EntityType = EntityType.Pictures
                },
                new Column
                {
                    Id = 2,
                    Name = "Id",
                    Type = ColumnType.Int,
                    EntityType = EntityType.Id
                },
                new Column
                {
                    Id = 3,
                    Name = "First Name",
                    Type = ColumnType.Text,
                    EntityType = EntityType.FirstName
                },
                new Column
                {
                    Id = 4,
                    Name = "Middle Name",
                    Type = ColumnType.Text,
                    EntityType = EntityType.MiddleName
                },
                new Column
                {
                    Id = 5,
                    Name = "Last Name",
                    Type = ColumnType.Text,
                    EntityType = EntityType.LastName
                },
                new Column
                {
                    Id = 6,
                    Name = "Genealogical Tree",
                    Type = ColumnType.Text,
                    EntityType = EntityType.GenealogicalTree
                },
                new Column
                {
                    Id = 7,
                    Name = "Birth date",
                    Type = ColumnType.Text,
                    EntityType = EntityType.DateOfBirth
                },
                new Column
                {
                    Id = 8,
                    Name = "Death date",
                    Type = ColumnType.Text,
                    EntityType = EntityType.DateOfDeath
                },
                new Column
                {
                    Id = 9,
                    Name = "Gender",
                    Type = ColumnType.Text,
                    EntityType = EntityType.Gender
                },
                new Column
                {
                    Id = 10,
                    Name = "Birth Place",
                    Type = ColumnType.Text,
                    EntityType = EntityType.BirthPlace
                },
            };
        }

        protected override bool IsRemovableRow(Person model) => !model.HasLinkToAccount;

        protected override int GetRowId(Person person, int index) => person.Id;

        protected override Cell GetCell(Person person, Column column)
        {
            var cell = new Cell() { ColumnId = column.Id };

            switch (column.EntityType)
            {
                case EntityType.Id:
                    cell.Value = person.Id;
                    break;
                case EntityType.Pictures:
                    cell.Value = person.GetAvatarUrl();
                    break;
                case EntityType.FirstName:
                    cell.Value = person.FirstName;
                    break;
                case EntityType.MiddleName:
                    cell.Value = person.MiddleName;
                    break;
                case EntityType.LastName:
                    cell.Value = person.LastName;
                    break;
                case EntityType.DateOfBirth:
                    cell.Value = person.Biography?.BirthDate?.ToShortDateString();
                    break;
                case EntityType.DateOfDeath:
                    cell.Value = person.Biography?.DeathDate?.ToShortDateString();
                    break;
                case EntityType.BirthPlace:
                    cell.Value = person.Biography?.BirthPlace?.GetFullAddress();
                    break;
                case EntityType.Gender:
                    cell.Value = person.Gender.GetClientView();
                    break;
                case EntityType.GenealogicalTree:
                    if (person.GenealogicalTreeId is not null)
                        cell.Value = userTrees.First(t => t.Id == person.GenealogicalTreeId).Name;
                    break;
                default: break;
            }

            return cell;
        }
    }
}
