using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PainterPalApi.Migrations
{
    /// <inheritdoc />
    public partial class TablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_ProductPreferences_PreferredProductId",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotes_Jobs_JobId",
                table: "Quotes");

            migrationBuilder.DropTable(
                name: "ProductPreferences");

            migrationBuilder.DropIndex(
                name: "IX_Quotes_JobId",
                table: "Quotes");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_PreferredProductId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ColourId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "ProductPreferenceId",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "QuantityInStock",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "PreferredProductId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ProductPreferenceId",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "QuoteDescription",
                table: "Quotes",
                newName: "QuoteNotes");

            migrationBuilder.RenameColumn(
                name: "JobDescription",
                table: "Jobs",
                newName: "JobNotes");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string[]>(
                name: "Tags",
                table: "Quotes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "Tasks",
                table: "Quotes",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "Tags",
                table: "Jobs",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string[]>(
                name: "Tasks",
                table: "Jobs",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.AddColumn<string>(
                name: "ColourUsage",
                table: "Colours",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "JobId",
                table: "Colours",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuoteId",
                table: "Colours",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserMaterialPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    IsFavourite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMaterialPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMaterialPreferences_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMaterialPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferredColours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ColourId = table.Column<int>(type: "integer", nullable: false),
                    IsFavourite = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferredColours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferredColours_Colours_ColourId",
                        column: x => x.ColourId,
                        principalTable: "Colours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferredColours_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Colours_JobId",
                table: "Colours",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Colours_QuoteId",
                table: "Colours",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaterialPreferences_MaterialId",
                table: "UserMaterialPreferences",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaterialPreferences_UserId",
                table: "UserMaterialPreferences",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferredColours_ColourId",
                table: "UserPreferredColours",
                column: "ColourId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferredColours_UserId",
                table: "UserPreferredColours",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colours_Jobs_JobId",
                table: "Colours",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Colours_Quotes_QuoteId",
                table: "Colours",
                column: "QuoteId",
                principalTable: "Quotes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colours_Jobs_JobId",
                table: "Colours");

            migrationBuilder.DropForeignKey(
                name: "FK_Colours_Quotes_QuoteId",
                table: "Colours");

            migrationBuilder.DropTable(
                name: "UserMaterialPreferences");

            migrationBuilder.DropTable(
                name: "UserPreferredColours");

            migrationBuilder.DropIndex(
                name: "IX_Colours_JobId",
                table: "Colours");

            migrationBuilder.DropIndex(
                name: "IX_Colours_QuoteId",
                table: "Colours");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Tasks",
                table: "Quotes");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Tasks",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ColourUsage",
                table: "Colours");

            migrationBuilder.DropColumn(
                name: "JobId",
                table: "Colours");

            migrationBuilder.DropColumn(
                name: "QuoteId",
                table: "Colours");

            migrationBuilder.RenameColumn(
                name: "QuoteNotes",
                table: "Quotes",
                newName: "QuoteDescription");

            migrationBuilder.RenameColumn(
                name: "JobNotes",
                table: "Jobs",
                newName: "JobDescription");

            migrationBuilder.AddColumn<int>(
                name: "ColourId",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductPreferenceId",
                table: "Quotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Materials",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "QuantityInStock",
                table: "Materials",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreferredProductId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductPreferenceId",
                table: "Jobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductPreferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ColourId = table.Column<int>(type: "integer", nullable: false),
                    PreferredColourId = table.Column<int>(type: "integer", nullable: false),
                    ProductDescription = table.Column<string>(type: "text", nullable: false),
                    ProductFinish = table.Column<string>(type: "text", nullable: false),
                    ProductName = table.Column<string>(type: "text", nullable: false),
                    ProductPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductSize = table.Column<string>(type: "text", nullable: false),
                    ProductType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPreferences_Colours_ColourId",
                        column: x => x.ColourId,
                        principalTable: "Colours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_JobId",
                table: "Quotes",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PreferredProductId",
                table: "Jobs",
                column: "PreferredProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPreferences_ColourId",
                table: "ProductPreferences",
                column: "ColourId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_ProductPreferences_PreferredProductId",
                table: "Jobs",
                column: "PreferredProductId",
                principalTable: "ProductPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotes_Jobs_JobId",
                table: "Quotes",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
