using System;
using Npgsql;

namespace DBDataGenLibrary
{
    public class DateGenerator
    {
        public static NpgsqlTypes.NpgsqlDate Generate(DateTime minDate, DateTime maxDate)
        {
            var rand = new Random();

            DateTime start = new DateTime(1995, 1, 1);
            int range = (maxDate - minDate).Days;           
            return new NpgsqlTypes.NpgsqlDate(minDate.AddDays(rand.Next(range)));
        }
    }
}
