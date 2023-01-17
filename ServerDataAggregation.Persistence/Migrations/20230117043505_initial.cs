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
                    failedqueryattempts = table.Column<int>(name: "failed_query_attempts", type: "integer", nullable: false),
                    lastquery = table.Column<DateTime>(name: "last_query", type: "timestamp with time zone", nullable: true),
                    lastquerysuccess = table.Column<DateTime>(name: "last_query_success", type: "timestamp with time zone", nullable: true),
                    queryresult = table.Column<int>(name: "query_result", type: "integer", nullable: false),
                    nextquery = table.Column<DateTime>(name: "next_query", type: "timestamp with time zone", nullable: true),
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
                    serversettings = table.Column<string>(name: "server_settings", type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_snapshot", x => x.serversnapshotid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server");

            migrationBuilder.DropTable(
                name: "server_snapshot");
        }
    }
}
