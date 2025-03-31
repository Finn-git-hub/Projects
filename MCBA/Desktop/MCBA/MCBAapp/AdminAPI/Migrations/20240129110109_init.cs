using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminAPI.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    City = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Postcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    TFN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    State = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
