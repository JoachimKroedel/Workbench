using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;

using System.Data.SQLite;

namespace FillAPixRobot.Persistence
{
    public class SQLiteSensoryUnit : ISensoryUnit
    {
        private const string TABLE_NAME = "SensoryUnit";

        static private ISensoryUnit CreateSensoryUnit(SQLiteDataReader reader)
        {
            var sensoryUnit = new SQLiteSensoryUnit();
            sensoryUnit.Id = (long)reader["Id"];
            sensoryUnit.Type = (SensoryTypes)Enum.Parse(typeof(SensoryTypes), (string)reader["Type"]);
            sensoryUnit.Value = (string)reader["Value"];
            return sensoryUnit;
        }

        static public List<ISensoryUnit> LoadAll()
        {
            var result = new List<ISensoryUnit>();

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + "");
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                result.Add(CreateSensoryUnit(reader));
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            SQLiteHelper.CloseConnection();

            return result;
        }

        static public ISensoryUnit Load(long id)
        {
            ISensoryUnit result = null;

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + " WHERE Id=@Id ");
            sqlCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                result = CreateSensoryUnit(reader);
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();

            return result;
        }

        protected SQLiteSensoryUnit()
        {
        }

        public SQLiteSensoryUnit(SensoryTypes senseType, string value, bool saveable = true)
        {
            Id = -1;
            Type = senseType;
            Value = value;

            if (!saveable)
            {
                return;
            }

            // Check if entry with same properties already exists before create new one
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT Id FROM " + TABLE_NAME + " WHERE Type=@Type AND Value=@Value");
            sqlCommand.Parameters.Add(new SQLiteParameter("@Type", Type.ToString()));
            sqlCommand.Parameters.Add(new SQLiteParameter("@Value", Value));
            Id = SQLiteHelper.GetId(sqlCommand);

            if (Id < 0)
            {
                Id = SQLiteHelper.GetMaxId(TABLE_NAME);

                sqlCommand = new SQLiteCommand("INSERT INTO " + TABLE_NAME + " (Id, Type, Value) VALUES (@Id, @Type, @Value)");
                sqlCommand.Parameters.Add(new SQLiteParameter("@Id", Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@Type", Type.ToString()));
                sqlCommand.Parameters.Add(new SQLiteParameter("@Value", Value));

                sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }

            SQLiteHelper.CloseConnection();
        }

        public long Id { get; protected set; }
        public SensoryTypes Type { get; protected set; }
        public string Value { get; protected set; }
    }
}
