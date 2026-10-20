using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeiFinanceiro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estabelecimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Atividade = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SalarioMinimoReferencia = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estabelecimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FechamentosMensais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EstabelecimentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Ano = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Faturamento = table.Column<decimal>(type: "TEXT", nullable: false),
                    Cmv = table.Column<decimal>(type: "TEXT", nullable: false),
                    CustosFixos = table.Column<decimal>(type: "TEXT", nullable: false),
                    DasCalculado = table.Column<decimal>(type: "TEXT", nullable: false),
                    LucroLiquido = table.Column<decimal>(type: "TEXT", nullable: false),
                    MargemLiquida = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FechamentosMensais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FechamentosMensais_Estabelecimentos_EstabelecimentoId",
                        column: x => x.EstabelecimentoId,
                        principalTable: "Estabelecimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FechamentosMensais_EstabelecimentoId_Ano_Mes",
                table: "FechamentosMensais",
                columns: new[] { "EstabelecimentoId", "Ano", "Mes" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FechamentosMensais");

            migrationBuilder.DropTable(
                name: "Estabelecimentos");
        }
    }
}
