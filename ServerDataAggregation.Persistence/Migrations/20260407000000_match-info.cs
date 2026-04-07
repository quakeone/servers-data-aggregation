using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class matchinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "match_status",
                table: "server_state");

            migrationBuilder.AddColumn<string>(
                name: "match_info",
                table: "server_state",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql("DROP VIEW v_server_state;");
            migrationBuilder.Sql(@"
CREATE VIEW v_server_state AS
SELECT s.server_id, s.game_id, s.address, s.port, s.country, s.locality, s.mod, s.parameters,
    ss.timestamp, ss.query_result, ss.last_query, ss.hostname, ss.map, ss.mode, ss.max_players, ss.server_settings, ss.players,
    ss.match_info,
    sm.match_start AS last_match_start, sm.match_end AS last_match_end
FROM server s
INNER JOIN server_state ss ON (s.server_id = ss.server_id)
LEFT JOIN server_match sm ON
    sm.server_match_id = (SELECT server_match_id
                        FROM server_match
                        WHERE server_id = s.server_id
                        ORDER BY match_start
                        DESC LIMIT 1)
WHERE s.active = true AND ss.failed_query_attempts < 20
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW v_server_state;");
            migrationBuilder.Sql(@"
CREATE VIEW v_server_state AS
SELECT s.server_id, s.game_id, s.address, s.port, s.country, s.locality, s.mod, s.parameters,
    ss.timestamp, ss.query_result, ss.last_query, ss.hostname, ss.map, ss.mode, ss.max_players, ss.server_settings, ss.players,
    sm.match_start AS last_match_start, sm.match_end AS last_match_end
FROM server s
INNER JOIN server_state ss ON (s.server_id = ss.server_id)
LEFT JOIN server_match sm ON
    sm.server_match_id = (SELECT server_match_id
                        FROM server_match
                        WHERE server_id = s.server_id
                        ORDER BY match_start
                        DESC LIMIT 1)
WHERE s.active = true AND ss.failed_query_attempts < 120
");

            migrationBuilder.DropColumn(
                name: "match_info",
                table: "server_state");

            migrationBuilder.AddColumn<int>(
                name: "match_status",
                table: "server_state",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
