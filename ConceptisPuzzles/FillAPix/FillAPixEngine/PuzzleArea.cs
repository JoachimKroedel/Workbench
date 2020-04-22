using System;
using System.Collections.Generic;
using System.Drawing;

namespace FillAPixEngine
{
    public class PuzzleArea
    {
        #region ====================== Static =========================

        static public List<Point> GetNeigbours(PuzzleBoard board, Point center)
        {
            List<Point> result = new List<Point>();
            for (int y = -2; y < 3; y++)
            {
                for (int x = -2; x < 3; x++)
                {
                    if (y == 0 && x == 0)
                    {
                        continue;
                    }
                    Point pos = new Point(center.X + x, center.Y + y);
                    if (board.GetValue(pos) < 0 || board.GetValue(pos) > 9)
                    {
                        continue;
                    }
                    result.Add(pos);
                }
            }
            return result;
        }

        static public List<PuzzleArea> CleanUp(List<PuzzleArea> areas)
        {
            List<PuzzleArea> result = new List<PuzzleArea>();
            foreach (PuzzleArea area in areas)
            {
                if (area == null || area.Positions.Count < 2 || area.UndefinedCount < 1)
                {
                    continue;
                }
                result.Add(area);
            }
            return result;
        }

        static public List<PuzzleArea> Split(List<PuzzleArea> areas)
        {
            List<int> indexes = new List<int>();
            List<PuzzleArea> splittedAreas = new List<PuzzleArea>();
            for (int i = 0; i < areas.Count; i++)
            {
                PuzzleArea areaA = areas[i];
                bool foundSplit = false;
                for (int j = i + 1; j < areas.Count; j++)
                {
                    PuzzleArea areaB = areas[j];
                    if (HasOverlap(areaA, areaB) && !areaA.PositionsEquals(areaB))
                    {
                        PuzzleArea overlapA = Overlap(areaA, areaB);
                        PuzzleArea overlapB = Overlap(areaB, areaA);
                        PuzzleArea overlapAB = Join(overlapA, overlapB);
                        PuzzleArea leftA = Subset(areaA, overlapAB);
                        PuzzleArea rightB = Subset(areaB, overlapAB);
                        if (!indexes.Contains(i))
                        {
                            indexes.Add(i);
                        }
                        if (!indexes.Contains(j))
                        {
                            indexes.Add(j);
                        }

                        if (leftA != null && leftA.Positions.Count > 1)
                        {
                            splittedAreas.Add(leftA);
                            foundSplit = true;
                        }
                        if (overlapAB != null && overlapAB.Positions.Count > 1)
                        {
                            splittedAreas.Add(overlapAB);
                            foundSplit = true;
                        }
                        if (rightB != null && rightB.Positions.Count > 1)
                        {
                            splittedAreas.Add(rightB);
                            foundSplit = true;
                        }
                    }
                }
                if (!foundSplit)
                {
                    splittedAreas.Add(areaA);
                }
            }
            List<PuzzleArea> joindAreas = CleanUp(Join(splittedAreas));
            return CleanUp(joindAreas);
        }

        static public PuzzleArea Clone(PuzzleArea original)
        {
            PuzzleArea result = new PuzzleArea(original.Board);
            foreach (Point pos in original.Positions)
            {
                result.AddPosition(pos);
            }
            return result;
        }


        static public PuzzleArea Subset(PuzzleArea areaA, PuzzleArea areaB)
        {
            PuzzleArea result = Differenz(areaA, areaB);
            if (result != null)
            {
                result.MaxFilled = Math.Min(areaA.MinFilled - areaB.MinFilled, result.Positions.Count);
                result.MinFilled = Math.Min(areaA.MaxFilled - areaB.MaxFilled, result.Positions.Count);

                result.MaxEmpty = Math.Min(areaA.MinEmpty - areaB.MinEmpty, result.Positions.Count);
                result.MinEmpty = Math.Min(areaA.MaxEmpty - areaB.MaxEmpty, result.Positions.Count);

                result.RefreshDistincts();
                foreach (Point pos in areaA.InvolvedNumberPositions)
                {
                    if (!result.InvolvedNumberPositions.Contains(pos))
                    {
                        result.InvolvedNumberPositions.Add(pos);
                    }
                }
                foreach (Point pos in areaB.InvolvedNumberPositions)
                {
                    if (!result.InvolvedNumberPositions.Contains(pos))
                    {
                        result.InvolvedNumberPositions.Add(pos);
                    }
                }
            }
            return result;
        }

