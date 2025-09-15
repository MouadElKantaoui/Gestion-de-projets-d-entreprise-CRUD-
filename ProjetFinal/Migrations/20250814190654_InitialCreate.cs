using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetFinal.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Voie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodePostal = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Entreprises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdresseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entreprises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entreprises_Adresses_AdresseId",
                        column: x => x.AdresseId,
                        principalTable: "Adresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Developpeurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Anciennete = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntrepriseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developpeurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Developpeurs_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progression = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntrepriseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projets_Entreprises_EntrepriseId",
                        column: x => x.EntrepriseId,
                        principalTable: "Entreprises",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeveloppeurProjet",
                columns: table => new
                {
                    DeveloppeursId = table.Column<int>(type: "int", nullable: false),
                    ProjetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeveloppeurProjet", x => new { x.DeveloppeursId, x.ProjetsId });
                    table.ForeignKey(
                        name: "FK_DeveloppeurProjet_Developpeurs_DeveloppeursId",
                        column: x => x.DeveloppeursId,
                        principalTable: "Developpeurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeveloppeurProjet_Projets_ProjetsId",
                        column: x => x.ProjetsId,
                        principalTable: "Projets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeveloppeurProjet_ProjetsId",
                table: "DeveloppeurProjet",
                column: "ProjetsId");

            migrationBuilder.CreateIndex(
                name: "IX_Developpeurs_EntrepriseId",
                table: "Developpeurs",
                column: "EntrepriseId");

            migrationBuilder.CreateIndex(
                name: "IX_Entreprises_AdresseId",
                table: "Entreprises",
                column: "AdresseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projets_EntrepriseId",
                table: "Projets",
                column: "EntrepriseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeveloppeurProjet");

            migrationBuilder.DropTable(
                name: "Developpeurs");

            migrationBuilder.DropTable(
                name: "Projets");

            migrationBuilder.DropTable(
                name: "Entreprises");

            migrationBuilder.DropTable(
                name: "Adresses");
        }
    }
}
