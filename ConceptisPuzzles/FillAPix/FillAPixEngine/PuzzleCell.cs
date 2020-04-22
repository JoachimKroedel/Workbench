using System.Drawing;

namespace FillAPixEngine
{
    public class PuzzleCell
    {
        private int _value;

        public PuzzleCell(Point position)
        {
            Position = position;
            Value = -1;
        }

        public int Value
        {
            get { return _value; }
            set
            {
                int checkValue = value;
                if (checkValue < 0 || checkValue > 9)
                {
                    checkValue = -1;
                }
                if (checkValue != _value)
                {
                    _value = checkValue;
                }
            }
        }

        public PuzzleCellStateTypes State { get; set; }

        public Point Position { get; private set; }

    }
}
