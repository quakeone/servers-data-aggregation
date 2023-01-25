using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "server",
                columns: table => new
                {
                    serverid = table.Column<int>(name: "server_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    gameid = table.Column<int>(name: "game_id", type: "integer", nullable: false),
                    port = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    country = table.Column<string>(type: "text", nullable: true),
                    locality = table.Column<string>(type: "text", nullable: true),
                    queryinterval = table.Column<int>(name: "query_interval", type: "integer", nullable: false),
                    mod = table.Column<string>(type: "text", nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    apikey = table.Column<string>(name: "api_key", type: "text", nullable: false),
                    parameters = table.Column<string>(type: "text", nullable: true),
                    metadata = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server", x => x.serverid);
                });

            migrationBuilder.CreateTable(
                name: "server_snapshot",
                columns: table => new
                {
                    serversnapshotid = table.Column<int>(name: "server_snapshot_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serverid = table.Column<int>(name: "server_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    hostname = table.Column<string>(type: "text", nullable: false),
                    map = table.Column<string>(type: "text", nullable: false),
                    mod = table.Column<string>(type: "text", nullable: true),
                    mode = table.Column<string>(type: "text", nullable: true),
                    ipaddress = table.Column<string>(name: "ip_address", type: "text", nullable: false),
                    maxplayers = table.Column<int>(name: "max_players", type: "integer", nullable: false),
                    serversettings = table.Column<string>(name: "server_settings", type: "text", nullable: false),
                    players = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_snapshot", x => x.serversnapshotid);
                });

            migrationBuilder.CreateTable(
                name: "server_match",
                columns: table => new
                {
                    servermatchid = table.Column<int>(name: "server_match_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serverid = table.Column<int>(name: "server_id", type: "integer", nullable: false),
                    map = table.Column<string>(type: "text", nullable: false),
                    mod = table.Column<string>(type: "text", nullable: true),
                    mode = table.Column<string>(type: "text", nullable: true),
                    matchstart = table.Column<DateTime>(name: "match_start", type: "timestamp with time zone", nullable: false),
                    matchend = table.Column<DateTime>(name: "match_end", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_match", x => x.servermatchid);
                    table.ForeignKey(
                        name: "FK_server_match_server_server_id",
                        column: x => x.serverid,
                        principalTable: "server",
                        principalColumn: "server_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "server_state",
                columns: table => new
                {
                    serverstateid = table.Column<int>(name: "server_state_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serverid = table.Column<int>(name: "server_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    hostname = table.Column<string>(type: "text", nullable: true),
                    map = table.Column<string>(type: "text", nullable: true),
                    mod = table.Column<string>(type: "text", nullable: true),
                    mode = table.Column<string>(type: "text", nullable: true),
                    ipaddress = table.Column<string>(name: "ip_address", type: "text", nullable: true),
                    maxplayers = table.Column<int>(name: "max_players", type: "integer", nullable: false),
                    serversettings = table.Column<string>(name: "server_settings", type: "text", nullable: true),
                    players = table.Column<string>(type: "jsonb", nullable: true),
                    failedqueryattempts = table.Column<int>(name: "failed_query_attempts", type: "integer", nullable: false),
                    lastquery = table.Column<DateTime>(name: "last_query", type: "timestamp with time zone", nullable: true),
                    queryresult = table.Column<int>(name: "query_result", type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_state", x => x.serverstateid);
                    table.ForeignKey(
                        name: "FK_server_state_server_server_id",
                        column: x => x.serverid,
                        principalTable: "server",
                        principalColumn: "server_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "player_match",
                columns: table => new
                {
                    playermatchid = table.Column<int>(name: "player_match_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    servermatchid = table.Column<int>(name: "server_match_id", type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    nameraw = table.Column<string>(name: "name_raw", type: "text", nullable: false),
                    number = table.Column<int>(type: "integer", nullable: false),
                    shirtcolor = table.Column<int>(name: "shirt_color", type: "integer", nullable: false),
                    pantcolor = table.Column<int>(name: "pant_color", type: "integer", nullable: false),
                    model = table.Column<string>(type: "text", nullable: true),
                    skin = table.Column<string>(type: "text", nullable: true),
                    frags = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    playermatchstart = table.Column<DateTime>(name: "player_match_start", type: "timestamp with time zone", nullable: true),
                    playermatchend = table.Column<DateTime>(name: "player_match_end", type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_match", x => x.playermatchid);
                    table.ForeignKey(
                        name: "FK_player_match_server_match_server_match_id",
                        column: x => x.servermatchid,
                        principalTable: "server_match",
                        principalColumn: "server_match_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_match_server_match_id",
                table: "player_match",
                column: "server_match_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_match_server_id",
                table: "server_match",
                column: "server_id");

            migrationBuilder.CreateIndex(
                name: "IX_server_state_server_id",
                table: "server_state",
                column: "server_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_match");

            migrationBuilder.DropTable(
                name: "server_snapshot");

            migrationBuilder.DropTable(
                name: "server_state");

            migrationBuilder.DropTable(
                name: "server_match");

            migrationBuilder.DropTable(
                name: "server");
        }
    }
}
