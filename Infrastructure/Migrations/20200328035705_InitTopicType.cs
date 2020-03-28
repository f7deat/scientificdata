using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class InitTopicType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseType",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "AttachmentType",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "TopicType",
                table: "Topics");

            migrationBuilder.AddColumn<int>(
                name: "TopicTypeId",
                table: "Warehouses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicTypeId",
                table: "Topics",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    AttachmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Path = table.Column<string>(maxLength: 200, nullable: true),
                    TopicId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.AttachmentId);
                    table.ForeignKey(
                        name: "FK_Attachments_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicTypes",
                columns: table => new
                {
                    TopicTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<bool>(nullable: true),
                    TopicTypeId1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicTypes", x => x.TopicTypeId);
                    table.ForeignKey(
                        name: "FK_TopicTypes_TopicTypes_TopicTypeId1",
                        column: x => x.TopicTypeId1,
                        principalTable: "TopicTypes",
                        principalColumn: "TopicTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouses_TopicTypeId",
                table: "Warehouses",
                column: "TopicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_TopicTypeId",
                table: "Topics",
                column: "TopicTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_TopicId",
                table: "Attachments",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicTypes_TopicTypeId1",
                table: "TopicTypes",
                column: "TopicTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_TopicTypes_TopicTypeId",
                table: "Topics",
                column: "TopicTypeId",
                principalTable: "TopicTypes",
                principalColumn: "TopicTypeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Warehouses_TopicTypes_TopicTypeId",
                table: "Warehouses",
                column: "TopicTypeId",
                principalTable: "TopicTypes",
                principalColumn: "TopicTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topics_TopicTypes_TopicTypeId",
                table: "Topics");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouses_TopicTypes_TopicTypeId",
                table: "Warehouses");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "TopicTypes");

            migrationBuilder.DropIndex(
                name: "IX_Warehouses_TopicTypeId",
                table: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_Topics_TopicTypeId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "TopicTypeId",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "TopicTypeId",
                table: "Topics");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseType",
                table: "Warehouses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentType",
                table: "Topics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "Topics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TopicType",
                table: "Topics",
                type: "int",
                nullable: true);
        }
    }
}
