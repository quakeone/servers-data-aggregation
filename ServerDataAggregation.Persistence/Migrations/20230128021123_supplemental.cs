using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServerDataAggregation.Persistence.Migrations
{


    /// <inheritdoc />
    public partial class supplemental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE FUNCTION f_server_map_percentage (
	p_date TIMESTAMP,
	p_server_id INTEGER)
returns table(map text, percent numeric) as $$
declare
total integer;
BEGIN 
	total := 
		(SELECT  SUM(extract(epoch from (sm.match_start - coalesce(sm.match_end, timezone('utc', now())))))
		FROM		server_match as sm
		WHERE		sm.match_start
		 			BETWEEN p_date + (-30 *interval '1 day') AND p_date
		AND 		sm.server_id = p_server_id
		AND EXISTS (SELECT * FROM player_match as pm WHERE pm.server_match_id = sm.server_match_id AND pm.frags > 0));
	RETURN QUERY			
	SELECT   
			sub.map as map, 
			round(cast(100 * (time_spent/total) as numeric), 2) as percent
	FROM
		(SELECT 
			sm.map, SUM(extract(epoch from (sm.match_start - coalesce(sm.match_end, timezone('utc', now()))))) as time_spent
		FROM		server_match as sm
		WHERE		sm.match_start 
		 			BETWEEN p_date + (-30 *interval '1 day') AND p_date
		AND 		sm.server_id = p_server_id
		AND EXISTS (SELECT * FROM player_match as pm WHERE pm.server_match_id = sm.server_match_id AND pm.frags > 0)
		GROUP BY sm.map) as sub
	ORDER BY sub.time_spent DESC
	LIMIT 20;
END;
$$ language plpgsql;");
            migrationBuilder.Sql(@"
create view v_server_state as
select s.server_id, s.game_id, s.address, s.port, s.country, s.locality, s.mod, s.parameters,
	ss.timestamp, ss.query_result, ss.last_query, ss.hostname, ss.map, ss.mode, ss.max_players, ss.server_settings, ss.players,
	sm.match_start as last_match_start, sm.match_end as last_match_end
FROM server s
INNER JOIN server_state ss on (s.server_id = ss.server_id)
LEFT JOIN server_match sm ON
	sm.server_match_id =  (SELECT  server_match_id 
                        from server_match 
                        WHERE server_id = s.server_id 
                        ORDER BY match_start 
                        DESC LIMIT 1)
	
WHERE s.active = true
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION f_server_map_percentage;");
            migrationBuilder.Sql("DROP VIEW v_server_state;");
        }
    }
}
