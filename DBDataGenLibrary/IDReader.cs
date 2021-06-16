using System;
using Npgsql;
using System.Collections.Generic;

namespace DBDataGenLibrary
{
    public class IDReader
    {
        public static void Read(NpgsqlDataReader reader, ref List<long> dest)
        {
            while (reader.Read()) dest.Add(reader.GetInt64(0));
            reader.Close();
        }
    }
}