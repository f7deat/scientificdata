using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Topic_TopicId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Author_Department_DepartmentId",
                table: "Author");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorTopic_Author_AuthorId",
                table: "AuthorTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorTopic_Topic_TopicId",
                table: "AuthorTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTopic_Tag_TagId",
                table: "TagTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTopic_Topic_TopicId",
                table: "TagTopic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Categories_CategoryId",
                table: "Topic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topic",
                table: "Topic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagTopic",
                table: "TagTopic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tag",
                table: "Tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Department",
                table: "Department");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorTopic",
                table: "AuthorTopic");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Author",
                table: "Author");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment");

            migrationBuilder.RenameTable(
                name: "Topic",
                newName: "Topics");

            migrationBuilder.RenameTable(
                name: "TagTopic",
                newName: "TagTopics");

            migrationBuilder.RenameTable(
                name: "Tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "Department",
                newName: "Departments");

            migrationBuilder.RenameTable(
                name: "AuthorTopic",
                newName: "AuthorTopics");

            migrationBuilder.RenameTable(
                name: "Author",
                newName: "Authors");

            migrationBuilder.RenameTable(
                name: "Attachment",
                newName: "Attachments");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_CategoryId",
                table: "Topics",
                newName: "IX_Topics_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TagTopic_TopicId",
                table: "TagTopics",
                newName: "IX_TagTopics_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorTopic_TopicId",
                table: "AuthorTopics",
                newName: "IX_AuthorTopics_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Author_DepartmentId",
                table: "Authors",
                newName: "IX_Authors_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_TopicId",
                table: "Attachments",
                newName: "IX_Attachments_TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topics",
                table: "Topics",
                column: "TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagTopics",
                table: "TagTopics",
                columns: new[] { "TagId", "TopicId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                table: "Departments",
                column: "DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorTopics",
                table: "AuthorTopics",
                columns: new[] { "AuthorId", "TopicId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Authors",
                table: "Authors",
                column: "AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Topics_TopicId",
                table: "Attachments",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Departments_DepartmentId",
                table: "Authors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorTopics_Authors_AuthorId",
                table: "AuthorTopics",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorTopics_Topics_TopicId",
                table: "AuthorTopics",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTopics_Tags_TagId",
                table: "TagTopics",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTopics_Topics_TopicId",
                table: "TagTopics",
                column: "TopicId",
                principalTable: "Topics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Categories_CategoryId",
                table: "Topics",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Topics_TopicId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Departments_DepartmentId",
                table: "Authors");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorTopics_Authors_AuthorId",
                table: "AuthorTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorTopics_Topics_TopicId",
                table: "AuthorTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTopics_Tags_TagId",
                table: "TagTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_TagTopics_Topics_TopicId",
                table: "TagTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Categories_CategoryId",
                table: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Topics",
                table: "Topics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagTopics",
                table: "TagTopics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                table: "Departments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthorTopics",
                table: "AuthorTopics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Authors",
                table: "Authors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attachments",
                table: "Attachments");

            migrationBuilder.RenameTable(
                name: "Topics",
                newName: "Topic");

            migrationBuilder.RenameTable(
                name: "TagTopics",
                newName: "TagTopic");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "Tag");

            migrationBuilder.RenameTable(
                name: "Departments",
                newName: "Department");

            migrationBuilder.RenameTable(
                name: "AuthorTopics",
                newName: "AuthorTopic");

            migrationBuilder.RenameTable(
                name: "Authors",
                newName: "Author");

            migrationBuilder.RenameTable(
                name: "Attachments",
                newName: "Attachment");

            migrationBuilder.RenameIndex(
                name: "IX_Topics_CategoryId",
                table: "Topic",
                newName: "IX_Topic_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_TagTopics_TopicId",
                table: "TagTopic",
                newName: "IX_TagTopic_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthorTopics_TopicId",
                table: "AuthorTopic",
                newName: "IX_AuthorTopic_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_DepartmentId",
                table: "Author",
                newName: "IX_Author_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attachments_TopicId",
                table: "Attachment",
                newName: "IX_Attachment_TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Topic",
                table: "Topic",
                column: "TopicId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagTopic",
                table: "TagTopic",
                columns: new[] { "TagId", "TopicId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tag",
                table: "Tag",
                column: "TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Department",
                table: "Department",
                column: "DepartmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthorTopic",
                table: "AuthorTopic",
                columns: new[] { "AuthorId", "TopicId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Author",
                table: "Author",
                column: "AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attachment",
                table: "Attachment",
                column: "AttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Topic_TopicId",
                table: "Attachment",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Author_Department_DepartmentId",
                table: "Author",
                column: "DepartmentId",
                principalTable: "Department",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorTopic_Author_AuthorId",
                table: "AuthorTopic",
                column: "AuthorId",
                principalTable: "Author",
                principalColumn: "AuthorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorTopic_Topic_TopicId",
                table: "AuthorTopic",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTopic_Tag_TagId",
                table: "TagTopic",
                column: "TagId",
                principalTable: "Tag",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagTopic_Topic_TopicId",
                table: "TagTopic",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Categories_CategoryId",
                table: "Topic",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
