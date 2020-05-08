using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SQLite;

using FillAPixRobot.Interfaces;
using FillAPixRobot.Enums;

namespace FillAPixRobot.Persistence
{
    public class SQLiteSensationSnapshot : ISensationSnapshot
    {
        private const string TABLE_NAME = "SensationSnapshot";

        static private ISensationSnapshot CreateSensationSnapshot(SQLiteDataReader reader)
        {
            var sensationSnapshot = new SQLiteSensationSnapshot();
            sensationSnapshot.Id = (long)reader["Id"];
            sensationSnapshot.FieldOfVision = (FieldOfVisionTypes)Enum.Parse(typeof(FieldOfVisionTypes), (string)reader["FieldOfVisionType"]);
            string listOfPatternIds = (string)reader["SensoryPatternIds"];
            foreach (string unitId in listOfPatternIds.Split(';'))
            {
                long id = long.Parse(unitId);
                var sensoryPattern = SQLiteSensoryPattern.Load(id);
                sensationSnapshot.SensoryPatterns.Add(sensoryPattern);
            }
            return sensationSnapshot;
        }

        static public List<ISensationSnapshot> LoadAll()
        {
            var result = new List<ISensationSnapshot>();

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + "");
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                result.Add(CreateSensationSnapshot(reader));
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            SQLiteHelper.CloseConnection();

            return result;
        }

        protected SQLiteSensationSnapshot()
        {
            SensoryPatterns = new List<ISensoryPattern>();
        }

        public SQLiteSensationSnapshot(DirectionTypes directionType,FieldOfVisionTypes fieldOfVisionType, List<ISensoryPattern> sensoryPatterns, bool saveable = true)
            : this()
        {
            Id = -1;
            Direction = directionType;
            FieldOfVision = fieldOfVisionType;
            SensoryPatterns.AddRange(sensoryPatterns);

            if (!saveable)
            {
                return;
            }

            string sensoryPatternIds = SQLiteHelper.CreateIdsText(sensoryPatterns.Select(u => u.Id));
            // Check if entry with same properties already exists before create new one
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT Id FROM " + TABLE_NAME + " WHERE FieldOfVisionType=@FieldOfVisionType AND SensoryPatternIds=@sensoryPatternIds");
            sqlCommand.Parameters.Add(new SQLiteParameter("@FieldOfVisionType", fieldOfVisionType.ToString()));
            sqlCommand.Parameters.Add(new SQLiteParameter("@SensoryPatternIds", sensoryPatternIds));
            Id = SQLiteHelper.GetId(sqlCommand);

            if (Id < 0)
            {
                Id = SQLiteHelper.GetMaxId(TABLE_NAME);

                sqlCommand = new SQLiteCommand("INSERT INTO " + TABLE_NAME + " (Id, FieldOfVisionType, SensoryPatternIds) VALUES (@Id, @FieldOfVisionType, @SensoryPatternIds)");
                sqlCommand.Parameters.Add(new SQLiteParameter("@Id", Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@FieldOfVisionType", FieldOfVision.ToString()));
                sqlCommand.Parameters.Add(new SQLiteParameter("@SensoryPatternIds", sensoryPatternIds));

                sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }

            SQLiteHelper.CloseConnection();
        }

        public long Id { get; protected set; }

        public DirectionTypes Direction { get; set; }
        public FieldOfVisionTypes FieldOfVision { get; set; }

        public List<ISensoryPattern> SensoryPatterns { get; protected set; }
    }
}
