using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class indexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_server_state_last_query",
                table: "server_state",
                column: "last_query");

            migrationBuilder.CreateIndex(
                name: "IX_server_snapshot_timestamp",
                table: "server_snapshot",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_server_match_match_end_match_start",
                table: "server_match",
                columns: new[] { "match_end", "match_start" });

            migrationBuilder.CreateIndex(
                name: "IX_server_active",
                table: "server",
                column: "active");

            migrationBuilder.CreateIndex(
                name: "IX_server_address_port",
                table: "server",
                columns: new[] { "address", "port" });

            migrationBuilder.CreateIndex(
                name: "IX_server_api_key",
                table: "server",
                column: "api_key");

            migrationBuilder.CreateIndex(
                name: "IX_server_source",
                table: "server",
                column: "source");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_frags",
                table: "player_match",
                column: "frags");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_name_raw",
                table: "player_match",
                column: "name_raw");

            migrationBuilder.CreateIndex(
                name: "IX_player_match_player_match_end_player_match_start",
                table: "player_match",
                columns: new[] { "player_match_end", "player_match_start" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_server_state_last_query",
                table: "server_state");

            migrationBuilder.DropIndex(
                name: "IX_server_snapshot_timestamp",
                table: "server_snapshot");

            migrationBuilder.DropIndex(
                name: "IX_server_match_match_end_match_start",
                table: "server_match");

            migrationBuilder.DropIndex(
                name: "IX_server_active",
                table: "server");

            migrationBuilder.DropIndex(
                name: "IX_server_address_port",
                table: "server");

            migrationBuilder.DropIndex(
                name: "IX_server_api_key",
                table: "server");

            migrationBuilder.DropIndex(
                name: "IX_server_source",
                table: "server");

            migrationBuilder.DropIndex(
                name: "IX_player_match_frags",
                table: "player_match");

            migrationBuilder.DropIndex(
                name: "IX_player_match_name_raw",
                table: "player_match");

            migrationBuilder.DropIndex(
                name: "IX_player_match_player_match_end_player_match_start",
                table: "player_match");
        }
    }
}
