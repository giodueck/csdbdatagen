using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class DivisionGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> divisionIds, long outpostId, List<long> divisionCategoryIds, List<long> leaderIds, List<long> viceLeaderIds)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO division (division_category_id, outpost_id, leader_id, vice_leader_id) VALUES ";

            foreach (long division_category_id in divisionCategoryIds)
            {
                if (divisionCategoryIds[0] != division_category_id)
                    sql += ",";

                // sql
                sql += string.Format("({0}, {1}, {2}, {3})", division_category_id, outpostId, leaderIds[divisionCategoryIds.IndexOf(division_category_id)], viceLeaderIds[divisionCategoryIds.IndexOf(division_category_id)]);
            }

            // Store ids
            cmd.CommandText = sql + " RETURNING division_id";
            IDReader.Read(cmd.ExecuteReader(), ref divisionIds);
        }
    }
}