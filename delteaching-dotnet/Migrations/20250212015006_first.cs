using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace delteaching_dotnet.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Branch = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    Type = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    HolderName = table.Column<string>(type: "VARCHAR(150)", maxLength: 50, nullable: false),
                    HolderEmail = table.Column<string>(type: "VARCHAR(300)", maxLength: 50, nullable: false),
                    HolderDocument = table.Column<string>(type: "VARCHAR(14)", maxLength: 50, nullable: false),
                    HolderType = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GEUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccounts");
        }
    }
}
