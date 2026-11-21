using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionBudgets.Migrations
{
    /// <inheritdoc />
    public partial class SuiviBudgetaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ajustements",
                columns: table => new
                {
                    IdAjustement = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LigneId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateAjustement = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AncienMontant = table.Column<decimal>(type: "TEXT", nullable: false),
                    NouveauMontant = table.Column<decimal>(type: "TEXT", nullable: false),
                    Motif = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ajustements", x => x.IdAjustement);
                    table.ForeignKey(
                        name: "FK_Ajustements_LignesBudgetaires_LigneId",
                        column: x => x.LigneId,
                        principalTable: "LignesBudgetaires",
                        principalColumn: "IdLigne",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    IdTransaction = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Montant = table.Column<decimal>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    DateTransaction = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LigneId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.IdTransaction);
                    table.ForeignKey(
                        name: "FK_Transactions_LignesBudgetaires_LigneId",
                        column: x => x.LigneId,
                        principalTable: "LignesBudgetaires",
                        principalColumn: "IdLigne",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ajustements_LigneId",
                table: "Ajustements",
                column: "LigneId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LigneId",
                table: "Transactions",
                column: "LigneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ajustements");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
