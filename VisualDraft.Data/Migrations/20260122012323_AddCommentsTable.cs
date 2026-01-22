using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisualDraft.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "Pins");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Pins");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PinId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Pins_PinId",
                        column: x => x.PinId,
                        principalTable: "Pins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PinId",
                table: "Comments",
                column: "PinId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "Pins",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Pins",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
