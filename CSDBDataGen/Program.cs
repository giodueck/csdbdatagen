using System;
using Npgsql;
using System.Collections.Generic;

namespace CSDBDataGen
{
    class Program
    {
        // Generates awards with their requirements for all the given division categories
        static void GenAwards(NpgsqlConnection conn, int awardCount, ref List<long> awardIds, int reqCount, ref List<long> reqIds, List<long> divisionCategoryIds)
        {
            foreach (long dcid in divisionCategoryIds)
                CSDBDataGenLibrary.AwardGenerator.Generate(conn, ref awardIds, awardCount, dcid);
            foreach (long id in awardIds)
            {
                CSDBDataGenLibrary.RequirementGenerator.Generate(conn, ref reqIds, reqCount, id);
            }
        }

        // Associates the given awards and their requirements with the given persons
        static void GiveAwards(NpgsqlConnection conn, List<long> awardIds, List<long> reqIds, List<long> personIds, DateTime minDate, DateTime maxDate, long certifyingLeaderId)
        {
            var date = CSDBDataGenLibrary.DateGenerator.Generate(minDate, maxDate);
            CSDBDataGenLibrary.RequirementGenerator.GiveRequirements(conn, reqIds, personIds, date, certifyingLeaderId);
            CSDBDataGenLibrary.AwardGenerator.GiveAwards(conn, awardIds, personIds, date, certifyingLeaderId);
        }

        // Generates persons and automatically makes them leaders
        static void GenLeaders(NpgsqlConnection conn, int count, ref List<long> personIds, ref List<long> leaderIds, DateTime minBirthday, DateTime maxBirthday, DateTime startDate, char gender = ' ', bool isJunior = false)
        {
            CSDBDataGenLibrary.PersonGenerator.Generate(conn, ref personIds, count, minBirthday, maxBirthday, gender);
            CSDBDataGenLibrary.PersonGenerator.MakeLeader(conn, ref leaderIds, personIds, isJunior, startDate);
        }

        // Generates persons and automatically makes them scouts
        static void GenScouts(NpgsqlConnection conn, int count, ref List<long> personIds, ref List<long> scoutIds, DateTime minBirthday, DateTime maxBirthday, DateTime startDate, char gender = ' ', int teamId = 0)
        {
            CSDBDataGenLibrary.PersonGenerator.Generate(conn, ref personIds, count, minBirthday, maxBirthday, gender);
            CSDBDataGenLibrary.PersonGenerator.MakeScout(conn, ref scoutIds, personIds, startDate, teamId);
        }

