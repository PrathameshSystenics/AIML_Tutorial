using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProductClassification.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationBatch",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationBatch", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EvaluationData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "Text", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationData", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PromptData",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvaluationBatchID = table.Column<int>(type: "integer", nullable: false),
                    SystemPrompt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromptData", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PromptData_EvaluationBatch_EvaluationBatchID",
                        column: x => x.EvaluationBatchID,
                        principalTable: "EvaluationBatch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvaluatedResult",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EvaluationBatchID = table.Column<int>(type: "integer", nullable: false),
                    EvaluationDataID = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluatedResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_EvaluatedResult_EvaluationBatch_EvaluationBatchID",
                        column: x => x.EvaluationBatchID,
                        principalTable: "EvaluationBatch",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EvaluatedResult_EvaluationData_EvaluationDataID",
                        column: x => x.EvaluationDataID,
                        principalTable: "EvaluationData",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluatedResult_EvaluationBatchID",
                table: "EvaluatedResult",
                column: "EvaluationBatchID");

            migrationBuilder.CreateIndex(
                name: "IX_EvaluatedResult_EvaluationDataID",
                table: "EvaluatedResult",
                column: "EvaluationDataID");

            migrationBuilder.CreateIndex(
                name: "IX_PromptData_EvaluationBatchID",
                table: "PromptData",
                column: "EvaluationBatchID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluatedResult");

            migrationBuilder.DropTable(
                name: "PromptData");

            migrationBuilder.DropTable(
                name: "EvaluationData");

            migrationBuilder.DropTable(
                name: "EvaluationBatch");
        }
    }
}
