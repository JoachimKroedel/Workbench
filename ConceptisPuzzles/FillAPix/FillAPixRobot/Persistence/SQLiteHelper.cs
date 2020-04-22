using System.Collections.Generic;

using System.Data.SQLite;

namespace FillAPixRobot.Persistence
{
    public static class SQLiteHelper
    {
        // ToDo: Make this hard coded db-filename more dynamic ... maybe add to config-file
        private const string DATA_SOURCE_FILE_NAME = "FillAPixRobotKnowledgeBase.db";

        private static SQLiteConnection _connection = null;

        public static SQLiteConnection GetOpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SQLiteConnection();
                _connection.ConnectionString = "Data Source=" + DATA_SOURCE_FILE_NAME;
                _connection.Open();
            }
            return _connection;
        }

        public static void CloseConnection()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }

        public static long GetMaxId(string tableName)
        {
            long maxId = 0;
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT MAX(Id) FROM " + tableName + " ");
            sqlCommand.Connection = GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                object dbValue = reader[0];
                if (dbValue is long)
                {
                    maxId = (long)dbValue + 1;
                }
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            return maxId;
        }

        public static long GetId(SQLiteCommand sqlCommand)
        {
            long result = -1;
            sqlCommand.Connection = GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                object dbValue = reader["Id"];
                if (dbValue is long)
                {
                    result = (long)dbValue;
                }
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            return result;
        }

        public static string CreateIdsText(IEnumerable<long> ids)
        {
            string result = "";

            var sortedPatternIds = new List<long>();
            sortedPatternIds.AddRange(ids);
            sortedPatternIds.Sort();

            foreach (long id in sortedPatternIds)
            {
                result += ";" + id;
            }
            if (result.Length > 0)
            {
                result = result.Substring(1);
            }

            return result;
        }
    }
}