        static void Main(string[] args)
        {
            System.Console.WriteLine("Start");

            // Open the connection
            var connString = "Host=localhost;Username=postgres;Password=password;Database=ch6datagen";
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            // Create a command variable
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Arrays of ids
            var awardIds = new List<long>();
            var requirementIds = new List<long>();
            var leaderAwardIds = new List<long>();
            var leaderReqIds = new List<long>();
            var discoveryAwardIds = new List<long>();
            var discoveryReqIds = new List<long>();
            var adventureAwardIds = new List<long>();
            var adventureReqIds = new List<long>();
            var expeditionAwardIds = new List<long>();
            var expeditionReqIds = new List<long>();
            var divisionCategoryIds = new List<long>();
            var divisionCategoryNames = new List<string>();
            var personIds = new List<long>();
            var leaderIds = new List<long>();
            var scoutIds = new List<long>();
            var officeIds = new List<long>();
            var outpostIds = new List<long>();
            var divisionIds = new List<long>();

            var auxList = new List<long>();
            var auxList2 = new List<long>();

            // Useful variables
            int i, j;
            int awardCount = 4, reqCount = 2;
            var leaderMinBD = new DateTime(1971, 1, 1);
            var leaderMaxBD = new DateTime(2003, 12, 31);
            var startDate = new DateTime(2020, 1, 1);

            // Clear database for testing
            cmd.CommandText = "SELECT * FROM clearall()";
            System.Console.WriteLine("Clearing DB: {0}", cmd.ExecuteScalar().ToString());

            // Categories and awards with their requirements
            divisionCategoryNames.Add("Discovery");
            divisionCategoryNames.Add("Adventure");
            divisionCategoryNames.Add("Expedition");
            divisionCategoryIds.Add(0); // leaders
            CSDBDataGenLibrary.DivisionCategoryGenerator.Generate(conn, ref divisionCategoryIds, divisionCategoryNames);
            GenAwards(conn, awardCount, ref awardIds, reqCount, ref requirementIds, divisionCategoryIds);
            divisionCategoryIds.Remove(0);
            for (i = 0; i < awardCount; i++)
            {
                leaderAwardIds.Add(awardIds[i]);
                discoveryAwardIds.Add(awardIds[i + awardCount]);
                adventureAwardIds.Add(awardIds[i + 2 * awardCount]);
                expeditionAwardIds.Add(awardIds[i + 3 * awardCount]);
                for (j = 0; j < reqCount; j++)
                {
                    leaderReqIds.Add(requirementIds[i * reqCount + j]);
                    discoveryReqIds.Add(requirementIds[(i + awardCount) * reqCount + j]);
                    adventureReqIds.Add(requirementIds[(i + 2 * awardCount) * reqCount + j]);
                    expeditionReqIds.Add(requirementIds[(i + 3 * awardCount) * reqCount + j]);
                }
            }

            // Persons
            GenScouts(conn, 10, ref personIds, ref scoutIds, leaderMinBD, leaderMaxBD, startDate);
            CSDBDataGenLibrary.PersonGenerator.Pause(conn, personIds, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1), 0);
            personIds.Clear();
            GenLeaders(conn, 10, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
            CSDBDataGenLibrary.PersonGenerator.Pause(conn, personIds, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));

            auxList.Add(1);
            auxList.Add(5);
            GiveAwards(conn, discoveryAwardIds, discoveryReqIds, auxList, startDate, DateTime.Today, 1);
            auxList.Clear();
            auxList.Add(11);
            auxList.Add(17);
            GiveAwards(conn, leaderAwardIds, leaderReqIds, auxList, startDate, DateTime.Today, 1);

            // Office
            auxList.Clear();
            personIds.Clear();
            GenLeaders(conn, 1, ref personIds, ref auxList, leaderMinBD, leaderMaxBD, startDate);
            personIds.Clear();
            GenLeaders(conn, 1, ref personIds, ref auxList2, leaderMinBD, leaderMaxBD, startDate);
            leaderIds.AddRange(auxList);
            CSDBDataGenLibrary.OfficeGenerator.Generate(conn, ref officeIds, auxList, auxList2);

            // Outpost
            auxList.Clear();
            personIds.Clear();
            GenLeaders(conn, 1, ref personIds, ref auxList, leaderMinBD, leaderMaxBD, startDate);
            personIds.Clear();
            GenLeaders(conn, 1, ref personIds, ref auxList2, leaderMinBD, leaderMaxBD, startDate);
            leaderIds.AddRange(auxList);
            CSDBDataGenLibrary.OutpostGenerator.Generate(conn, ref outpostIds, 1, auxList, auxList2);

            // Division
            auxList.Clear();
            personIds.Clear();
            GenLeaders(conn, divisionCategoryIds.Count, ref personIds, ref auxList, leaderMinBD, leaderMaxBD, startDate);
            personIds.Clear();
            GenLeaders(conn, divisionCategoryIds.Count, ref personIds, ref auxList2, leaderMinBD, leaderMaxBD, startDate);
            leaderIds.AddRange(auxList);
            CSDBDataGenLibrary.DivisionGenerator.Generate(conn, ref divisionIds, outpostIds[0], divisionCategoryIds, auxList, auxList2);

            System.Console.WriteLine("End");
        }
    }
}
