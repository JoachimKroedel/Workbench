using FillAPixRobot.Enums;
using FillAPixRobot.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FillAPixRobot.Persistence
{
    public class SQLitePuzzleAction : IPuzzleAction
    {
        private const string TABLE_NAME = "FillAPixAction";

        protected ActionTypes _actionType;
        protected DirectionTypes _directionType;

        static private IPuzzleAction CreateNewAction(SQLiteDataReader reader)
        {
            var action = new SQLitePuzzleAction();
            action.Id = (long)reader["Id"];
            action.ActionType = (ActionTypes)Enum.Parse(typeof(ActionTypes), (string)reader["ActionType"]);
            action.DirectionType = (DirectionTypes)Enum.Parse(typeof(DirectionTypes), (string)reader["DirectionType"]);
            return action;
        }

        static public List<IPuzzleAction> LoadAll()
        {
            var result = new List<IPuzzleAction>();

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + "");
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                result.Add(CreateNewAction(reader));
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();
            SQLiteHelper.CloseConnection();

            return result;
        }

        static public IPuzzleAction Load(long id)
        {
            IPuzzleAction result = null;

            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT * FROM " + TABLE_NAME + " WHERE Id=@Id ");
            sqlCommand.Parameters.Add(new SQLiteParameter("@Id", id));
            sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
            SQLiteDataReader reader = sqlCommand.ExecuteReader();
            if (reader.Read())
            {
                result = CreateNewAction(reader);
            }
            reader.Close();
            reader.Dispose();
            sqlCommand.Dispose();

            return result;
        }

        protected SQLitePuzzleAction()
        {
        }

        public SQLitePuzzleAction(ActionTypes actionType, DirectionTypes directionType)
        {
            Id = -1;
            _actionType = actionType;
            _directionType = directionType;

            // Check if entry with same properties already exists before create new one
            SQLiteCommand sqlCommand = new SQLiteCommand("SELECT Id FROM " + TABLE_NAME + " WHERE ActionType=@ActionType AND DirectionType=@DirectionType");
            sqlCommand.Parameters.Add(new SQLiteParameter("@ActionType", ActionType.ToString()));
            sqlCommand.Parameters.Add(new SQLiteParameter("@DirectionType", DirectionType.ToString()));
            Id = SQLiteHelper.GetId(sqlCommand);

            if (Id < 0)
            {
                Id = SQLiteHelper.GetMaxId(TABLE_NAME);

                sqlCommand = new SQLiteCommand("INSERT INTO " + TABLE_NAME + " (Id, ActionType, DirectionType) VALUES (@Id, @ActionType, @DirectionType)");
                sqlCommand.Parameters.Add(new SQLiteParameter("@Id", Id));
                sqlCommand.Parameters.Add(new SQLiteParameter("@ActionType", ActionType.ToString()));
                sqlCommand.Parameters.Add(new SQLiteParameter("@DirectionType", DirectionType.ToString()));

                sqlCommand.Connection = SQLiteHelper.GetOpenConnection();
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Dispose();
            }

            SQLiteHelper.CloseConnection();
        }

        public long Id { get; protected set; }
        public Enum ActionType
        {
            get { return _actionType; }

            set
            {
                if (value is ActionTypes fillAPixActionType)
                {
                    _actionType = fillAPixActionType;
                }
            }
        }

        public Enum DirectionType
        {
            get { return _directionType; }

            set
            {
                if (value is DirectionTypes directionType)
                {
                    _directionType = directionType;
                }
            }
        }
    }
}
