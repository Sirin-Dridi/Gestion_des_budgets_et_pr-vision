using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionBudgets.Migrations
{
    /// <inheritdoc />
    public partial class Initiale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    IdBudget = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Libelle = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateFin = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budgets", x => x.IdBudget);
                });

            migrationBuilder.CreateTable(
                name: "LignesBudgetaires",
                columns: table => new
                {
                    IdLigne = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Categorie = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    MontantPrevu = table.Column<decimal>(type: "TEXT", nullable: false),
                    BudgetId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LignesBudgetaires", x => x.IdLigne);
                    table.ForeignKey(
                        name: "FK_LignesBudgetaires_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "IdBudget",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LignesBudgetaires_BudgetId",
                table: "LignesBudgetaires",
                column: "BudgetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LignesBudgetaires");

            migrationBuilder.DropTable(
                name: "Budgets");
        }
    }
}
