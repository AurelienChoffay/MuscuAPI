using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MuscuAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DifficultesExercices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Niveau = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifficultesExercices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupesMusculaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupesMusculaires", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesEquipements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesEquipements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Muscles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NomLatin = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GroupeMusculaireId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fonction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Muscles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Muscles_GroupesMusculaires_GroupeMusculaireId",
                        column: x => x.GroupeMusculaireId,
                        principalTable: "GroupesMusculaires",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exercices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MusclePrincipalId = table.Column<int>(type: "int", nullable: false),
                    DifficulteId = table.Column<int>(type: "int", nullable: false),
                    TypeEquipementId = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Conseils = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    MuscleId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercices_DifficultesExercices_DifficulteId",
                        column: x => x.DifficulteId,
                        principalTable: "DifficultesExercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exercices_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exercices_Muscles_MusclePrincipalId",
                        column: x => x.MusclePrincipalId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exercices_TypesEquipements_TypeEquipementId",
                        column: x => x.TypeEquipementId,
                        principalTable: "TypesEquipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExercicesImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExerciceId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    IsPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicesImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExercicesImages_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExercicesMusclesSecondaires",
                columns: table => new
                {
                    ExerciceId = table.Column<int>(type: "int", nullable: false),
                    MuscleId = table.Column<int>(type: "int", nullable: false),
                    OrdreImportance = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicesMusclesSecondaires", x => new { x.ExerciceId, x.MuscleId });
                    table.ForeignKey(
                        name: "FK_ExercicesMusclesSecondaires_Exercices_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "Exercices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercicesMusclesSecondaires_Muscles_MuscleId",
                        column: x => x.MuscleId,
                        principalTable: "Muscles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DifficultesExercices",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Niveau", "Nom", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pour les personnes qui commencent la musculation", true, 1, "Débutant", null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nécessite quelques mois de pratique", true, 2, "Intermédiaire", null },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pour les pratiquants expérimentés", true, 3, "Avancé", null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mouvements complexes nécessitant une excellente maîtrise", true, 4, "Expert", null }
                });

            migrationBuilder.InsertData(
                table: "TypesEquipements",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "IsActive", "Nom", "Ordre", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Exercices au poids du corps", null, true, "Aucun équipement", 1, null },
                    { 2, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Poids libres modulables", null, true, "Haltères", 2, null },
                    { 3, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Barre olympique ou EZ", null, true, "Barre", 3, null },
                    { 4, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Machines guidées", null, true, "Machine", 4, null },
                    { 5, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Câbles et poulies", null, true, "Poulie", 5, null },
                    { 6, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bandes de résistance", null, true, "Élastique/Bande", 6, null },
                    { 7, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Poids avec poignée", null, true, "Kettlebell", 7, null },
                    { 8, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pour tractions et suspensions", null, true, "Barre de traction", 8, null },
                    { 9, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Banc de musculation", null, true, "Banc", 9, null },
                    { 10, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ballon de gym", null, true, "Swiss Ball", 10, null },
                    { 11, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sangles de suspension", null, true, "TRX", 11, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DifficultesExercices_Niveau",
                table: "DifficultesExercices",
                column: "Niveau");

            migrationBuilder.CreateIndex(
                name: "IX_DifficultesExercices_Nom",
                table: "DifficultesExercices",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_DifficulteId",
                table: "Exercices",
                column: "DifficulteId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_MuscleId",
                table: "Exercices",
                column: "MuscleId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_MusclePrincipalId",
                table: "Exercices",
                column: "MusclePrincipalId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_Nom",
                table: "Exercices",
                column: "Nom");

            migrationBuilder.CreateIndex(
                name: "IX_Exercices_TypeEquipementId",
                table: "Exercices",
                column: "TypeEquipementId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicesImages_ExerciceId_Ordre",
                table: "ExercicesImages",
                columns: new[] { "ExerciceId", "Ordre" });

            migrationBuilder.CreateIndex(
                name: "IX_ExercicesMusclesSecondaires_MuscleId",
                table: "ExercicesMusclesSecondaires",
                column: "MuscleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupesMusculaires_Nom",
                table: "GroupesMusculaires",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_GroupeMusculaireId",
                table: "Muscles",
                column: "GroupeMusculaireId");

            migrationBuilder.CreateIndex(
                name: "IX_Muscles_Nom",
                table: "Muscles",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesEquipements_Nom",
                table: "TypesEquipements",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesEquipements_Ordre",
                table: "TypesEquipements",
                column: "Ordre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExercicesImages");

            migrationBuilder.DropTable(
                name: "ExercicesMusclesSecondaires");

            migrationBuilder.DropTable(
                name: "Exercices");

            migrationBuilder.DropTable(
                name: "DifficultesExercices");

            migrationBuilder.DropTable(
                name: "Muscles");

            migrationBuilder.DropTable(
                name: "TypesEquipements");

            migrationBuilder.DropTable(
                name: "GroupesMusculaires");
        }
    }
}
