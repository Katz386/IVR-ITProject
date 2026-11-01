using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    TasksSolved = table.Column<string>(type: "text", nullable: false),
                    ArticlesRead = table.Column<string>(type: "text", nullable: false),
                    ArticlesReadCount = table.Column<int>(type: "integer", nullable: false),
                    EasyTasksSolvedCount = table.Column<int>(type: "integer", nullable: false),
                    MediumTasksSolvedCount = table.Column<int>(type: "integer", nullable: false),
                    HardTasksSolvedCount = table.Column<int>(type: "integer", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastTaskSolvedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastTaskSolvedId = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdminComment = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");
        }
    }
}
