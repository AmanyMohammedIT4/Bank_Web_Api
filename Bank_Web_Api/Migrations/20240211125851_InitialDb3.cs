using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bank_Web_Api.Migrations
{
    public partial class InitialDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionAccount",
                table: "Transactions",
                newName: "TransactionAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionAmount",
                table: "Transactions",
                newName: "TransactionAccount");
        }
    }
}
