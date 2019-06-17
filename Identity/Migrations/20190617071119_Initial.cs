using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Everest.Identity.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<string>(nullable: true),
                    NationalId = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ImageName = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    APIURL = table.Column<string>(nullable: true),
                    WebURL = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ResetPasswordCode = table.Column<string>(nullable: true),
                    ResetPasswordCodeCreateTime = table.Column<DateTime>(nullable: false),
                    AboutMe = table.Column<string>(nullable: true),
                    Website = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RedirectURL = table.Column<string>(nullable: true),
                    SecretCode = table.Column<string>(nullable: true),
                    ImageName = table.Column<string>(nullable: true),
                    ImageURL = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    Navigator = table.Column<string>(nullable: true),
                    RemoteAddress = table.Column<string>(nullable: true),
                    OS = table.Column<string>(nullable: true),
                    IsPersistent = table.Column<bool>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    AccountId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Connections_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Authorizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    ConnectionId1 = table.Column<long>(nullable: true),
                    ConnectionId = table.Column<string>(nullable: true),
                    AccessToken = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authorizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authorizations_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Authorizations_Connections_ConnectionId1",
                        column: x => x.ConnectionId1,
                        principalTable: "Connections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_ClientId",
                table: "Authorizations",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Authorizations_ConnectionId1",
                table: "Authorizations",
                column: "ConnectionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Connections_AccountId",
                table: "Connections",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Authorizations");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Connections");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
