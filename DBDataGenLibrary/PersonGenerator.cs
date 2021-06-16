using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class PersonGenerator
    {
        public static void Generate(NpgsqlConnection conn, ref List<long> personIds, int count, DateTime minBirthday, DateTime maxBirthday, char gender = ' ')
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO person (person_id, first_name, last_name, date_of_birth, gender) VALUES ";

            char[] genders = {'M', 'F'};
            bool none = false;
            if (gender != 'M' && gender != 'm' && gender != 'F' && gender != 'f')
                none = true;
            var generator = new NameGenerator();
            var rand = new Random();
            NpgsqlTypes.NpgsqlDate bday;
            string fn, ln;
            long person_id;

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    sql += ",";

                // Get next person_id
                cmd.CommandText = "SELECT * FROM nextval('person_person_id_seq')";
                person_id = (long)cmd.ExecuteScalar();
                personIds.Add((long)person_id);

                // first_name
                if (none) gender = genders[rand.Next(2)];
                fn = generator.getFirstName(gender);

                // last_name
                ln = generator.getLastName();

                // birthday
                bday = DateGenerator.Generate(minBirthday, maxBirthday);

                // sql
                sql += String.Format("({0}, '{1}', '{2}', '{3}', '{4}')", person_id.ToString(), fn, ln, bday, gender);
            }
            
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public static void MakeLeader(NpgsqlConnection conn, ref List<long> leaderIds, List<long> personIds, bool isJunior, DateTime startDate, bool rejoing = false)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO leader (leader_id, person_id, is_junior) VALUES ";
            string history = "INSERT INTO leader_history (leader_history_id, person_id, start_date, is_junior) VALUES ";

            long leader_id = 0, leader_history_id;

            foreach (int person_id in personIds)
            {
                if (personIds[0] != person_id)
                {
                    sql += ",";
                    history += ",";
                }

                // Get next leader_id
                if (!rejoing)
                {
                    cmd.CommandText = "SELECT * FROM nextval('leader_leader_id_seq_1_1')";
                    leader_id = (long)cmd.ExecuteScalar();
                    leaderIds.Add((long)leader_id);
                }

                // Get next leader_history_id
                cmd.CommandText = "SELECT * FROM nextval('leader_history_leader_history_id_seq')";
                leader_history_id = (long)cmd.ExecuteScalar();

                // sql
                if (!rejoing) sql += String.Format("({0}, {1}, {2})", leader_id.ToString(), person_id.ToString(), isJunior.ToString());
                history += String.Format("({0}, {1}, '{2}', {3})", leader_history_id.ToString(), person_id.ToString(), new NpgsqlTypes.NpgsqlDate(startDate), isJunior.ToString());
            }

            if (!rejoing)
                cmd.CommandText = sql + ";" + history;
            else
                cmd.CommandText = history;
            cmd.ExecuteNonQuery();
        }

        public static void MakeLeader(NpgsqlConnection conn, ref List<long> leaderIds, List<long> personIds, bool isJunior, NpgsqlTypes.NpgsqlDate startDate, bool rejoing = false)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO leader (leader_id, person_id, is_junior) VALUES ";
            string history = "INSERT INTO leader_history (leader_history_id, person_id, start_date, is_junior) VALUES ";

            long leader_id = 0, leader_history_id;

            foreach (int person_id in personIds)
            {
                if (personIds[0] != person_id)
                {
                    sql += ",";
                    history += ",";
                }

                // Get next leader_id
                if (!rejoing)
                {
                    cmd.CommandText = "SELECT * FROM nextval('leader_leader_id_seq_1_1')";
                    leader_id = (long)cmd.ExecuteScalar();
                    leaderIds.Add((long)leader_id);
                }

                // Get next leader_history_id
                cmd.CommandText = "SELECT * FROM nextval('leader_history_leader_history_id_seq')";
                leader_history_id = (long)cmd.ExecuteScalar();

                // sql
                if (!rejoing) sql += String.Format("({0}, {1}, {2})", leader_id.ToString(), person_id.ToString(), isJunior.ToString());
                history += String.Format("({0}, {1}, '{2}', {3})", leader_history_id.ToString(), person_id.ToString(), startDate, isJunior.ToString());
            }

            if (!rejoing)
                cmd.CommandText = sql + ";" + history;
            else
                cmd.CommandText = history;
            cmd.ExecuteNonQuery();
        }

        public static void MakeScout(NpgsqlConnection conn, ref List<long> scoutIds, List<long> personIds, DateTime startDate, long teamId = 0, bool rejoining = false)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "INSERT INTO scout (scout_id, person_id, team_id) VALUES ";
            string history = "INSERT INTO scout_history (scout_history_id, person_id, start_date) VALUES ";
            string thistory = "INSERT INTO scout_team_history (scout_team_history_id, team_id, scout_id, join_date) VALUES ";

            long scout_id = 0, scout_history_id, scout_team_history_id = 0;

            foreach (int person_id in personIds)
            {
                if (personIds[0] != person_id)
                {
                    sql += ",";
                    history += ",";
                    thistory += ",";
                }

                // Get next scout_id
                if (!rejoining)
                {
                    cmd.CommandText = "SELECT * FROM nextval('scout_scout_id_seq')";
                    scout_id = (long)cmd.ExecuteScalar();
                    scoutIds.Add((long)scout_id);
                }

                // Get next scout_history_id
                cmd.CommandText = "SELECT * FROM nextval('scout_history_scout_history_id_seq')";
                scout_history_id = (long)cmd.ExecuteScalar();

                // Get next scout_team_history_id
                if (teamId > 0)
                {
                    cmd.CommandText = "SELECT * FROM nextval('scout_team_history_scout_team_history_id_seq')";
                    scout_team_history_id = (long)cmd.ExecuteScalar();
                }

                // sql
                if (!rejoining) sql += String.Format("({0}, {1}, {2})", scout_id.ToString(), person_id.ToString(), (teamId > 0) ? teamId.ToString() : "null");
                history += String.Format("({0}, {1}, '{2}')", scout_history_id.ToString(), person_id.ToString(), new NpgsqlTypes.NpgsqlDate(startDate));
                if (teamId > 0) thistory += String.Format("({0}, {1}, {2}, '{3}')", scout_team_history_id.ToString(), (teamId > 0) ? teamId.ToString() : "null", scout_id.ToString(), new NpgsqlTypes.NpgsqlDate(startDate));
            }

            if (!rejoining)
                cmd.CommandText = sql + ";" + history;
            else 
                cmd.CommandText = history;
            if (teamId > 0) cmd.CommandText += ";" + thistory;
            cmd.ExecuteNonQuery();
        }

        public static void MakeScout(NpgsqlConnection conn, ref List<long> scoutIds, List<long> personIds, NpgsqlTypes.NpgsqlDate startDate, long teamId = 0, bool rejoining = false)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Build sql
            string sql = "";
            if (!rejoining) sql = "INSERT INTO scout (scout_id, person_id, team_id) VALUES ";
            string history = "INSERT INTO scout_history (scout_history_id, person_id, start_date) VALUES ";
            string thistory = "INSERT INTO scout_team_history (scout_team_history_id, team_id, scout_id, join_date) VALUES ";

            long scout_id = 0, scout_history_id, scout_team_history_id = 0;

            foreach (int person_id in personIds)
            {
                if (personIds[0] != person_id)
                {
                    if (!rejoining) sql += ",";
                    else sql += ";";
                    history += ",";
                    thistory += ",";
                }

                // Get next scout_id
                if (!rejoining)
                {
                    cmd.CommandText = "SELECT * FROM nextval('scout_scout_id_seq')";
                    scout_id = (long)cmd.ExecuteScalar();
                    scoutIds.Add((long)scout_id);
                } else
                {
                    cmd.CommandText = "SELECT scout_id FROM scout WHERE person_id = " + person_id;
                    scout_id = (int)cmd.ExecuteScalar();
                }

                // Get next scout_history_id
                cmd.CommandText = "SELECT * FROM nextval('scout_history_scout_history_id_seq')";
                scout_history_id = (long)cmd.ExecuteScalar();

                // Get next scout_team_history_id
                if (teamId > 0)
                {
                    cmd.CommandText = "SELECT * FROM nextval('scout_team_history_scout_team_history_id_seq')";
                    scout_team_history_id = (long)cmd.ExecuteScalar();
                }

                // sql
                if (!rejoining) sql += String.Format("({0}, {1}, {2})", scout_id.ToString(), person_id.ToString(), (teamId > 0) ? teamId.ToString() : "null");
                else sql += string.Format("UPDATE scout SET team_id = {0} WHERE scout_id = {1}", (teamId > 0) ? teamId.ToString() : "null", scout_id);
                history += String.Format("({0}, {1}, '{2}')", scout_history_id.ToString(), person_id.ToString(), startDate);
                thistory += String.Format("({0}, {1}, {2}, '{3}')", scout_team_history_id.ToString(), (teamId > 0) ? teamId.ToString() : "null", scout_id, new NpgsqlTypes.NpgsqlDate(startDate));
            }

            cmd.CommandText = sql + ";" + history;
            if (teamId > 0) cmd.CommandText += ";" + thistory;
            cmd.ExecuteNonQuery();
        }

        // Causes the leader history to show a random 30 day pause in participation for the given persons
        public static void Pause(NpgsqlConnection conn, List<long> personIds, DateTime minDate, DateTime maxDate, bool isJunior = false)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            string sql;
            var pauseDate = DateGenerator.Generate(minDate, maxDate);
            var resumeDate = pauseDate.AddDays(30);
            var list = new List<long>();
        
            foreach (long person_id in personIds)
            {
                // update leader_history
                sql = string.Format("UPDATE leader_history SET end_date = '{0}' WHERE end_date IS null AND person_id = {1}", pauseDate.ToString(), person_id.ToString());
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }

            // new leader_history rows
            MakeLeader(conn, ref list, personIds, isJunior, resumeDate, true);
        }

        // Causes the scout history to show a random 30 day pause in participation for the given persons
        public static void Pause(NpgsqlConnection conn, List<long> personIds, DateTime minDate, DateTime maxDate, long teamId)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            var pauseDate = DateGenerator.Generate(minDate, maxDate);
            var resumeDate = pauseDate.AddDays(30);
            var list = new List<long>();
            NpgsqlDataReader reader;
            long scout_id;
        
            foreach (long person_id in personIds)
            {
                // update scout_history
                cmd.CommandText = string.Format("UPDATE scout_history SET end_date = '{0}' WHERE end_date IS null AND person_id = {1};", pauseDate.ToString(), person_id.ToString());
                cmd.ExecuteNonQuery();

                // leave team too
                cmd.CommandText = "SELECT team_id, scout_id FROM scout WHERE person_id = " + person_id;
                reader = cmd.ExecuteReader();
                try
                {
                    reader.Read();
                    reader.GetInt64(0).ToString();  // will throw exception if team_id is null
                    scout_id = reader.GetInt64(1);
                    reader.Close();
                    LeaveTeam(conn, scout_id, pauseDate);
                }
                catch (System.Exception)
                {
                    reader.Close();
                    // System.Console.WriteLine(e.ToString());
                }
            }

            // new scout_history rows
            MakeScout(conn, ref list, personIds, resumeDate, teamId, true);
        }

        // Causes the given scout to leave their team 
        public static void LeaveTeam(NpgsqlConnection conn, long scoutId, NpgsqlTypes.NpgsqlDate leaveDate)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            string sql = string.Format("UPDATE scout_team_history SET leave_date = '{0}' WHERE leave_date IS null AND scout_id = {1};", leaveDate, scoutId);
            sql += "UPDATE scout SET team_id = null WHERE scout_id = " + scoutId;

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        // Causes the given scout to join the given team. Only call if the scout has team_id = null
        public static void JoinTeam(NpgsqlConnection conn, long scoutId, long teamId, NpgsqlTypes.NpgsqlDate joinDate)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            string sql = "INSERT INTO scout_team_history (scout_team_history_id, team_id, scout_id, join_date) VALUES ";

            long scout_team_history_id;

            // Get next scout_team_history_id
            cmd.CommandText = "SELECT * FROM nextval('scout_team_history_scout_team_history_id_seq')";
            scout_team_history_id = (long)cmd.ExecuteScalar();

            sql += string.Format("({0}, {1}, {2}, '{3}')", scout_team_history_id, teamId, scoutId, joinDate);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
        
        // Causes the given leader to join the given team.
        public static void JoinTeam(NpgsqlConnection conn, long leaderId, bool isJunior, long teamId, NpgsqlTypes.NpgsqlDate joinDate)
        {
            // Create command variable
            var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            string sql = "INSERT INTO leader_team_history (leader_team_history_id, team_id, leader_id, is_junior, join_date) VALUES ";

            long leader_team_history_id;

            // Get next leader_team_history_id
            cmd.CommandText = "SELECT * FROM nextval('leader_team_history_leader_team_history_id_seq')";
            leader_team_history_id = (long)cmd.ExecuteScalar();

            sql += string.Format("({0}, {1}, {2}, {3}, '{4}')", leader_team_history_id, teamId, leaderId, isJunior, joinDate);

            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }
    }
}
