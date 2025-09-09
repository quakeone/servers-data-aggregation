using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class servermatchactual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "server_match_actual",
                columns: table => new
                {
                    servermatchactualid = table.Column<int>(name: "server_match_actual_id", type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    serverid = table.Column<int>(name: "server_id", type: "integer", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_server_match_actual", x => x.servermatchactualid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_server_match_actual_timestamp",
                table: "server_match_actual",
                column: "timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "server_match_actual");
        }
    }
}
