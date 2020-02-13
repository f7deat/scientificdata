using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class WarehouseAnđepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                table: "Topics",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveDate",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ISSN",
                table: "Topics",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "Topics",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Page",
                table: "Topics",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Signer",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Topics",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicType",
                table: "Topics",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "Topics",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    WarehouseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Symbol = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.WarehouseId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_DepartmentId",
                table: "Topics",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_WarehouseId",
                table: "Topics",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Departments_DepartmentId",
                table: "Topics",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Warehouses_WarehouseId",
                table: "Topics",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Departments_DepartmentId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Warehouses_WarehouseId",
                table: "Topics");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Topics_DepartmentId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_WarehouseId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "EffectiveDate",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "ISSN",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Page",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Signer",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "TopicType",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "Topics");
        }
    }
}
