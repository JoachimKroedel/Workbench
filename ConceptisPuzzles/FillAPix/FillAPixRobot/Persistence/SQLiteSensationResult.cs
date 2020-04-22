using System.Collections.Generic;
using System.Linq;

using System.Data.SQLite;
using FillAPixRobot.Interfaces;

namespace FillAPixRobot.Persistence
{
    public class SQLiteSensationResult : ISensationResult
    {
        private const string TABLE_NAME = "SensationResult";

        static private ISensationResult CreateSensationResult(SQLiteDataReader reader)
        {
            var sensationResult = new SQLiteSensationResult();
            sensationResult.Id = (long)reader["Id"];
            sensationResult.SensationSnapshotBeforeId = (long)reader["SensationSnapshotBeforeId"];
            sensationResult.SensationSnapshotAfterId = (long)reader["SensationSnapshotAfterId"];
            sensationResult.ActionId = (long)reader["ActionId"];
            sensationResult.FeedbackValue = (long)reader["FeedbackValue"];
            return sensationResult;
        }

        static public List<ISensationResult> LoadAll()
        {
            var result = new List<ISensationResult>();

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + "");
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                result.Add(CreateSensationResult(reader));
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            SQLiteHelper.CloseConnection();

            foreach (var entry in result)
            {
                var sensationResult = entry as SQLiteSensationResult;
                if (sensationResult != null)
                {
                    sensationResult.SnapshotBefore = SensationSnapshot.SensationSnapshots.FirstOrDefault(s => s.Id == sensationResult.SensationSnapshotBeforeId);
                    sensationResult.Action = PuzzleAction.Actions.FirstOrDefault(s => s.Id == sensationResult.ActionId);
                    sensationResult.SnapshotAfter = SensationSnapshot.SensationSnapshots.FirstOrDefault(s => s.Id == sensationResult.SensationSnapshotAfterId);
                }
            }

            return result;
        }

        static public ISensationResult Load(long id)
        {
            ISensationResult result = null;

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + " WHERE Id=@Id ");
            sqlCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                result = CreateSensationResult(reader);
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();

            return result;
        }

        protected SQLiteSensationResult()
        {
        }

        public SQLiteSensationResult(ISensationSnapshot before, IPuzzleAction action, ISensationSnapshot after, long feedbackValue, bool saveable = true)
        {
            Id = -1;
            SnapshotBefore = before;
            Action = action;
            SnapshotAfter = after;
            FeedbackValue = feedbackValue;

            if (!saveable)
            {
                return;
            }

            // Check if entry with same properties already exists before create new one
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT Id FROM " + TABLE_NAME +
                " WHERE SensationSnapshotBeforeId=@SensationSnapshotBeforeId" +
                "  AND ActionId=@ActionId" +
                "  AND SensationSnapshotAfterId=@SensationSnapshotAfterId" +
                "  AND FeedbackValue=@FeedbackValue"
                );
            sqlCommand.Parameters.Add(new SQLiteParameter("@SensationSnapshotBeforeId", SnapshotBefore.Id));
            sqlCommand.Parameters.Add(new SQLiteParameter("@ActionId", Action.Id));
            sqlCommand.Parameters.Add(new SQLiteParameter("@SensationSnapshotAfterId", SnapshotAfter.Id));
            sqlCommand.Parameters.Add(new SQLiteParameter("@FeedbackValue", FeedbackValue));
            Id = SQLiteHelper.GetId(sqlCommand);

            if (Id < 0)
            {
                Id = SQLiteHelper.GetMaxId(TABLE_NAME);

                sqlCommand = new SQLiteCommand("INSERT INTO " + TABLE_NAME +
                    " (Id, SensationSnapshotBeforeId, ActionId, SensationSnapshotAfterId, FeedbackValue) VALUES " +
                    " (@Id, @SensationSnapshotBeforeId, @ActionId, @SensationSnapshotAfterId, @FeedbackValue)");
                sqlCommand.Parameters.Add(new SQLiteParameter("@Id", Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@SensationSnapshotBeforeId", SnapshotBefore.Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@ActionId", Action.Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@SensationSnapshotAfterId", SnapshotAfter.Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@FeedbackValue", FeedbackValue));

                sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }

            SQLiteHelper.CloseConnection();
        }

        public long Id { get; protected set; }
        public ISensationSnapshot SnapshotBefore { get; protected set; }
        public IPuzzleAction Action { get; protected set; }
        public ISensationSnapshot SnapshotAfter { get; protected set; }
        public long FeedbackValue { get; protected set; }

        public long SensationSnapshotBeforeId { get; private set; }
        public long ActionId { get; private set; }
        public long SensationSnapshotAfterId { get; private set; }
    }
}
