using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuscuAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GroupeMusculaireCorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GroupesMusculaires",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GroupesMusculaires",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GroupesMusculaires");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "GroupesMusculaires");
        }
    }
}
