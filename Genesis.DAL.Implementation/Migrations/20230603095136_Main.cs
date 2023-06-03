using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Genesis.DAL.Implementation.Migrations
{
    public partial class Main : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcceptTerms = table.Column<bool>(type: "bit", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResetTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SettlementCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "account_connections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountFromId = table.Column<int>(type: "int", nullable: false),
                    AccountToId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_account_connections_accounts_AccountFromId",
                        column: x => x.AccountFromId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_connections_accounts_AccountToId",
                        column: x => x.AccountToId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "genealogical_trees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_genealogical_trees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_genealogical_trees_accounts_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonRevoked = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_tokens_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlaceId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notations_locations_PlaceId",
                        column: x => x.PlaceId,
                        principalTable: "locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "persons_roles",
                columns: table => new
                {
                    AccountsId = table.Column<int>(type: "int", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons_roles", x => new { x.AccountsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_persons_roles_accounts_AccountsId",
                        column: x => x.AccountsId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persons_roles_roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Gender = table.Column<byte>(type: "tinyint", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    GenealogicalTreeId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_persons_accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persons_genealogical_trees_GenealogicalTreeId",
                        column: x => x.GenealogicalTreeId,
                        principalTable: "genealogical_trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "trees_modifiers",
                columns: table => new
                {
                    AvailableTreesId = table.Column<int>(type: "int", nullable: false),
                    ModifiersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trees_modifiers", x => new { x.AvailableTreesId, x.ModifiersId });
                    table.ForeignKey(
                        name: "FK_trees_modifiers_accounts_ModifiersId",
                        column: x => x.ModifiersId,
                        principalTable: "accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trees_modifiers_genealogical_trees_AvailableTreesId",
                        column: x => x.AvailableTreesId,
                        principalTable: "genealogical_trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoricalNotationId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_documents_notations_HistoricalNotationId",
                        column: x => x.HistoricalNotationId,
                        principalTable: "notations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "biographies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Info = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeathDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthPlaceId = table.Column<int>(type: "int", nullable: true),
                    DeathPlaceId = table.Column<int>(type: "int", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByAccount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_biographies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_biographies_locations_BirthPlaceId",
                        column: x => x.BirthPlaceId,
                        principalTable: "locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_biographies_locations_DeathPlaceId",
                        column: x => x.DeathPlaceId,
                        principalTable: "locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_biographies_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "pictures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMain = table.Column<bool>(type: "bit", nullable: false),
                    HistoricalNotationId = table.Column<int>(type: "int", nullable: true),
                    GenealogicalTreeId = table.Column<int>(type: "int", nullable: true),
                    PersonId = table.Column<int>(type: "int", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_pictures_genealogical_trees_GenealogicalTreeId",
                        column: x => x.GenealogicalTreeId,
                        principalTable: "genealogical_trees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_pictures_notations_HistoricalNotationId",
                        column: x => x.HistoricalNotationId,
                        principalTable: "notations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_pictures_persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "relations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromPersonId = table.Column<int>(type: "int", nullable: false),
                    ToPersonId = table.Column<int>(type: "int", nullable: false),
                    GenealogicalTreeId = table.Column<int>(type: "int", nullable: false),
                    RelationType = table.Column<int>(type: "int", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_relations_genealogical_trees_GenealogicalTreeId",
                        column: x => x.GenealogicalTreeId,
                        principalTable: "genealogical_trees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_relations_persons_FromPersonId",
                        column: x => x.FromPersonId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_relations_persons_ToPersonId",
                        column: x => x.ToPersonId,
                        principalTable: "persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "persons_documents",
                columns: table => new
                {
                    PersonsId = table.Column<int>(type: "int", nullable: false),
                    RelatedDocumentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons_documents", x => new { x.PersonsId, x.RelatedDocumentsId });
                    table.ForeignKey(
                        name: "FK_persons_documents_documents_RelatedDocumentsId",
                        column: x => x.RelatedDocumentsId,
                        principalTable: "documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_persons_documents_persons_PersonsId",
                        column: x => x.PersonsId,
                        principalTable: "persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "events_biographies",
                columns: table => new
                {
                    BiographiesId = table.Column<int>(type: "int", nullable: false),
                    EventsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events_biographies", x => new { x.BiographiesId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_events_biographies_biographies_BiographiesId",
                        column: x => x.BiographiesId,
                        principalTable: "biographies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_events_biographies_notations_EventsId",
                        column: x => x.EventsId,
                        principalTable: "notations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[] { 1, "User" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[] { 2, "Admin" });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "Id", "RoleName" },
                values: new object[] { 3, "ArchiveWorker" });

            migrationBuilder.CreateIndex(
                name: "IX_account_connections_AccountFromId",
                table: "account_connections",
                column: "AccountFromId");

            migrationBuilder.CreateIndex(
                name: "IX_account_connections_AccountToId",
                table: "account_connections",
                column: "AccountToId");

            migrationBuilder.CreateIndex(
                name: "IX_biographies_BirthPlaceId",
                table: "biographies",
                column: "BirthPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_biographies_DeathPlaceId",
                table: "biographies",
                column: "DeathPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_biographies_PersonId",
                table: "biographies",
                column: "PersonId",
                unique: true,
                filter: "[PersonId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_documents_HistoricalNotationId",
                table: "documents",
                column: "HistoricalNotationId");

            migrationBuilder.CreateIndex(
                name: "IX_events_biographies_EventsId",
                table: "events_biographies",
                column: "EventsId");

            migrationBuilder.CreateIndex(
                name: "IX_genealogical_trees_OwnerId",
                table: "genealogical_trees",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_notations_PlaceId",
                table: "notations",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_AccountId",
                table: "persons",
                column: "AccountId",
                unique: true,
                filter: "[AccountId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_persons_GenealogicalTreeId",
                table: "persons",
                column: "GenealogicalTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_documents_RelatedDocumentsId",
                table: "persons_documents",
                column: "RelatedDocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_persons_roles_RolesId",
                table: "persons_roles",
                column: "RolesId");

            migrationBuilder.CreateIndex(
                name: "IX_pictures_GenealogicalTreeId",
                table: "pictures",
                column: "GenealogicalTreeId",
                unique: true,
                filter: "[GenealogicalTreeId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_pictures_HistoricalNotationId",
                table: "pictures",
                column: "HistoricalNotationId");

            migrationBuilder.CreateIndex(
                name: "IX_pictures_PersonId",
                table: "pictures",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_AccountId",
                table: "refresh_tokens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_relations_FromPersonId",
                table: "relations",
                column: "FromPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_relations_GenealogicalTreeId",
                table: "relations",
                column: "GenealogicalTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_relations_ToPersonId",
                table: "relations",
                column: "ToPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_trees_modifiers_ModifiersId",
                table: "trees_modifiers",
                column: "ModifiersId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_connections");

            migrationBuilder.DropTable(
                name: "events_biographies");

            migrationBuilder.DropTable(
                name: "persons_documents");

            migrationBuilder.DropTable(
                name: "persons_roles");

            migrationBuilder.DropTable(
                name: "pictures");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "relations");

            migrationBuilder.DropTable(
                name: "trees_modifiers");

            migrationBuilder.DropTable(
                name: "biographies");

            migrationBuilder.DropTable(
                name: "documents");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropTable(
                name: "notations");

            migrationBuilder.DropTable(
                name: "genealogical_trees");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "accounts");
        }
    }
}
