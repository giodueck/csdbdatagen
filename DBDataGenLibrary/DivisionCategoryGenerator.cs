using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class DivisionCategoryGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> divisionCategoryIds, List<string> names)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO division_category (division_category_id, name) VALUES ";

            NameGenerator generator = new NameGenerator();

            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    sql += ",";
                
                // Get next division_category_id
                cmd.CommandText = "SELECT * FROM nextval('division_category_division_category_id_seq_1')";
                var division_category_id = cmd.ExecuteScalar();
                divisionCategoryIds.Add((long)division_category_id);

                // sql
                sql += String.Format("({0}, '{1}')", division_category_id.ToString(), names[i]);
            }
            
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
