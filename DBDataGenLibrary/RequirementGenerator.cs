using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class RequirementGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> requirementIds, int count, long award_id)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO requirement (requirement_id, award_id, description) VALUES ";

            NameGenerator generator = new NameGenerator();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sql += ",";
                
                // Get next requirement_id
                cmd.CommandText = "SELECT * FROM nextval('requirement_requirement_id_seq')";
                var requirement_id = cmd.ExecuteScalar();
                requirementIds.Add((long)requirement_id);

                // description
                string description = generator.getAdjective();
                NameGenerator.CapitalizeAt(0, ref description);

                // sql
                sql += String.Format("({0}, {1}, '{2}')", requirement_id.ToString(), award_id.ToString(), description);
            }
            
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static void GiveRequirements(NpgsqlConnection conn, List<long> reqIds, List<long> personIds, NpgsqlTypes.NpgsqlDate date, long certifyingLeaderId)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO person_requirement (person_id, requirement_id, \"date\", leader_id) VALUES ";

            foreach (long person_id in personIds)
            {
                if (personIds[0] != person_id)
                    sql += ",";
                
                foreach (long requirement_id in reqIds)
                {
                    if (reqIds[0] != requirement_id)
                        sql += ",";

                    sql += string.Format("({0}, {1}, '{2}', {3})", person_id.ToString(), requirement_id.ToString(), date, certifyingLeaderId.ToString());
                }
            }

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
