using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class TeamGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> teamIds, long divisionId, List<long> leaderIds, List<long> juniorLeaderIds, char gender, DateTime startDate)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO team (team_id, division_id, leader_id, junior_leader_id, name, gender, start_date) VALUES ";
            string history = "INSERT INTO leader_team_history (leader_team_history_id, team_id, leader_id, is_junior, join_date) VALUES ";

            long team_id, leader_team_history_id, junior_leader_team_history_id;
            var nameGenerator = new NameGenerator();
            string name;

            foreach (long leader_id in leaderIds)
            {
                if (leaderIds[0] != leader_id)
                {
                    sql += ",";
                    history += ",";
                }
                
                // Get next team_id
                cmd.CommandText = "SELECT * FROM nextval('team_team_id_seq')";
                team_id = (long)cmd.ExecuteScalar();
                teamIds.Add((long)team_id);

                // Get next leader_team_history_id for leader and junior
                cmd.CommandText = "SELECT * FROM nextval('leader_team_history_leader_team_history_id_seq')";
                leader_team_history_id = (long)cmd.ExecuteScalar();
                cmd.CommandText = "SELECT * FROM nextval('leader_team_history_leader_team_history_id_seq')";
                junior_leader_team_history_id = (long)cmd.ExecuteScalar();

                // name
                name = nameGenerator.getAdjective() + " " + nameGenerator.getAnimals();
                NameGenerator.CapitalizeAt(0, ref name);

                // sql
                sql += string.Format("({0}, {1}, {2}, {3}, '{4}', '{5}', '{6}')", team_id, divisionId, leader_id, juniorLeaderIds[leaderIds.IndexOf(leader_id)], name, gender, new NpgsqlTypes.NpgsqlDate(startDate));
                history += string.Format("({0}, {1}, {2}, {3}, '{4}'),", leader_team_history_id, team_id, leader_id, false, startDate);
                history += string.Format("({0}, {1}, {2}, {3}, '{4}')", junior_leader_team_history_id, team_id, juniorLeaderIds[leaderIds.IndexOf(leader_id)], true, startDate);
            }

            cmd.CommandText = sql + ";" + history;
            cmd.ExecuteNonQuery();
        }
        
        public static void ReplaceLeader(NpgsqlConnection conn, long teamId, long newLeaderId, bool isJunior, DateTime replaceDate)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            int old_leader_id;  // for some reason this time the id is int32 and not int64

            // get old leader
            if (isJunior)
            {
                cmd.CommandText = "SELECT leader_id FROM team WHERE team_id = " + teamId;
                old_leader_id = (int)cmd.ExecuteScalar();
            } else
            {
                cmd.CommandText = "SELECT junior_leader_id FROM team WHERE team_id = " + teamId;
                old_leader_id = (int)cmd.ExecuteScalar();
            }

            // old leader leaves
            cmd.CommandText = string.Format("UPDATE leader_team_history SET leave_date = '{0}' WHERE leave_date IS null AND leader_id = {1};", new NpgsqlTypes.NpgsqlDate(replaceDate), old_leader_id);
            cmd.ExecuteNonQuery();

            // new leader joins
            PersonGenerator.JoinTeam(conn, newLeaderId, isJunior, teamId, new NpgsqlTypes.NpgsqlDate(replaceDate));

            // update team
            if (isJunior)
                cmd.CommandText = string.Format("UPDATE team SET leader_id = {0} WHERE team_id = {1}", teamId, newLeaderId);
            else
                cmd.CommandText = string.Format("UPDATE team SET junior_leader_id = {0} WHERE team_id = {1}", teamId, newLeaderId);
            cmd.ExecuteNonQuery();
        }
    }
}