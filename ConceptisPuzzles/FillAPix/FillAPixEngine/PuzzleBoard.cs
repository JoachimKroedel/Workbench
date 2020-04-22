using System;
using System.Collections.Generic;
using System.Drawing;

namespace FillAPixEngine
{
    public class PuzzleBoard
    {

        private readonly List<List<Tuple<Point, PuzzleCellStateTypes>>> _history = new List<List<Tuple<Point, PuzzleCellStateTypes>>>();
        private readonly List<List<Tuple<Point, PuzzleCellStateTypes>>> _redos = new List<List<Tuple<Point, PuzzleCellStateTypes>>>();

        private int _notMarkedCellsCount = 0;
        private PuzzleCell[,] _playGround;

        public PuzzleBoard(string fileName)
        {
            if (!LoadPuzzle(fileName))
            {
                CreateEmptyBoard(1, 1);
            }
            else
            {
                FileName = fileName;
            }
        }

        public PuzzleBoard(int columns, int rows)
        {
            CreateEmptyBoard(columns, rows);
        }

        public List<Point> NumberPositions
        {
            get
            {
                List<Point> result = new List<Point>();
                for (int y = 0; y < Rows; y++)
                {
                    for (int x = 0; x < Columns; x++)
                    {
                        Point pos = new Point(x, y);
                        if (GetValue(pos) >= 0 && GetValue(pos) <= 9)
                        {
                            result.Add(pos);
                        }
                    }
                }
                return result;
            }
        }

        public int Columns { get; private set; }

        public int Rows { get; private set; }

        public List<Point> FillUndefined(Point cellPos)
        {
            List<Point> result = new List<Point>();
            if (cellPos.X < 0 || cellPos.X >= Columns || cellPos.Y < 0 || cellPos.Y >= Rows)
            {
                return result;
            }
            int countFilled = 0;
            int shouldFilled = GetValue(cellPos);
            foreach (PuzzleCell cell in GetArea(cellPos))
            {
                if (cell.State == PuzzleCellStateTypes.Filled)
                {
                    countFilled++;
                }
                else if (cell.State == PuzzleCellStateTypes.NotMarked)
                {
                    result.Add(cell.Position);
                }
            }
            List<Tuple<Point, PuzzleCellStateTypes>> values = new List<Tuple<Point, PuzzleCellStateTypes>>();
            foreach (Point pos in result)
            {
                if (shouldFilled > countFilled)
                {
                    values.Add(new Tuple<Point, PuzzleCellStateTypes>(pos, PuzzleCellStateTypes.Filled));
                    countFilled++;
                }
                else
                {
                    values.Add(new Tuple<Point, PuzzleCellStateTypes>(pos, PuzzleCellStateTypes.Empty));
                }
            }
            SetStates(values);

            return result;
        }

