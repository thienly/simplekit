using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderWorker.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SagaState",
                columns: table => new
                {
                    SagaId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    IsCompleted = table.Column<bool>(nullable: false),
                    CurrentState = table.Column<string>(nullable: false),
                    NextState = table.Column<string>(nullable: true),
                    SagaDefinitionType = table.Column<string>(nullable: true),
                    Direction = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SagaState", x => new { x.SagaId, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "SagaState");
        }
    }
}
