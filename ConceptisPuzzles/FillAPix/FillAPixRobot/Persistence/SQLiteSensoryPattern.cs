using System.Collections.Generic;
using System.Linq;

using System.Data.SQLite;
using FillAPixRobot.Interfaces;
using FillAPixRobot.Enums;

namespace FillAPixRobot.Persistence
{
    public class SQLiteSensoryPattern : ISensoryPattern
    {
        private const string TABLE_NAME = "SensoryPattern";

        static private ISensoryPattern CreateSensoryPattern(SQLiteDataReader reader)
        {
            var sensoryPattern = new SQLiteSensoryPattern();
            sensoryPattern.Id = (long)reader["Id"];
            string listOfUnitIds = (string)reader["SensoryUnitIds"];
            foreach (string unitId in listOfUnitIds.Split(';'))
            {
                long sensoryUnitId = long.Parse(unitId);
                var sensoryUnit = SQLiteSensoryUnit.Load(sensoryUnitId);
                sensoryPattern.SensoryUnits.Add(sensoryUnit);
            }
            return sensoryPattern;
        }

        static public List<ISensoryPattern> LoadAll()
        {
            var result = new List<ISensoryPattern>();

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + "");
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                result.Add(CreateSensoryPattern(reader));
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            SQLiteHelper.CloseConnection();

            return result;
        }

        static public ISensoryPattern Load(long id)
        {
            ISensoryPattern result = null;

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + " WHERE Id=@Id ");
            sqlCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                result = CreateSensoryPattern(reader);
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();

            return result;
        }

        protected SQLiteSensoryPattern()
        {
            SensoryUnits = new List<ISensoryUnit>();
        }

        public SQLiteSensoryPattern(DirectionTypes directionType, List<ISensoryUnit> sensoryUnits, bool saveable = true)
            : this()
        {
            Id = -1;
            DirectionType = directionType;
            SensoryUnits.AddRange(sensoryUnits);

            if (!saveable)
            {
                return;
            }

            string sensoryUnitIds = SQLiteHelper.CreateIdsText(sensoryUnits.Select(u => u.Id));
            // Check if entry with same properties already exists before create new one
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT Id FROM " + TABLE_NAME + " WHERE SensoryUnitIds=@sensoryUnitIds");
            sqlCommand.Parameters.Add(new SQLiteParameter("@SensoryUnitIds", sensoryUnitIds));
            Id = SQLiteHelper.GetId(sqlCommand);

            if (Id < 0)
            {
                Id = SQLiteHelper.GetMaxId(TABLE_NAME);

                sqlCommand = new SQLiteCommand("INSERT INTO " + TABLE_NAME + " (Id, SensoryUnitIds) VALUES (@Id, @SensoryUnitIds)");
                sqlCommand.Parameters.Add(new SQLiteParameter("@Id", Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@SensoryUnitIds", sensoryUnitIds));

                sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }

            SQLiteHelper.CloseConnection();
        }

        public long Id { get; protected set; }
        public DirectionTypes DirectionType { get; set; }
        public List<ISensoryUnit> SensoryUnits { get; protected set; }
    }
}