        public void Reset()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    _playGround[x, y].State = PuzzleCellStateTypes.NotMarked;
                }
            }
            _history.Clear();
            _redos.Clear();
            _notMarkedCellsCount = Columns * Rows;
        }

        public void TriggerNextState(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return;
            }
            PuzzleCellStateTypes state = _playGround[pos.X, pos.Y].State;
            switch (state)
            {
                case PuzzleCellStateTypes.NotMarked: SetState(pos, PuzzleCellStateTypes.Filled); break;
                case PuzzleCellStateTypes.Filled: SetState(pos, PuzzleCellStateTypes.Empty); break;
                case PuzzleCellStateTypes.Empty: SetState(pos, PuzzleCellStateTypes.NotMarked); break;
            }
        }

        public List<Point> SetStates(List<Tuple<Point, PuzzleCellStateTypes>> states, bool useUndo = true, bool clearAllRedos = true)
        {
            List<Point> result = new List<Point>();
            List<Tuple<Point, PuzzleCellStateTypes>> historyStates = new List<Tuple<Point, PuzzleCellStateTypes>>();
            foreach (Tuple<Point, PuzzleCellStateTypes> item in states)
            {
                Point pos = item.Item1;
                PuzzleCellStateTypes state = item.Item2;
                if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
                {
                    continue;
                }
                PuzzleCellStateTypes actualState = _playGround[pos.X, pos.Y].State;
                if (actualState != state)
                {
                    historyStates.Add(new Tuple<Point, PuzzleCellStateTypes>(pos, actualState));
                    _playGround[pos.X, pos.Y].State = state;
                    if (actualState == PuzzleCellStateTypes.NotMarked)
                    {
                        _notMarkedCellsCount--;
                    }
                    else if (state == PuzzleCellStateTypes.NotMarked)
                    {
                        _notMarkedCellsCount++;
                    }
                    result.Add(pos);
                }
            }
            if (useUndo && historyStates.Count > 0)
            {
                _history.Add(historyStates);
            }
            if (clearAllRedos)
            {
                _redos.Clear();
            }
            return result;
        }

        public List<Point> SetState(Point pos, PuzzleCellStateTypes state)
        {
            return SetStates(new List<Tuple<Point, PuzzleCellStateTypes>>() { new Tuple<Point, PuzzleCellStateTypes>(pos, state) });
        }

        public void SetValue(Point pos, int value)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return;
            }
            if (_playGround[pos.X, pos.Y].Value != value)
            {
                _playGround[pos.X, pos.Y].Value = value;
            }
        }

        public PuzzleCellStateTypes GetState(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return PuzzleCellStateTypes.Outside;
            }
            return _playGround[pos.X, pos.Y].State;
        }

        public int GetValue(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return -1;
            }
            return _playGround[pos.X, pos.Y].Value;
        }

        public List<Point> Undo()
        {
            List<Point> result = new List<Point>();
            if (_history.Count > 0)
            {
                List<Tuple<Point, PuzzleCellStateTypes>> states = _history[_history.Count - 1];
                List<Tuple<Point, PuzzleCellStateTypes>> redoStates = new List<Tuple<Point, PuzzleCellStateTypes>>();
                foreach (Tuple<Point, PuzzleCellStateTypes> item in states)
                {
                    Point pos = item.Item1;
                    PuzzleCellStateTypes state = item.Item2;
                    if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
                    {
                        continue;
                    }
                    PuzzleCellStateTypes actualState = _playGround[pos.X, pos.Y].State;
                    if (actualState != state)
                    {
                        redoStates.Add(new Tuple<Point, PuzzleCellStateTypes>(pos, actualState));
                    }
                }
                if (redoStates.Count > 0)
                {
                    _redos.Add(redoStates);
                }
                result.AddRange(SetStates(states, false, false));
                _history.RemoveAt(_history.Count - 1);
            }
            return result;
        }

        public List<Point> Redo()
        {
            List<Point> result = new List<Point>();
            if (_redos.Count > 0)
            {
                List<Tuple<Point, PuzzleCellStateTypes>> states = _redos[_redos.Count - 1];
                _redos.RemoveAt(_redos.Count - 1);
                result.AddRange(SetStates(states, true, false));
            }
            return result;
        }

        public string FileName { get; private set; }

        public DateTime Date { get; private set; }

        public DateTime SaveDate { get; private set; }

        public int Index { get; private set; }

        public string Level { get; private set; }

        public PuzzleStateTypes State { get; private set; }

        public Size Size { get; private set; }

        public bool SavePuzzle()
        {
            if (string.IsNullOrEmpty(FileName))
            {
                return false;
            }
            return SavePuzzle(FileName, Date, Index, Level, IsComplete() && !IsWrong() ? PuzzleStateTypes.Done : PuzzleStateTypes.Saved);
        }

        public bool SavePuzzle(string fileName, DateTime date, int index, string level, PuzzleStateTypes state)
        {
            string fileHeader = Columns + ";" + Rows + ";" + date.ToShortDateString() + ";" + index + ";" + level + ";" + state + ";" + DateTime.Now + "\n";
            string playgroundDefinition = ToString();
            string states = "\n";
            if (state != PuzzleStateTypes.New)
            {
                for (int y = 0; y < Rows; y++)
                {
                    for (int x = 0; x < Columns; x++)
                    {
                        switch (_playGround[x, y].State)
                        {
                            case PuzzleCellStateTypes.NotMarked: states += "?"; break;
                            case PuzzleCellStateTypes.Empty: states += " "; break;
                            case PuzzleCellStateTypes.Filled: states += "X"; break;
                            case PuzzleCellStateTypes.Outside: states += "O"; break;
                        }
                    }
                    states += "\n";
                }
            }
            System.IO.File.WriteAllText(fileName, fileHeader + playgroundDefinition + states);
            return true;
        }

        public bool LoadPuzzle(string fileName)
        {
            int maxY = 0;
            List<string> lines = new List<string>();

            // Read the file and display it line by line.  
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            string line;
            int maxX = 0;
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
                if (maxX < line.Length)
                {
                    maxX = line.Length;
                }
                maxY++;
            }
            file.Close();

            if (fileName.EndsWith(".fap"))
            {
                string[] splitInfo = lines[0].Split(new char[] { ';' });
                if (splitInfo.Length >= 6)
                {
                    maxX = int.Parse(splitInfo[0]);
                    maxY = int.Parse(splitInfo[1]);
                    Size = new Size(maxX, maxY);
                    Date = DateTime.Parse(splitInfo[2]);
                    Index = int.Parse(splitInfo[3]);
                    Level = splitInfo[4];
                    State = (PuzzleStateTypes)Enum.Parse(typeof(PuzzleStateTypes), splitInfo[5]);
                    SaveDate = splitInfo.Length >= 7 ? DateTime.Parse(splitInfo[6]) : DateTime.MinValue;
                    lines.RemoveAt(0);
                }
            }
            else
            {
                Size = new Size(maxX, maxY);
                Date = DateTime.Now;
                Index = -1;
                Level = "<undefined>";
                State = PuzzleStateTypes.New;
            }

            CreateEmptyBoard(maxX, maxY);
            Columns = maxX;
            Rows = maxY;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if ((char)lines[y].Length > x)
                    {
                        int ascii = lines[y][x];
                        _playGround[x, y].Value = ascii - 48;
                        _playGround[x, y].State = PuzzleCellStateTypes.NotMarked;
                    }
                }
            }
            if (State != PuzzleStateTypes.New)
            {
                for (int y = maxY; y < lines.Count && y < 2 * maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        Point pos = new Point(x, y - maxY);
                        if ((char)lines[y].Length > x)
                        {
                            switch (lines[y][x])
                            {
                                case '?': SetState(pos, PuzzleCellStateTypes.NotMarked); break;
                                case ' ': SetState(pos, PuzzleCellStateTypes.Empty); break;
                                case 'X': SetState(pos, PuzzleCellStateTypes.Filled); break;
                                case 'O': SetState(pos, PuzzleCellStateTypes.Outside); break;
                            }
                        }
                    }
                }
            }
            _history.Clear();
            return true;
        }

        public List<Point> GetPosibleCandidates()
        {
            List<Point> result = new List<Point>();
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    Point pos = new Point(x, y);
                    if (IsReadyToFill(pos) || IsReadyToSpace(pos))
                    {
                        result.Add(pos);
                    }
                }
            }
            return result;
        }

        public bool IsReadyToFill(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return false;
            }
            if (_playGround[pos.X, pos.Y].Value < 0 || _playGround[pos.X, pos.Y].Value > 9)
            {
                return false;
            }
            int count = 0;
            int maxCount = 0;
            int countUndefined = 0;
            foreach (PuzzleCell cell in GetArea(pos))
            {
                maxCount++;
                if (cell.State == PuzzleCellStateTypes.Empty || cell.State == PuzzleCellStateTypes.Outside)
                {
                    count++;
                }
                else if (cell.State == PuzzleCellStateTypes.NotMarked)
                {
                    countUndefined++;
                }
            }
            return countUndefined > 0 && _playGround[pos.X, pos.Y].Value + count == maxCount;
        }

        public bool IsReadyToSpace(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return false;
            }
            if (_playGround[pos.X, pos.Y].Value < 0 || _playGround[pos.X, pos.Y].Value > 9)
            {
                return false;
            }
            int count = 0;
            int countUndefined = 0;
            foreach (PuzzleCell cell in GetArea(pos))
            {
                if (cell.State == PuzzleCellStateTypes.Filled)
                {
                    count++;
                }
                else if (cell.State == PuzzleCellStateTypes.NotMarked)
                {
                    countUndefined++;
                }
            }
            return countUndefined > 0 && _playGround[pos.X, pos.Y].Value - count == 0;
        }

        public bool IsWrong()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (IsWrong(new Point(x, y)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsWrong(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return false;
            }
            if (_playGround[pos.X, pos.Y].Value < 0 || _playGround[pos.X, pos.Y].Value > 9)
            {
                return false;
            }
            int countFilled = 0;
            int countEmpty = 0;
            int countNotMarked = 0;
            foreach (PuzzleCell cell in GetArea(pos))
            {
                if (cell.State == PuzzleCellStateTypes.NotMarked)
                {
                    countNotMarked++;
                }
                if (cell.State == PuzzleCellStateTypes.Filled)
                {
                    countFilled++;
                }
                else if (cell.State == PuzzleCellStateTypes.Empty || cell.State == PuzzleCellStateTypes.Outside)
                {
                    countEmpty++;
                }
            }
            if (countFilled == 0 && countEmpty == 0)
            {
                return false;
            }
            if (countNotMarked > 0)
            {
                return _playGround[pos.X, pos.Y].Value < countFilled || _playGround[pos.X, pos.Y].Value > 9 - countEmpty;
            }
            else
            {
                return _playGround[pos.X, pos.Y].Value != countFilled;
            }
        }

        public bool IsComplete()
        {
            return _notMarkedCellsCount == 0;
        }

        public bool IsComplete(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return false;
            }
            foreach (PuzzleCell cell in GetArea(pos))
            {
                if (cell.State == PuzzleCellStateTypes.NotMarked)
                {
                    return false;
                }
            }
            return true;
        }

        private List<Point> GetAreaPositions(Point pos)
        {
            if (pos.X < 0 || pos.X >= Columns || pos.Y < 0 || pos.Y >= Rows)
            {
                return new List<Point>();
            }
            int x1 = pos.X > 0 ? pos.X - 1 : 0;
            int y1 = pos.Y > 0 ? pos.Y - 1 : 0;
            int maxX = pos.X + 1;
            int maxY = pos.Y + 1;
            if (maxX >= Columns) maxX = Columns - 1;
            if (maxY >= Rows) maxY = Rows - 1;

            List<Point> result = new List<Point>();
            for (int y = y1; y <= maxY; y++)
            {
                for (int x = x1; x <= maxX; x++)
                {
                    result.Add(new Point(x, y));
                }
            }
            return result;
        }

        private List<PuzzleCell> GetArea(Point pos)
        {
            List<PuzzleCell> result = new List<PuzzleCell>();
            foreach (Point neighbour in GetAreaPositions(pos))
            {
                result.Add(_playGround[neighbour.X, neighbour.Y]);
            }
            return result;
        }

        private void CreateEmptyBoard(int columns, int rows)
        {
            _playGround = new PuzzleCell[columns, rows];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    _playGround[x, y] = new PuzzleCell(new Point(x, y));
                }
            }
            Columns = columns;
            Rows = rows;
            _notMarkedCellsCount = Columns * Rows;
        }

        public override string ToString()
        {
            string result = "";
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int value = _playGround[x, y].Value;
                    if (value >= 0 && value <= 9)
                    {
                        result += value.ToString("d0");
                    }
                    else
                    {
                        result += " ";
                    }
                }
                result += "\n";
            }
            return result.Substring(0, result.Length - 1);
        }
    }
}
