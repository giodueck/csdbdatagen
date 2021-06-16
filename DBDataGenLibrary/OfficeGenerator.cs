using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class OfficeGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> officeIds, List<long> leaderIds, List<long> viceLeaderIds)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO office (office_id, leader_id, vice_leader_id, region) VALUES ";

            var regionGen = new PlaceNameGenerator();
            long office_id;

            foreach (long leader_id in leaderIds)
            {
                if (leaderIds[0] != leader_id)
                    sql += ",";
                
                // Get next office_id
                cmd.CommandText = "SELECT * FROM nextval('office_office_id_seq')";
                office_id = (long)cmd.ExecuteScalar();
                officeIds.Add((long)office_id);

                // sql
                sql += string.Format("({0}, {1}, {2}, '{3}')", office_id, leader_id, viceLeaderIds[leaderIds.IndexOf(leader_id)], regionGen.Generate());
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}