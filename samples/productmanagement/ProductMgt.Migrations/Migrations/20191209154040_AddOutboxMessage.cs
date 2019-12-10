using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductMgt.Infrastructure.Migrations
{
    public partial class AddOutboxMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>("Version", table: "Product", rowVersion: true);

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "Product",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    Version = table.Column<byte[]>(rowVersion: true, nullable: true),
                    varchar500 = table.Column<string>(name: "varchar(500)", nullable: false),
                    Body = table.Column<string>(type: "varchar(500)", nullable: false),
                    DispatchedTime = table.Column<long>(nullable: false),
                    ProcessedTime = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropColumn("Version", "Product");

            migrationBuilder.AlterColumn<int>(
                name: "UpdatedBy",
                table: "Product",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
