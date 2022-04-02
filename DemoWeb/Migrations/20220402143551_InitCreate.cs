using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoWeb.Migrations
{
    public partial class InitCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StaticFileDatas",
                columns: table => new
                {
                    FileDataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    Content = table.Column<byte[]>(type: "BLOB", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ClientIp = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 1, nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFileDatas", x => x.FileDataId);
                });

            migrationBuilder.CreateTable(
                name: "StaticFileIndices",
                columns: table => new
                {
                    Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    FileDataId = table.Column<int>(type: "INTEGER", nullable: false),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaticFileIndices", x => x.Path);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaticFileDatas");

            migrationBuilder.DropTable(
                name: "StaticFileIndices");
        }
    }
}
