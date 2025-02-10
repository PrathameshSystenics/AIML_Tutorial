using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductClassification.Migrations
{
    /// <inheritdoc />
    public partial class latest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromptData_EvaluationBatchID",
                table: "PromptData");

            migrationBuilder.CreateIndex(
                name: "IX_PromptData_EvaluationBatchID",
                table: "PromptData",
                column: "EvaluationBatchID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PromptData_EvaluationBatchID",
                table: "PromptData");

            migrationBuilder.CreateIndex(
                name: "IX_PromptData_EvaluationBatchID",
                table: "PromptData",
                column: "EvaluationBatchID");
        }
    }
}
