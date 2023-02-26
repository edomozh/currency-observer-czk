using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLibrary.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCurrencyMultiplierType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Rates_CurrencyId_Date",
                table: "Rates");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Currencies_Code_Multiplier",
                table: "Currencies");

            migrationBuilder.AlterColumn<int>(
                name: "Multiplier",
                table: "Currencies",
                type: "int",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_CurrencyId_Date",
                table: "Rates",
                columns: new[] { "CurrencyId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code_Multiplier",
                table: "Currencies",
                columns: new[] { "Code", "Multiplier" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rates_CurrencyId_Date",
                table: "Rates");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Code_Multiplier",
                table: "Currencies");

            migrationBuilder.AlterColumn<short>(
                name: "Multiplier",
                table: "Currencies",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Rates_CurrencyId_Date",
                table: "Rates",
                columns: new[] { "CurrencyId", "Date" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Currencies_Code_Multiplier",
                table: "Currencies",
                columns: new[] { "Code", "Multiplier" });
        }
    }
}
