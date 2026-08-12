using Microsoft.EntityFrameworkCore.Migrations;


#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class indexoptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_server_state_last_query",
                table: "server_state");

            // Scaffolded AlterColumn on match_info removed by hand: snapshot drift from
            // 20260407000000_match-info, and it would have reset the column default to
            // '', which is not valid jsonb.

            migrationBuilder.CreateIndex(
                name: "IX_server_match_server_id_open",
                table: "server_match",
                column: "server_id",
                filter: "match_end IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_server_match_server_id_open",
                table: "server_match");

            // See note in Up().

            migrationBuilder.CreateIndex(
                name: "IX_server_state_last_query",
                table: "server_state",
                column: "last_query");
        }
    }
}
