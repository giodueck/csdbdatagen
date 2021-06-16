using System;
using Npgsql;
using System.Collections.Generic;

namespace CSDBDataGenLibrary
{
    public class DivisionGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> divisionIds, long outpostId, List<long> divisionCategoryIds, List<long> leaderIds, List<long> viceLeaderIds)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO division (division_id, division_category_id, outpost_id, leader_id, vice_leader_id) VALUES ";

            long division_id;

            foreach (long division_category_id in divisionCategoryIds)
            {
                if (divisionCategoryIds[0] != division_category_id)
                    sql += ",";
                
                // Get next division_id
                cmd.CommandText = "SELECT * FROM nextval('division_division_id_seq')";
                division_id = (long)cmd.ExecuteScalar();
                divisionIds.Add((long)division_id);

                // sql
                sql += string.Format("({0}, {1}, {2}, {3}, {4})", division_id, division_category_id, outpostId, leaderIds[divisionCategoryIds.IndexOf(division_category_id)], viceLeaderIds[divisionCategoryIds.IndexOf(division_category_id)]);
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}