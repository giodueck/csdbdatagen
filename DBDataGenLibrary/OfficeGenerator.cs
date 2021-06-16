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
            string sql = "INSERT INTO office (leader_id, vice_leader_id, region) VALUES ";

            var regionGen = new PlaceNameGenerator();

            foreach (long leader_id in leaderIds)
            {
                if (leaderIds[0] != leader_id)
                    sql += ",";

                // sql
                sql += string.Format("({0}, {1}, '{2}')",leader_id, viceLeaderIds[leaderIds.IndexOf(leader_id)], regionGen.Generate());
            }

            // Store ids
            cmd.CommandText = sql + " RETURNING office_id";
            IDReader.Read(cmd.ExecuteReader(), ref officeIds);
        }
    }
}