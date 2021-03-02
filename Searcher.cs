using DrMarioPlayer.Model;
using DrMarioPlayer.Utility;
using System.Collections.Generic;

namespace DrMarioPlayer
{
    internal class Searcher
    {
        public List<Pill> LockedPills { get; set; }

        private Tile[,] gameBoard;
        private Queue<Pill> pillQueue;
        private Dictionary<string, Move> moveSet;
        
        public Searcher(Tile[,] gameBoard)
        {
            this.gameBoard = gameBoard;
            this.pillQueue = new Queue<Pill>();
            this.moveSet = new Dictionary<string, Move>();
            this.LockedPills = new List<Pill>();
        }

        public void Search(Pill currentPill)
        {
            pillQueue.Enqueue(currentPill);

            while (pillQueue.Count > 0)
            {
                Pill pill = pillQueue.Dequeue();

                //Check if Pill is Locked
                if (pill.Orientation == Orientation.HORIZONTAL || pill.Orientation == Orientation.REVERSE_HORIZONTAL)
                {
                    if ( pill.Position.Y == 0 
                        || gameBoard[pill.Position.X, pill.Position.Y - 1] != Tile.NONE 
                        || gameBoard[pill.Position.X + 1, pill.Position.Y - 1] != Tile.NONE )
                    {
                        LockedPills.Add(pill);
                    }
                }
                else if (pill.Orientation == Orientation.REVERSE_VERTICAL)
                {
                    if ( pill.Position.Y == 1 
                        || gameBoard[pill.Position.X, pill.Position.Y - 2] != Tile.NONE)
                    {
                        LockedPills.Add(pill);
                    }
                }
                else //Orientation.VERTICAL
                {
                    if ( pill.Position.Y == 0 
                        || gameBoard[pill.Position.X, pill.Position.Y - 1] != Tile.NONE)
                    {
                        LockedPills.Add(pill);
                    }
                }

                //ROTATION
                if (pill.Orientation == Orientation.HORIZONTAL)
                {
                    if ( pill.Position.Y < Config.Environment.HEIGHT - 1 
                        && gameBoard[pill.Position.X, pill.Position.Y + 1] == Tile.NONE)
                    {
                        EnqueuePill(pill, Move.ROTATE_90);
                    }
                }
                else if (pill.Orientation == Orientation.REVERSE_HORIZONTAL)
                {
                    if ( pill.Position.Y > 0 
                        && gameBoard[pill.Position.X, pill.Position.Y - 1] == Tile.NONE)
                    {
                        EnqueuePill(pill, Move.ROTATE_90);
                    }
                }
                else if (pill.Orientation == Orientation.REVERSE_VERTICAL)
                {
                    if ( pill.Position.X == Config.Environment.WIDTH - 1 
                        || ( pill.Position.Y > 0
                        && gameBoard[pill.Position.X + 1, pill.Position.Y - 1] == Tile.NONE ))
                    {
                        EnqueuePill(pill, Move.ROTATE_90);
                    }
                }
                else if (pill.Orientation == Orientation.VERTICAL)
                {
                    if (pill.Position.X == Config.Environment.WIDTH - 1 
                        || gameBoard[pill.Position.X + 1, pill.Position.Y] == Tile.NONE)
                    {
                        EnqueuePill(pill, Move.ROTATE_90);
                    }
                }

                //LEFT
                if (pill.Position.X > 0)
                {
                    EnqueuePill(pill, Move.LEFT);
                }

                //RIGHT
                if (pill.Position.X < Config.Environment.WIDTH - 1)
                {
                    EnqueuePill(pill, Move.RIGHT);
                }

                //DOWN
                if (pill.Position.Y > 0)
                {
                    EnqueuePill(pill, Move.DOWN);
                }
            }
        }

        private void EnqueuePill(Pill pill, Move move)
        {
            pill = pill.Move(move);

            if ((pill.Orientation == Orientation.HORIZONTAL || pill.Orientation == Orientation.REVERSE_HORIZONTAL) 
                && pill.Position.X == Config.Environment.WIDTH - 1)
            {
                pill.Position.X -= 1;
            }

            if ( ContainsMoveSet(pill.Position.X, pill.Position.Y, pill.Orientation) 
                || gameBoard[pill.Position.X, pill.Position.Y] != Tile.NONE)
            {
                return;
            }      

            if (pill.Orientation == Orientation.HORIZONTAL || pill.Orientation == Orientation.REVERSE_HORIZONTAL)
            {
                if (gameBoard[pill.Position.X + 1, pill.Position.Y] != Tile.NONE)
                {
                    return;
                }
            }
            else if (pill.Orientation == Orientation.VERTICAL)
            {
                if ( pill.Position.Y < Config.Environment.HEIGHT - 1 
                    && gameBoard[pill.Position.X, pill.Position.Y + 1] != Tile.NONE)
                {
                    return;
                }
            }
            else //Orientation.REVERSE_VERTICAL
            {
                if ( pill.Position.Y == 0 
                    || ( pill.Position.Y > 0
                    && gameBoard[pill.Position.X, pill.Position.Y - 1] != Tile.NONE))
                {
                    return;
                }
            }

            AddMoveSet(pill.Position.X, pill.Position.Y, pill.Orientation, move);

            pillQueue.Enqueue(pill);
        }

        private void AddMoveSet(int row, int col, Orientation orientation, Move move)
        {
            string key = $"{row}-{col}-{orientation}";

            if (!moveSet.ContainsKey(key))
            {
                moveSet.Add(key, move);
            }
        }

        private bool ContainsMoveSet(int row, int col, Orientation orientation)
        {
            string key = $"{row}-{col}-{orientation}";
            return moveSet.ContainsKey(key);
        }

        public List<Move> GetSpecialMoveSet(int row, int col, Orientation orientation)
        {
            List<Move> moves = new List<Move>();

            int x = row;
            int y = col;
            Orientation o = orientation;

            while (moveSet.TryGetValue($"{x}-{y}-{o}", out Move move))
            {
                moves.Add(move);

                switch (move)
                {
                    case Move.LEFT:
                        x++;
                        break;

                    case Move.RIGHT:
                        x--;
                        break;

                    case Move.DOWN:
                        y++;
                        break;

                    case Move.ROTATE_90:
                        o = OrientationHelper.Rotate(o, 3);
                        if (!moveSet.ContainsKey(($"{x}-{y}-{o}")))
                        {
                            x++;
                        }

                        break;
                }

                if (x == 3 && y == 12 && o == Orientation.HORIZONTAL)
                {
                    break;
                }
            }

            return moves;
        }

        public List<Move> GetMoveSet(int row, int col, Orientation orientation)
        {
            List<Move> moves = new List<Move>();

            int x = row;
            int y = col;
            Orientation o = orientation;

            while (moveSet.TryGetValue($"{x}-{y}-{o}", out Move move))
            {
                moves.Add(move);

                switch (move)
                {
                    case Move.LEFT:
                        x++;
                        break;

                    case Move.RIGHT:
                        x--;
                        break;

                    case Move.DOWN:
                        y++;
                        break;

                    case Move.ROTATE_90:
                        o = OrientationHelper.Rotate(o, 3);
                        if (!moveSet.ContainsKey(($"{x}-{y}-{o}")))
                        {
                            x++;
                        }
                        break;
                }

                if (x == 3 && y == 13 && o == Orientation.HORIZONTAL)
                {
                    break;
                }
            }

            return moves;
        }
    }
}