        static public List<PuzzleArea> Join(List<PuzzleArea> areas)
        {
            List<PuzzleArea> result = new List<PuzzleArea>();
            result.AddRange(areas);
            for (int i = 0; i < result.Count; i++)
            {
                PuzzleArea areaA = result[i];
                int j = i + 1;
                while (j < result.Count)
                {
                    PuzzleArea areaB = result[j];
                    if (areaA.PositionsEquals(areaB))
                    {
                        result[i] = Join(areaA, areaB);
                        result.RemoveAt(j);
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            return result;
        }

        static public PuzzleArea Join(PuzzleArea areaA, PuzzleArea areaB)
        {
            // Sicherstellen, dass beide Mengen gleich groß sind!!!
            if (!areaA.PositionsEquals(areaB))
            {
                return null;
            }
            PuzzleArea result = Clone(areaA);

            result.MinFilled = Math.Max(areaA.MinFilled, areaB.MinFilled);
            result.MaxFilled = Math.Min(areaA.MaxFilled, areaB.MaxFilled);
            result.MinEmpty = Math.Max(areaA.MinEmpty, areaB.MinEmpty);
            result.MaxEmpty = Math.Min(areaA.MaxEmpty, areaB.MaxEmpty);

            result.RefreshDistincts();
            foreach (Point pos in areaA.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            foreach (Point pos in areaB.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            return result;
        }

        static public bool HasOverlap(PuzzleArea areaA, PuzzleArea areaB)
        {
            foreach (Point pos in areaA.Positions)
            {
                if (areaB.Positions.Contains(pos))
                {
                    return true;
                }
            }
            return false;
        }

        static public PuzzleArea Overlap(PuzzleArea areaA, PuzzleArea areaB)
        {
            PuzzleArea result = new PuzzleArea(areaA.Board);
            PuzzleArea leftAreaA = new PuzzleArea(areaA.Board);
            foreach (Point pos in areaA.Positions)
            {
                if (areaB.Positions.Contains(pos))
                {
                    result.AddPosition(pos);
                }
                else
                {
                    leftAreaA.AddPosition(pos);
                }
            }
            if (result.Positions.Count == 0)
            {
                return null;
            }
            result.MinFilled = Math.Max(0, areaA.MinFilled - leftAreaA.UndefinedCount - leftAreaA.FilledCount);
            result.MaxFilled = Math.Min(areaA.MaxFilled, result.UndefinedCount + result.FilledCount);

            result.MinEmpty = Math.Max(0, areaA.MinEmpty - leftAreaA.UndefinedCount - leftAreaA.EmptyCount);
            result.MaxEmpty = Math.Min(areaA.MaxEmpty, result.UndefinedCount + result.EmptyCount);

            result.RefreshDistincts();
            foreach (Point pos in areaA.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            foreach (Point pos in areaB.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            return result;
        }

        static public PuzzleArea Differenz(PuzzleArea areaA, PuzzleArea areaB)
        {
            PuzzleArea result = new PuzzleArea(areaA.Board);
            PuzzleArea overlap = Overlap(areaA, areaB);
            foreach (Point pos in areaA.Positions)
            {
                if (!areaB.Positions.Contains(pos))
                {
                    result.AddPosition(pos);
                }
            }

            PuzzleArea areaRightB = new PuzzleArea(areaB.Board);
            foreach (Point pos in areaB.Positions)
            {
                if (!areaA.Positions.Contains(pos))
                {
                    areaRightB.AddPosition(pos);
                }
            }

            if (result.Positions.Count == 0)
            {
                return null;
            }

            result.MinFilled = Math.Max(result.Positions.Count, areaA.MinFilled - overlap.MaxFilled - areaRightB.MinFilled);
            result.MaxFilled = Math.Min(result.Positions.Count, areaA.MaxFilled - overlap.MinFilled - areaRightB.MaxFilled);

            result.MinEmpty = Math.Max(result.Positions.Count, areaA.MinEmpty - overlap.MaxEmpty - areaRightB.MinEmpty);
            result.MaxEmpty = Math.Min(result.Positions.Count, areaA.MaxEmpty - overlap.MinEmpty - areaRightB.MaxEmpty);

            result.RefreshDistincts();
            foreach (Point pos in areaA.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            foreach (Point pos in areaB.InvolvedNumberPositions)
            {
                if (!result.InvolvedNumberPositions.Contains(pos))
                {
                    result.InvolvedNumberPositions.Add(pos);
                }
            }
            return result;
        }

        #endregion ====================== Static =========================

        private readonly List<Point> _distinctFilledPositions = new List<Point>();
        private readonly List<Point> _distinctEmptyPositions = new List<Point>();
        private readonly List<Point> _involvedNumberPositions = new List<Point>();

        private PuzzleArea(PuzzleBoard board)
        {
            Board = board;
            EmptyCount = 0;
            FilledCount = 0;
            UndefinedCount = 0;
            Positions = new List<Point>();
        }

        public PuzzleArea(PuzzleBoard board, Point center) : this(board)
        {
            for (int y = -1; y < 2; y++)
            {
                int posY = center.Y + y;
                if (posY < 0 || posY >= board.Rows)
                {
                    continue;
                }
                for (int x = -1; x < 2; x++)
                {
                    int posX = center.X + x;
                    if (posX < 0 || posX >= board.Columns)
                    {
                        continue;
                    }
                    AddPosition(new Point(posX, posY));
                }
            }
            int value = board.GetValue(center);
            if (value < 0 || value > 9)
            {
                MaxFilled = 9 - EmptyCount;
                MinFilled = FilledCount;
                MaxEmpty = 9 - FilledCount;
                MinEmpty = EmptyCount;
            }
            else
            {
                _involvedNumberPositions.Add(center);
                MaxFilled = value;
                MinFilled = MaxFilled;
                MaxEmpty = Positions.Count - value;
                MinEmpty = MaxEmpty;
            }
            RefreshDistincts();
        }

        public int MaxFilled { get; private set; }
        public int MaxEmpty { get; private set; }
        public int MinFilled { get; private set; }
        public int MinEmpty { get; private set; }

        public int EmptyCount { get; private set; }
        public int FilledCount { get; private set; }
        public int UndefinedCount { get; private set; }

        public PuzzleBoard Board { get; private set; }

        public List<Point> Positions { get; private set; }

        public List<Point> DistinctEmptyPositions { get { return _distinctEmptyPositions; } }

        public List<Point> DistinctFilledPositions { get { return _distinctFilledPositions; } }

        public List<Point> InvolvedNumberPositions { get { return _involvedNumberPositions; } }

        public void AddPosition(Point pos)
        {
            Positions.Add(pos);
            if (Board.GetState(pos) == PuzzleCellStateTypes.NotMarked)
            {
                UndefinedCount++;
            }
            else if (Board.GetState(pos) == PuzzleCellStateTypes.Filled)
            {
                FilledCount++;
            }
            else if (Board.GetState(pos) == PuzzleCellStateTypes.Empty || Board.GetState(pos) == PuzzleCellStateTypes.Outside)
            {
                EmptyCount++;
            }
        }

        public bool PositionsEquals(PuzzleArea compare)
        {
            if (Positions.Count != compare.Positions.Count)
            {
                return false;
            }
            foreach (Point pos in Positions)
            {
                if (!compare.Positions.Contains(pos))
                {
                    return false;
                }
            }
            return true;
        }

        private void RefreshDistincts()
        {
            _distinctEmptyPositions.Clear();
            _distinctFilledPositions.Clear();
            if (UndefinedCount > 0)
            {
                // Wenn bereits die maximale Füllung erreicht ist kann der Rest mit Leere aufgefüllt werden
                if (MaxFilled - FilledCount == 0)
                {
                    foreach (Point pos in Positions)
                    {
                        if (Board.GetState(pos) == PuzzleCellStateTypes.NotMarked)
                        {
                            _distinctEmptyPositions.Add(pos);
                        }
                    }
                }
                // Wenn bereits die maximale Leere erreicht ist kann der Rest mit Gefüllt aufgefüllt werden ;o))
                if (MaxEmpty - EmptyCount == 0)
                {
                    foreach (Point pos in Positions)
                    {
                        if (Board.GetState(pos) == PuzzleCellStateTypes.NotMarked)
                        {
                            _distinctFilledPositions.Add(pos);
                        }
                    }
                }
            }
        }

        public PuzzleArea Clone()
        {
            return Clone(this);
        }

        public override string ToString()
        {
            string result = string.Format("{5} [{2}] {6} | {3} [{1}] {4} | [{0}]", UndefinedCount, EmptyCount, FilledCount, MinEmpty, MaxEmpty, MinFilled, MaxFilled);
            return result;
        }
    }
}
