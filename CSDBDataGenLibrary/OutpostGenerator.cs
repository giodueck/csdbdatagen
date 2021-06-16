using System;
using Npgsql;
using System.Collections.Generic;

namespace CSDBDataGenLibrary
{
    public class OutpostGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> outpostIds, long officeId, List<long> leaderIds, List<long> viceLeaderIds)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO outpost (outpost_id, office_id, leader_id, vice_leader_id, outpost_number, name) VALUES ";

            var nameGen = new NameGenerator();
            long outpost_id;

            foreach (long leader_id in leaderIds)
            {
                if (leaderIds[0] != leader_id)
                    sql += ",";
                
                // Get next outpost_id
                cmd.CommandText = "SELECT * FROM nextval('outpost_outpost_id_seq')";
                outpost_id = (long)cmd.ExecuteScalar();
                outpostIds.Add((long)outpost_id);

                // sql
                sql += string.Format("({0}, {1}, {2}, {3}, {4}, '{5}')", outpost_id, officeId, leader_id, viceLeaderIds[leaderIds.IndexOf(leader_id)], outpost_id, nameGen.getLastName());
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}