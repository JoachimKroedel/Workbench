using FillAPixEngine;
using FillAPixRobot;
using FillAPixRobot.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConceptisPuzzles.Robot
{
    public partial class FillAPixRobotView : Form
    {
        private enum PlayMode
        {
            Pause,
            PlaySimulation,
            RunInBackGround,
            FindBest
        }

        private PlayMode _playMode = PlayMode.Pause;

        private PuzzleBoard _puzzleBoard = null;
        private RobotBrain _robotBrain;
        private PuzzleReferee _puzzleReferee;
        private int _cellSize = 20;
        private List<PuzzleArea> _knownAreas = new List<PuzzleArea>();
        private Bitmap _backGroundImage = null;
        private Random _random = new Random();

        private List<DirectionTypes> _allowedDirectionTypes = new List<DirectionTypes>();
        private List<FieldOfVisionTypes> _allowedFieldOfVisionTypes = new List<FieldOfVisionTypes>();

        public FillAPixRobotView()
        {
            InitializeComponent();
            foreach (var fieldOfVisionType in Enum.GetValues(typeof(FieldOfVisionTypes)))
            {
                _ddbFieldOfVisionTypes.Items.Add(fieldOfVisionType);
            }
            
            foreach (var directionType in Enum.GetValues(typeof(DirectionTypes)))
            {
                _cbxDirectionTypes.Items.Add(directionType);
            }
            _cbxDirectionTypes.SelectedIndex = 0;

            _cbxTypeOfRobot.SelectedIndex = 2;
            _ddbFieldOfVisionTypes.SelectedIndex = 2;

            _cbxBehaviourOnError.SelectedIndex = 2;
        }

        private void FillAPixRobotView_Load(object sender, EventArgs e)
        {
            _robotBrain = new RobotBrain();
            _robotBrain.PropertyChanged += RobotBrain_PropertyChanged;
            _robotBrain.ExperienceWanted += RobotBrain_ExperienceWanted;
            _robotBrain.ActionWanted += RobotBrain_ActionWanted;
            _robotBrain.ConflictDetected += RobotBrain_ConflictDetected;
        }

        private void BtnLoadPuzzle_Click(object sender, EventArgs e)
        {
            DialogLoadPuzzle loadDialog = new DialogLoadPuzzle();
            // ToDo: Replace static hard coded folder reference with relative path
            loadDialog.InitialDirectory = @"..\..\FillAPixRobot\Puzzles";
            if (loadDialog.ShowDialog() == DialogResult.OK)
            {
                _puzzleBoard = new PuzzleBoard(loadDialog.FileName);
                _robotBrain.Activate(new Point(0, 0), new Rectangle(new Point(0, 0), _puzzleBoard.Size));
                _puzzleReferee = new PuzzleReferee(_puzzleBoard);
            }
            _gbxRobot.Enabled = _puzzleBoard != null;
            _btnRunInBackground.Enabled = _puzzleBoard != null;
            RefreshRobotSettings();
            RefreshPlayGround();
            RecreateCells();
        }

        private void RefreshRobotSettings()
        {
            _nudPositionX.Minimum = _robotBrain.Area.Left;
            _nudPositionX.Maximum = _robotBrain.Area.Right - _robotBrain.Area.Left - 1;
            _nudPositionY.Minimum = _robotBrain.Area.Top;
            _nudPositionY.Maximum = _robotBrain.Area.Bottom - _robotBrain.Area.Top - 1;
        }

        private void RefreshPlayGround()
        {
            if (_puzzleBoard == null)
            {
                _pbxPlayGround.Image = null;
                return;
            }

            int width = _puzzleBoard.Columns * _cellSize;
            int height = _puzzleBoard.Rows * _cellSize;
            Bitmap playGround = new Bitmap(width + 1, height + 1);
            Font font = new Font(Font.FontFamily, (int)Math.Round(_cellSize * 0.55));
            List<List<Point>> positionAreas = new List<List<Point>>();
            foreach (PuzzleArea area in _knownAreas)
            {
                positionAreas.Add(area.Positions);
            }
            using (Graphics graphics = Graphics.FromImage(playGround))
            {
                graphics.Clear(Color.Transparent);
                for (int y = 0; y <= _puzzleBoard.Rows; y++)
                {
                    graphics.DrawLine(new Pen(Color.Gray), 0, y * _cellSize, width, y * _cellSize);
                }
                for (int x = 0; x <= _puzzleBoard.Columns; x++)
                {
                    graphics.DrawLine(new Pen(Color.Gray), x * _cellSize, 0, x * _cellSize, height);
                }
                for (int y = 0; y < _puzzleBoard.Rows; y++)
                {
                    for (int x = 0; x < _puzzleBoard.Columns; x++)
                    {
                        Point cellPos = new Point(x, y);
                        int number = _puzzleBoard.GetValue(cellPos);
                        if (number >= 0 && number <= 9)
                        {
                            Brush fontBrush = _puzzleBoard.GetState(cellPos) == PuzzleCellStateTypes.Filled ? Brushes.White : Brushes.Black;
                            graphics.DrawString(number.ToString(), font, fontBrush, new Rectangle(x * _cellSize + 3, y * _cellSize + 1, _cellSize, _cellSize));
                        }
                    }
                }
            }
            _pbxPlayGround.Image = playGround;
            FitPlayGroundToPanel();
        }

        private void FitPlayGroundToPanel()
        {
            if (_pbxPlayGround.Image == null)
            {
                return;
            }
            if (_pbxPlayGround.Image.Width > _panelPlayground.Width || _pbxPlayGround.Image.Height > _panelPlayground.Height)
            {
                _pbxPlayGround.BackgroundImageLayout = ImageLayout.Zoom;
                _pbxPlayGround.SizeMode = PictureBoxSizeMode.AutoSize;
                _pbxPlayGround.Dock = DockStyle.None;
            }
            else
            {
                _pbxPlayGround.BackgroundImageLayout = ImageLayout.Center;
                _pbxPlayGround.SizeMode = PictureBoxSizeMode.CenterImage;
                _pbxPlayGround.Dock = DockStyle.Fill;
            }
        }

        private void RecreateCells()
        {
            if (_pbxPlayGround.Image == null)
            {
                return;
            }

            _backGroundImage = new Bitmap(_pbxPlayGround.Image.Width, _pbxPlayGround.Image.Height);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                graphics.Clear(Color.White);
            }
            for (int y = 0; y < _puzzleBoard.Rows; y++)
            {
                for (int x = 0; x < _puzzleBoard.Columns; x++)
                {
                    RefreshCell(x, y);
                }
            }
            for (int y = 0; y < _puzzleBoard.Rows; y++)
            {
                for (int x = 0; x < _puzzleBoard.Columns; x++)
                {
                    RefreshRobotCell(x, y);
                }
            }
            _pbxPlayGround.BackgroundImage = _backGroundImage;
        }

        private void RefreshCell(int posX, int posY)
        {
            int drawX = posX * _cellSize;
            int drawY = posY * _cellSize;
            Point position = new Point(posX, posY);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                switch (_puzzleBoard.GetState(new Point(posX, posY)))
                {
                    case PuzzleCellStateTypes.NotMarked: 
                        graphics.FillRectangle(Brushes.White, drawX, drawY, _cellSize, _cellSize); 
                        break;
                    case PuzzleCellStateTypes.Filled: 
                        graphics.FillRectangle(Brushes.Black, drawX, drawY, _cellSize, _cellSize); 
                        break;
                    case PuzzleCellStateTypes.Empty:
                        graphics.FillRectangle(Brushes.Gainsboro, drawX, drawY, _cellSize, _cellSize);
                        graphics.DrawLine(new Pen(Color.Gray), drawX, drawY, drawX + _cellSize, drawY + _cellSize);
                        graphics.DrawLine(new Pen(Color.Gray), drawX, drawY + _cellSize, drawX + _cellSize, drawY);
                        break;
                    case PuzzleCellStateTypes.Outside:
                        graphics.FillRectangle(Brushes.DarkBlue, drawX, drawY, _cellSize, _cellSize);
                        graphics.DrawLine(new Pen(Color.White), drawX, drawY, drawX + _cellSize, drawY + _cellSize);
                        graphics.DrawLine(new Pen(Color.White), drawX, drawY + _cellSize, drawX + _cellSize, drawY);
                        break;
                }
                if (_cbxHighlightErrors.Checked && _puzzleBoard.IsWrong(position))
                {
                    Pen errorPositionPen = new Pen(Brushes.Red, 3);
                    graphics.DrawRectangle(errorPositionPen, drawX + 2, drawY + 2, _cellSize - 4, _cellSize - 4);
                }
            }
        }

        private void RefreshRobotCell(int posX, int posY)
        {
            if (_robotBrain == null)
                return;
            int drawX = posX * _cellSize;
            int drawY = posY * _cellSize;
            Point position = new Point(posX, posY);
            using (Graphics graphics = Graphics.FromImage(_backGroundImage))
            {
                lock (_robotBrain.LastPositions)
                {
                    if (_robotBrain.Position.X == posX && _robotBrain.Position.Y == posY)
                    {
                        Pen robotPen = new Pen(Color.Blue, 3);
                        graphics.DrawEllipse(robotPen, drawX + 2, drawY + 2, _cellSize - 4, _cellSize - 4);
                    }
                    else if (_robotBrain.LastPositions.Contains(position))
                    {
                        int index = _robotBrain.LastPositions.IndexOf(position);
                        if (index > -1)
                        {
                            int grayColor = 255 / _robotBrain.LastPositions.Count * index;
                            Pen robotPositionPen = new Pen(Color.FromArgb(grayColor, grayColor, grayColor), 2);
                            graphics.DrawEllipse(robotPositionPen, drawX, drawY, _cellSize, _cellSize);
                        }
                    }
                }
            }
        }

        private void BtnResetPuzzle_Click(object sender, EventArgs e)
        {
            _puzzleBoard.Reset();

            RefreshRobotSettings();
            RefreshPlayGround();
            RecreateCells();
        }

        private void NudZoomFactor_ValueChanged(object sender, EventArgs e)
        {
            _cellSize = (int)Math.Round(20 * _nudZoomFactor.Value / 100);
            if (_cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
                Refresh();
            }
        }

        private void CbxHighlightErrors_CheckedChanged(object sender, EventArgs e)
        {
            RecreateCells();
        }

        private void MoveRobot(DirectionTypes directionType)
        {
            if (_puzzleReferee.CheckAction(_robotBrain.Position, directionType, ActionTypes.Move))
            {
                Point direction = PuzzleReferee.ConvertToPoint(directionType);
                Point actionPosition = new Point(_robotBrain.Position.X + direction.X, _robotBrain.Position.Y + direction.Y);
                _robotBrain.Position = actionPosition;
            }
        }

        private void BtnMoveDown_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.South);
        }

        private void BtnMoveRight_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.East);
        }

        private void BtnMoveUp_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.North);
        }

        private void _btnMoveLeft_Click(object sender, EventArgs e)
        {
            MoveRobot(DirectionTypes.West);
        }

        private void BtnJump_Click(object sender, EventArgs e)
        {
            if (_chkRandomJump.Checked)
            {
                _nudPositionX.Value = _random.Next((int)_nudPositionX.Minimum, (int)_nudPositionX.Maximum);
                _nudPositionY.Value = _random.Next((int)_nudPositionY.Minimum, (int)_nudPositionY.Maximum);
            }
            _robotBrain.Position = new Point((int)_nudPositionX.Value, (int)_nudPositionY.Value);
        }

        private void BtnMarkAsFilled_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.Filled);
        }

        private void BtnMarkAsEmpty_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.Empty);
        }

        private void BtnMarkAsUndefined_Click(object sender, EventArgs e)
        {
            DoStateAction(PuzzleCellStateTypes.NotMarked);
        }

        private void BtnMove_Click(object sender, EventArgs e)
        {
            if (_cbxDirectionTypes.SelectedIndex < 0)
            {
                return;
            }
            DirectionTypes directionType = (DirectionTypes)_cbxDirectionTypes.SelectedItem;
            MoveRobot(directionType);
        }

        private void DoStateAction(PuzzleCellStateTypes markerState)
        {
            if (_cbxDirectionTypes.SelectedIndex < 0)
            {
                return;
            }
            DirectionTypes directionType = (DirectionTypes)_cbxDirectionTypes.SelectedItem;
            Point direction = PuzzleReferee.ConvertToPoint(directionType);
            Point markerPosition = new Point(_robotBrain.Position.X + direction.X, _robotBrain.Position.Y + direction.Y);
            _puzzleBoard.SetState(markerPosition, markerState);

            if (_cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
            }
        }

        private void SetValueAndState(Point position, Point distancePoint, PuzzleBoard patialBoard)
        {
            int width = patialBoard.Columns;
            int height = patialBoard.Rows;
            Point centerPos = new Point((width - 1) / 2, (height - 1) / 2);
            Point nextPosition = new Point(position.X + distancePoint.X, position.Y + distancePoint.Y);
            Point partialPos = new Point(centerPos.X + distancePoint.X, centerPos.Y + distancePoint.Y);
            int number = _puzzleBoard.GetValue(nextPosition);
            PuzzleCellStateTypes state = _puzzleBoard.GetState(nextPosition);
            patialBoard.SetValue(partialPos, number);
            patialBoard.SetState(partialPos, state);
        }

        private void FillPartialBoard(PuzzleBoard patialBoard, FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            switch(fieldOfVisionType)
            {
                case FieldOfVisionTypes.Single:
                    SetValueAndState(position, new Point(0, 0), patialBoard);
                    break;
                case FieldOfVisionTypes.ThreeByThree:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.Single, position);
                    SetValueAndState(position, new Point(-1, 0), patialBoard);
                    SetValueAndState(position, new Point(1, 0), patialBoard);
                    SetValueAndState(position, new Point(0, -1), patialBoard);
                    SetValueAndState(position, new Point(0, 1), patialBoard);
                    SetValueAndState(position, new Point(-1, -1), patialBoard);
                    SetValueAndState(position, new Point(-1,  1), patialBoard);
                    SetValueAndState(position, new Point( 1,  1), patialBoard);
                    SetValueAndState(position, new Point( 1, -1), patialBoard);
                    break;
                case FieldOfVisionTypes.FiveByFive:
                    FillPartialBoard(patialBoard, FieldOfVisionTypes.ThreeByThree, position);
                    SetValueAndState(position, new Point(-2, 0), patialBoard);
                    SetValueAndState(position, new Point(2, 0), patialBoard);
                    SetValueAndState(position, new Point(0, 2), patialBoard);
                    SetValueAndState(position, new Point(0, -2), patialBoard);
                    SetValueAndState(position, new Point(-2, -1), patialBoard);
                    SetValueAndState(position, new Point(-2, 1), patialBoard);
                    SetValueAndState(position, new Point(2, -1), patialBoard);
                    SetValueAndState(position, new Point(2, 1), patialBoard);

                    SetValueAndState(position, new Point(-1, 2), patialBoard);
                    SetValueAndState(position, new Point(1, 2), patialBoard);
                    SetValueAndState(position, new Point(-1, -2), patialBoard);
                    SetValueAndState(position, new Point(1, -2), patialBoard);
                    SetValueAndState(position, new Point(-2, -2), patialBoard);
                    SetValueAndState(position, new Point(-2,  2), patialBoard);
                    SetValueAndState(position, new Point( 2, -2), patialBoard);
                    SetValueAndState(position, new Point( 2,  2), patialBoard);
                    break;
            }
        }

        private PuzzleBoard CreatePartialBoard(FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            PuzzleBoard partialBoard;
            switch (fieldOfVisionType)
            {
                case FieldOfVisionTypes.Single:
                    partialBoard = new PuzzleBoard(1, 1);
                    break;
                case FieldOfVisionTypes.ThreeByThree:
                    partialBoard = new PuzzleBoard(3, 3);
                    break;
                case FieldOfVisionTypes.FiveByFive:
                    partialBoard = new PuzzleBoard(5, 5);
                    break;
                default:
                    return null;
            }
            FillPartialBoard(partialBoard, fieldOfVisionType, position);
            return partialBoard;
        }

        private void DrawFieldOfVision(FieldOfVisionTypes fieldOfVisionType, Point position)
        {
            PuzzleBoard partialBoard = CreatePartialBoard(fieldOfVisionType, position);
            if (partialBoard == null)
            {
                return;
            }

            int width = partialBoard.Columns;
            int height = partialBoard.Rows;
            int drawCellSize = Math.Min(_pbxLookResult.Width, _pbxLookResult.Height);
            int maxFieldDimension = Math.Max(width, height);
            drawCellSize = drawCellSize / maxFieldDimension;

            int drawWidth = width * drawCellSize;
            int drawHeight = height * drawCellSize;
            Bitmap playGround = new Bitmap(drawWidth + 1, drawHeight + 1);
            Bitmap playGroundBackground = new Bitmap(drawWidth + 1, drawHeight + 1);
            Font font = new Font(Font.FontFamily, (int)Math.Round(drawCellSize * 0.55));

            using (Graphics graphics = Graphics.FromImage(playGround))
            {
                using (Graphics graphicsBackground = Graphics.FromImage(playGroundBackground))
                {
                    graphics.Clear(Color.Transparent);
                    graphicsBackground.Clear(Color.White);

                    for (int y = 0; y <= partialBoard.Rows; y++)
                    {
                        graphics.DrawLine(new Pen(Color.Gray), 0, y * drawCellSize, drawWidth, y * drawCellSize);
                    }
                    for (int x = 0; x <= partialBoard.Columns; x++)
                    {
                        graphics.DrawLine(new Pen(Color.Gray), x * drawCellSize, 0, x * drawCellSize, drawHeight);
                    }
                    for (int y = 0; y < partialBoard.Rows; y++)
                    {
                        for (int x = 0; x < partialBoard.Columns; x++)
                        {
                            Point cellPos = new Point(x, y);
                            int drawX = x * drawCellSize;
                            int drawY = y * drawCellSize;
                            int number = partialBoard.GetValue(cellPos);
                            if (number >= 0 && number <= 9)
                            {
                                Brush fontBrush = partialBoard.GetState(cellPos) == PuzzleCellStateTypes.Filled ? Brushes.White : Brushes.Black;
                                graphics.DrawString(number.ToString(), font, fontBrush, new Rectangle(drawX + (30 / maxFieldDimension), drawY + (10 / maxFieldDimension), drawCellSize, drawCellSize));
                            }
                            switch (partialBoard.GetState(cellPos))
                            {
                                case PuzzleCellStateTypes.Undefined:
                                    graphicsBackground.FillRectangle(Brushes.Yellow, drawX, drawY, drawCellSize, drawCellSize); 
                                    break;
                                case PuzzleCellStateTypes.Filled: 
                                    graphicsBackground.FillRectangle(Brushes.Black, drawX, drawY, drawCellSize, drawCellSize); 
                                    break;
                                case PuzzleCellStateTypes.Empty:
                                    graphicsBackground.FillRectangle(Brushes.Gainsboro, drawX, drawY, drawCellSize, drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.Gray), drawX, drawY, drawX + drawCellSize, drawY + drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.Gray), drawX, drawY + drawCellSize, drawX + drawCellSize, drawY);
                                    break;
                                case PuzzleCellStateTypes.Outside:
                                    graphicsBackground.FillRectangle(Brushes.DarkBlue, drawX, drawY, drawCellSize, drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.White), drawX, drawY, drawX + drawCellSize, drawY + drawCellSize);
                                    graphicsBackground.DrawLine(new Pen(Color.White), drawX, drawY + drawCellSize, drawX + drawCellSize, drawY);
                                    break;
                            }
                        }
                    }
                }
            }
            _pbxLookResult.Image = playGround;
            _pbxLookResult.BackgroundImage = playGroundBackground;
        }

        private void DdbFieldOfVisionTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            _fieldOfVisionType = (FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem;
            if (_robotBrain != null && _ddbFieldOfVisionTypes.SelectedIndex >= 0)
            {
                DrawFieldOfVision((FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem, _robotBrain.Position);
            }
        }

        private void CbxTypeOfRobot_SelectedIndexChanged(object sender, EventArgs e)
        {
            _allowedDirectionTypes.Clear();
            _allowedFieldOfVisionTypes.Clear();

            if (_cbxTypeOfRobot.SelectedIndex >= 0)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.Single);

                _allowedDirectionTypes.Add(DirectionTypes.Center);
                _allowedDirectionTypes.Add(DirectionTypes.North);
                _allowedDirectionTypes.Add(DirectionTypes.East);
                _allowedDirectionTypes.Add(DirectionTypes.South);
                _allowedDirectionTypes.Add(DirectionTypes.West);
            }
            if (_cbxTypeOfRobot.SelectedIndex >= 1)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.ThreeByThree);

                _allowedDirectionTypes.Add(DirectionTypes.NorthWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthEast);
            }
            if (_cbxTypeOfRobot.SelectedIndex >= 2)
            {
                _allowedFieldOfVisionTypes.Add(FieldOfVisionTypes.FiveByFive);

                _allowedDirectionTypes.Add(DirectionTypes.NorthNorth);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouth);
                _allowedDirectionTypes.Add(DirectionTypes.WestWest);
                _allowedDirectionTypes.Add(DirectionTypes.EastEast);

                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthEast);
                _allowedDirectionTypes.Add(DirectionTypes.NorthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthEastEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthEastEast);

                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.NorthNorthEastEast);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthWestWest);
                _allowedDirectionTypes.Add(DirectionTypes.SouthSouthEastEast);
            }

            _ddbFieldOfVisionTypes.Items.Clear();
            foreach (var fieldOfVisionType in _allowedFieldOfVisionTypes)
            {
                _ddbFieldOfVisionTypes.Items.Add(fieldOfVisionType);
            }
            _ddbFieldOfVisionTypes.SelectedIndex = 0;

            _cbxDirectionTypes.Items.Clear();
            foreach (var directionType in _allowedDirectionTypes)
            {
                _cbxDirectionTypes.Items.Add(directionType);
            }
            _cbxDirectionTypes.SelectedIndex = 0;
        }

        private void RobotBrain_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                MethodInvoker del = delegate {RobotBrain_PropertyChanged(this, e);};
                Invoke(del);
                return;
            }
            if (_simulationRunsInBackground)
            {
                return;
            }
            switch (e.PropertyName)
            {
                case nameof(_robotBrain.Position):
                    _nudPositionX.Value = _robotBrain.Position.X;
                    _nudPositionY.Value = _robotBrain.Position.Y;
                    DrawFieldOfVision((FieldOfVisionTypes)_ddbFieldOfVisionTypes.SelectedItem, _robotBrain.Position);
                    break;
                case nameof(_robotBrain.PercentageSolving):
                    _tbrSolvingPercentage.Value = (int)_robotBrain.PercentageSolving;
                    break;
            }
            if (_cbxAutoRefreshPlayground.Checked)
            {
                RecreateCells();
            }
        }

        private FieldOfVisionTypes _fieldOfVisionType;

        private void RobotBrain_ExperienceWanted(object sender, EventArgs e)
        {
            PuzzleBoard partialBoard = CreatePartialBoard(_fieldOfVisionType, _robotBrain.Position);
            if (partialBoard == null)
            {
                return;
            }
            _robotBrain.Experience(_fieldOfVisionType, partialBoard);
        }

        private void RobotBrain_ActionWanted(object sender, ActionWantedEventArgs e)
        {
            DirectionTypes actionDirection = e.Action.Direction;
            ActionTypes actionType = e.Action.Type;
            if (_puzzleReferee.CheckAction(_robotBrain.Position, actionDirection, actionType))
            {
                Point direction = PuzzleReferee.ConvertToPoint(actionDirection);
                Point actionPosition = new Point(_robotBrain.Position.X + direction.X, _robotBrain.Position.Y + direction.Y);
                int stateChangeCount = 0;
                switch (actionType)
                {
                    case ActionTypes.Move:
                        _robotBrain.Position = actionPosition;
                        break;
                    case ActionTypes.MarkAsEmpty:
                        stateChangeCount = _puzzleBoard.SetState(actionPosition, PuzzleCellStateTypes.Empty).Count;
                        break;
                    case ActionTypes.MarkAsFilled:
                        stateChangeCount = _puzzleBoard.SetState(actionPosition, PuzzleCellStateTypes.Filled).Count;
                        break;
                    case ActionTypes.RemoveMarker:
                        stateChangeCount = _puzzleBoard.SetState(actionPosition, PuzzleCellStateTypes.NotMarked).Count;
                        break;
                }
                if (_puzzleBoard.IsWrong())
                {
                    _robotBrain.ActionFeedback -= 1000;
                }
                else if (_puzzleBoard.IsComplete())
                {
                    _robotBrain.ActionFeedback += 100;
                }
                else if (stateChangeCount > 0)
                {
                    _robotBrain.ActionFeedback += stateChangeCount;
                }
            }

            if (!_simulationRunsInBackground && _cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
            }
        }

        private void RobotBrain_ConflictDetected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                MethodInvoker del = delegate { RobotBrain_ConflictDetected(this, e); };
                Invoke(del);
                return;
            }

            _puzzleBoard.Reset();
            if (!_simulationRunsInBackground && _cbxAutoRefreshPlayground.Checked)
            {
                RefreshPlayGround();
                RecreateCells();
            }
        }

        private bool CheckIfTimerShouldBeActive()
        {
            if (_playMode.Equals(PlayMode.FindBest))
            {
                return true;
            }
            if ((_cbxRunInterations.Checked || _simulationRunsInBackground) && _nudRemainigIterationCount.Value > _nudRemainigIterationCount.Minimum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChbRunInterations_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckIfTimerShouldBeActive())
            {
                _playMode = PlayMode.PlaySimulation;
                _timer.Interval = 10;
                _timer.Enabled = true;
            }
            else
            {
                _playMode = PlayMode.Pause;
                _timer.Enabled = false;
            }
        }

        private void CbxFindBest_CheckedChanged(object sender, EventArgs e)
        {
            if (_cbxFindBest.Checked)
            {
                if (_playMode != PlayMode.Pause)
                {
                    _cbxFindBest.Checked = false;
                }
                _scanPosition = new Point(0,0);
                _bestActionMemoryQuartet = null;
                _playMode = PlayMode.FindBest;
                _timer.Interval = 10;
                _timer.Enabled = true;
            }
            else
            {
                _playMode = PlayMode.Pause;
            }
        }

        private Point _scanPosition = new Point(0, 0);
        private Point _bestPosition = Point.Empty;
        private IActionMemoryQuartet _bestActionMemoryQuartet = null;
        private Point GetNextScanPoint(Point position, Size area)
        {
            Point result = new Point(position.X + 1, position.Y);
            if (result.X >= area.Width)
            {
                result = new Point(0, result.Y + 1);
            }
            if (result.Y >= area.Height)
            {
                result = new Point(0, 0);
            }
            return result;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Enabled = false;
            if (_playMode.Equals(PlayMode.FindBest))
            {
                var bestAction = _robotBrain.FindBestActionMemoryQuartet(_scanPosition);
                Console.WriteLine($" {_scanPosition} \t--> bestAction: {bestAction}");
                if (_bestActionMemoryQuartet == null || _bestActionMemoryQuartet.StepSize < bestAction.StepSize)
                {
                    _bestActionMemoryQuartet = bestAction;
                    _bestPosition = _scanPosition;
                }
                _scanPosition = GetNextScanPoint(_scanPosition, _puzzleBoard.Size);

                if (_scanPosition.Equals(Point.Empty))
                {
                    _robotBrain.Position = _bestPosition;
                    _cbxDirectionTypes.SelectedItem = _bestActionMemoryQuartet.Action.Direction;
                    _cbxFindBest.Checked = false;
                    Console.WriteLine($"Best found for position:\t{_bestPosition} \taction:{_bestActionMemoryQuartet}");
                }
            }
            else
            {
                if (_simulationRunsInBackground)
                {
                    _nudRemainigIterationCount.Value = _backgroundIterations;
                    _tbrSolvingPercentage.Value = (int)_robotBrain.PercentageSolving;
                    RefreshPlayGround();
                    RecreateCells();
                }
                else
                {
                    bool resetOnError = _cbxBehaviourOnError.SelectedIndex == 2;
                    if (_cbxBehaviourOnError.SelectedIndex == 3)
                    {
                        resetOnError = _tbrSolvingPercentage.Value <= 50;
                    }
                    RunOneIteration(resetOnError);

                    _nudRemainigIterationCount.Value -= 1;
                    if (_nudRemainigIterationCount.Value <= _nudRemainigIterationCount.Minimum)
                    {
                        _cbxRunInterations.Checked = false;
                    }
                }
            }
            _timer.Enabled = CheckIfTimerShouldBeActive();
        }

        private bool CheckPuzzle(bool resetOnOrror)
        {
            if (_puzzleBoard.IsWrong())
            {
                if (resetOnOrror)
                {
                    _puzzleBoard.Reset();
                }
                else
                {
                    _puzzleBoard.Undo();
                }

                if (!_simulationRunsInBackground && _cbxAutoRefreshPlayground.Checked)
                {
                    RefreshPlayGround();
                    RecreateCells();
                }
            }
            else if (_puzzleBoard.IsComplete())
            {
                if (_simulationRunsInBackground || _timer.Enabled)
                {
                    _timer.Enabled = false;
                    _cancellationTokenSource.Cancel();
                    _simulationRunsInBackground = false;
                }

                _cbxRunInterations.Checked = false;
                MessageBox.Show("Puzzle solved!!!", "Robot");
                return false;
            }
            return true;
        }

        private void RunOneIteration(bool resetOnOrror)
        {
            if (CheckPuzzle(resetOnOrror))
            {
                _robotBrain.RiskFactor = (double)_nudRiskFactor.Value / 100;
                _robotBrain.TryToLearn();
            }
        }

        private void BtnStatisticForm_Click(object sender, EventArgs e)
        {
            RobotBrainInfoForm robotBrainInfoDialog = new RobotBrainInfoForm();
            robotBrainInfoDialog.RobotBrain = _robotBrain;
            robotBrainInfoDialog.ShowDialog();
        }

        private void CbxAutoRefreshPlayground_CheckedChanged(object sender, EventArgs e)
        {
            _pbxPlayGround.Visible = _cbxAutoRefreshPlayground.Checked;
        }

        private bool _simulationRunsInBackground = false;
        private async void BtnRunInBackground_Click(object sender, EventArgs e)
        {
            if (_simulationRunsInBackground)
            {
                _cancellationTokenSource.Cancel();
                return;
            }

            _playMode = PlayMode.RunInBackGround;
            _simulationRunsInBackground = true;

            if (!CheckIfTimerShouldBeActive())
            {
                return;
            }
            _cbxRunInterations.Checked = false;
            _gbxRobot.Enabled = false;
            _timer.Interval = 1000;
            _timer.Enabled = true;

            _nudRemainigIterationCount.Value = await RunSimulationInBackgroundAsync(_cbxBehaviourOnError.SelectedIndex);

            _timer.Enabled = false;
            _gbxRobot.Enabled = true;

            _simulationRunsInBackground = false;
            _playMode = PlayMode.Pause;
        }

        private int _backgroundIterations = 0;
        private int RunSimulationInBackground(int iterrations, int resetOnErrorIndex, CancellationToken cancellationToken)
        {
            _backgroundIterations = iterrations;
            while (_backgroundIterations > 0 && !cancellationToken.IsCancellationRequested)
            {
                bool resetOnError = resetOnErrorIndex == 2;
                if (resetOnErrorIndex == 3)
                {
                    resetOnError = _robotBrain.PercentageSolving <= 50;
                }

                RunOneIteration(resetOnError);
                _backgroundIterations--;
            }
            return _backgroundIterations;
        }

        private Task<int> _backgroundTask = null;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private async Task<int> RunSimulationInBackgroundAsync(int resetOnErrorIndex)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _backgroundTask = new Task<int>(() => RunSimulationInBackground((int)_nudRemainigIterationCount.Value, resetOnErrorIndex, _cancellationTokenSource.Token));
            _backgroundTask.Start();
            int result = await _backgroundTask;
            _backgroundTask = null;
            return result;
        }
    }
}
