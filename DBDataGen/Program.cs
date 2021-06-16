using System;
using System.Threading;
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
                DBDataGenLibrary.AwardGenerator.Generate(conn, ref awardIds, awardCount, dcid);
            foreach (long id in awardIds)
            {
                DBDataGenLibrary.RequirementGenerator.Generate(conn, ref reqIds, reqCount, id);
            }
        }

        // Associates the given awards and their requirements with the given persons
        static void GiveAwards(NpgsqlConnection conn, List<long> awardIds, List<long> reqIds, List<long> personIds, DateTime minDate, DateTime maxDate, long certifyingLeaderId)
        {
            var date = DBDataGenLibrary.DateGenerator.Generate(minDate, maxDate);
            DBDataGenLibrary.RequirementGenerator.GiveRequirements(conn, reqIds, personIds, date, certifyingLeaderId);
            DBDataGenLibrary.AwardGenerator.GiveAwards(conn, awardIds, personIds, date, certifyingLeaderId);
        }

        // Generates persons and automatically makes them leaders
        static void GenLeaders(NpgsqlConnection conn, int count, ref List<long> personIds, ref List<long> leaderIds, DateTime minBirthday, DateTime maxBirthday, DateTime startDate, char gender = ' ', bool isJunior = false)
        {
            DBDataGenLibrary.PersonGenerator.Generate(conn, ref personIds, count, minBirthday, maxBirthday, gender);
            DBDataGenLibrary.PersonGenerator.MakeLeader(conn, ref leaderIds, personIds, isJunior, startDate);
        }

        // Generates persons and automatically makes them scouts
        static void GenScouts(NpgsqlConnection conn, int count, ref List<long> personIds, ref List<long> scoutIds, DateTime minBirthday, DateTime maxBirthday, DateTime startDate, char gender = ' ', long teamId = 0)
        {
            DBDataGenLibrary.PersonGenerator.Generate(conn, ref personIds, count, minBirthday, maxBirthday, gender);
            DBDataGenLibrary.PersonGenerator.MakeScout(conn, ref scoutIds, personIds, startDate, teamId);
        }

        // Generates the given number of offices with the given numbers to make it up
        static void GenerateOffice(NpgsqlConnection conn, ProgressBar progressBar, double percentage, List<long> awardList, List<long> reqList, List<long> divCatList, int outpostCount, int teamCount, int scoutCount)
        {
            // Lists of ids
            var personIds = new List<long>();
            var leaderIds = new List<long>();
            var viceLeaderIds = new List<long>();
            var scoutIds = new List<long>();
            var officeIds = new List<long>();
            var outpostIds = new List<long>();
            var divisionIds = new List<long>();
            var teamIds = new List<long>();

            // Useful variables
            var leaderMinBD = new DateTime(1971, 1, 1);
            var leaderMaxBD = new DateTime(2003, 12, 31);
            var discoveryMinBirthday = new DateTime(2010, 1, 1);
            var discoveryMaxBirthday = new DateTime(2012, 12, 31);
            var adventureMinBirthday = new DateTime(2007, 1, 1);
            var adventureMaxBirthday = new DateTime(2009, 12, 31);
            var expeditionMinBirthday = new DateTime(2004, 1, 1);
            var expeditionMaxBirthday = new DateTime(2006, 12, 31);
            var startDate = new DateTime(2020, 1, 1);
            var today = DateTime.Today;
            char gender;
            long certifier;

            // Award lists
            int n = awardList.Count / 4;
            int p = reqList.Count / (4 * n);
            List<long>[] awardLists = {new List<long>(), new List<long>(), new List<long>(), new List<long>()};
            List<long>[] reqLists = {new List<long>(), new List<long>(), new List<long>(), new List<long>()};
            for (int i = 0; i < 4; i++)
            {
                awardLists[0].Add(awardList[i]);
                awardLists[1].Add(awardList[i + n]);
                awardLists[2].Add(awardList[i + 2 * n]);
                awardLists[3].Add(awardList[i + 3 * n]);
                for (int j = 0; j < p; j++)
                {
                    reqLists[0].Add(reqList[j + i * p]);
                    reqLists[1].Add(reqList[j + (i + n) * p]);
                    reqLists[2].Add(reqList[j + (i + 2 * n) * p]);
                    reqLists[3].Add(reqList[j + (i + 3 * n) * p]);
                }
            }
            
            // Generate Office leaders
            GenLeaders(conn, 2, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
            certifier = leaderIds[0];   // for all the awards
            viceLeaderIds.Add(leaderIds[1]);
            leaderIds.RemoveAt(1);
            // awards and pause to generate extra rows in other tables
            GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
            DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
            // Generate office
            DBDataGenLibrary.OfficeGenerator.Generate(conn, ref officeIds, leaderIds, viceLeaderIds);

            // Generate outposts
            for (int i = 0; i < outpostCount; i++)
            {
                // Report progress
                progressBar.Report(((double) i / outpostCount) * 0.1 / 2 + percentage - 0.1);
                
                personIds.Clear();
                leaderIds.Clear();
                viceLeaderIds.Clear();
                // Generate outpost leaders
                GenLeaders(conn, 2, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
                viceLeaderIds.Add(leaderIds[1]);
                leaderIds.RemoveAt(1);
                // awards and pause to generate extra rows in other tables
                GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                // Generate outpost
                DBDataGenLibrary.OutpostGenerator.Generate(conn, ref outpostIds, officeIds[0], leaderIds, viceLeaderIds);

                // Generate divisions
                leaderIds.Clear();
                viceLeaderIds.Clear();
                // leaders
                personIds.Clear();
                GenLeaders(conn, divCatList.Count, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
                GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                // viceleaders
                personIds.Clear();
                GenLeaders(conn, divCatList.Count, ref personIds, ref viceLeaderIds, leaderMinBD, leaderMaxBD, startDate);
                GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                DBDataGenLibrary.DivisionGenerator.Generate(conn, ref divisionIds, outpostIds[i], divCatList, leaderIds, viceLeaderIds);
            }

            // Create teams
            for (int i = 0; i < divisionIds.Count; i++)
            {
                // Report progress
                progressBar.Report(((double) i / divisionIds.Count + 1) * 0.1 / 2 + percentage - 0.1);

                switch (i % divCatList.Count)
                {
                case 0: // discovery
                    for (int j = 0; j < teamCount; j++)
                    {
                        gender = ((j % 2) == 0) ? 'M' : 'F';
                        leaderIds.Clear();
                        viceLeaderIds.Clear();
                        // leaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // viceleaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref viceLeaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // Generate teams
                        teamIds.Clear();
                        DBDataGenLibrary.TeamGenerator.Generate(conn, ref teamIds, divisionIds[i], leaderIds, viceLeaderIds, gender, startDate);

                        personIds.Clear();
                        scoutIds.Clear();
                        GenScouts(conn, scoutCount, ref personIds, ref scoutIds, discoveryMinBirthday, discoveryMaxBirthday, startDate, gender, teamIds[0]);
                        // awards and pause to generate extra rows in other tables
                        GiveAwards(conn, awardLists[1], reqLists[1], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today, teamIds[1]);
                    }
                    continue;
                case 1: // adventure
                    for (int j = 0; j < teamCount; j++)
                    {
                        gender = ((j % 2) == 0) ? 'M' : 'F';
                        leaderIds.Clear();
                        viceLeaderIds.Clear();
                        // leaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // viceleaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref viceLeaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // Generate teams
                        teamIds.Clear();
                        DBDataGenLibrary.TeamGenerator.Generate(conn, ref teamIds, divisionIds[i], leaderIds, viceLeaderIds, gender, startDate);

                        personIds.Clear();
                        scoutIds.Clear();
                        GenScouts(conn, scoutCount, ref personIds, ref scoutIds, adventureMinBirthday, adventureMaxBirthday, startDate, gender, teamIds[0]);
                        // awards and pause to generate extra rows in other tables
                        GiveAwards(conn, awardLists[2], reqLists[2], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today, teamIds[1]);
                    }
                    continue;
                case 2: // expedition
                    for (int j = 0; j < teamCount; j++)
                    {
                        gender = ((j % 2) == 0) ? 'M' : 'F';
                        leaderIds.Clear();
                        viceLeaderIds.Clear();
                        // leaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref leaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // viceleaders
                        personIds.Clear();
                        GenLeaders(conn, 2, ref personIds, ref viceLeaderIds, leaderMinBD, leaderMaxBD, startDate);
                        GiveAwards(conn, awardLists[0], reqLists[0], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today);
                        // Generate teams
                        teamIds.Clear();
                        DBDataGenLibrary.TeamGenerator.Generate(conn, ref teamIds, divisionIds[i], leaderIds, viceLeaderIds, gender, startDate);

                        personIds.Clear();
                        scoutIds.Clear();
                        GenScouts(conn, scoutCount, ref personIds, ref scoutIds, expeditionMinBirthday, expeditionMaxBirthday, startDate, gender, teamIds[0]);
                        // awards and pause to generate extra rows in other tables
                        GiveAwards(conn, awardLists[3], reqLists[3], personIds, startDate, today, certifier);
                        DBDataGenLibrary.PersonGenerator.Pause(conn, personIds, startDate, today, teamIds[1]);
                    }
                    continue;
                default:
                    break;
                }
            }
        }

        // Generates the given number of offices with the given numbers to make it up
        // Assumes that awards, their requirements, and division_categories are already generated
        static void GenerateWithProgress(NpgsqlConnection conn, List<long> awardList, List<long> reqList, List<long> divCatList, int officeCount = 40, int outpostCount = 50, int teamCount = 10, int scoutCount = 5)
        {
            Console.Write("Generating data... ");
            using (var progress = new ProgressBar()) {
                for (int i = 1; i <= officeCount; i++) {
                    GenerateOffice(conn, progress, (double) i / officeCount, awardList, reqList, divCatList, outpostCount, teamCount, scoutCount);
                    // Thread.Sleep(20);
                }
            }
            Console.WriteLine("Done!");
        }

        static void Main(string[] args)
        {
            // Open the connection
            var connString = "Host=localhost;Username=postgres;Password=password;Database=ch7datagen";
            using var conn = new NpgsqlConnection(connString);
            conn.Open();

            // Create a command variable
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;

            // Lists of ids
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
            var teamIds = new List<long>();

            var auxList = new List<long>();
            var auxList2 = new List<long>();

            // Useful variables
            int i, j;
            int awardCount = 4, reqCount = 2;
            var leaderMinBD = new DateTime(1971, 1, 1);
            var leaderMaxBD = new DateTime(2003, 12, 31);
            var startDate = new DateTime(2020, 1, 1);

            // Clear database for testing
            // cmd.CommandText = "SELECT * FROM clearall()";
            // System.Console.WriteLine("Clearing DB... {0}", cmd.ExecuteScalar().ToString());

            // Categories and awards with their requirements
            System.Console.Write("Generating constant data... ");
            divisionCategoryNames.Add("Discovery");
            divisionCategoryNames.Add("Adventure");
            divisionCategoryNames.Add("Expedition");
            divisionCategoryIds.Add(0); // leaders
            DBDataGenLibrary.DivisionCategoryGenerator.Generate(conn, ref divisionCategoryIds, divisionCategoryNames);
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
            System.Console.WriteLine("Done!");

            GenerateWithProgress(conn, awardIds, requirementIds, divisionCategoryIds, 10);
        }
    }
}
