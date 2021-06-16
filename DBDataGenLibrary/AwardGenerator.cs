using System;
using Npgsql;
using System.Collections.Generic;

namespace CSDBDataGenLibrary
{
    public class AwardGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> awardIds, int count, long division_category_id = 0, long parent_award_id = 0)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO award (award_id, name, parent_award_id, division_category_id) VALUES ";

            NameGenerator generator = new NameGenerator();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sql += ",";
                
                // Get next award_id
                cmd.CommandText = "SELECT * FROM nextval('award_award_id_seq')";
                var award_id = cmd.ExecuteScalar();
                awardIds.Add((long)award_id);

                // name
                string name = generator.getAnimals();

                // sql
                sql += String.Format("({0}, '{1}', {2}, {3})", award_id.ToString(), name, parent_award_id == 0 ? "null" : parent_award_id.ToString(), division_category_id == 0 ? "null" : division_category_id.ToString());

                // parent_award_id
                parent_award_id = (long)award_id;
            }
            
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static void GiveAwards(NpgsqlConnection conn, List<long> awardIds, List<long> personIds, NpgsqlTypes.NpgsqlDate date, long certifyingLeaderId)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO person_award (person_id, award_id, \"date\", leader_id) VALUES ";

            foreach (long person_id in personIds)
            {
                if (personIds[0] != person_id)
                    sql += ",";

                foreach (long award_id in awardIds)
                {
                    if (awardIds[0] != award_id)
                        sql += ",";

                    sql += string.Format("({0}, {1}, '{2}', {3})", person_id.ToString(), award_id.ToString(), date, certifyingLeaderId.ToString());
                }
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
