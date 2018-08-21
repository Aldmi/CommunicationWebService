using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.EFCore.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EfSerialPortOptions",
                columns: table => new
                {
                    Port = table.Column<string>(nullable: true),
                    BaudRate = table.Column<int>(nullable: false),
                    DataBits = table.Column<int>(nullable: false),
                    StopBits = table.Column<int>(nullable: false),
                    Parity = table.Column<int>(nullable: false),
                    DtrEnable = table.Column<bool>(nullable: false),
                    RtsEnable = table.Column<bool>(nullable: false),
                    AutoStart = table.Column<bool>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfSerialPortOptions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EfSerialPortOptions");
        }
    }
}
