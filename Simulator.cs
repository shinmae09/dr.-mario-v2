using DrMarioPlayer.Model;
using DrMarioPlayer.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrMarioPlayer
{
    internal static class Simulator
    {
        public static Dictionary<Pill, Tile[,]> LockPills(Tile[,] board, List<Pill> pills)
        {
            Dictionary<Pill, Tile[,]> gameBoards = new Dictionary<Pill, Tile[,]>();

            //Parallel.ForEach(pills, (pill) =>
            foreach (var pill in pills)
            {
                Tile[,] gameBoard = (Tile[,])board.Clone();

                var halfPill1 = new Tile(pill.Color1.Value);
                var halfPill2 = new Tile(pill.Color2.Value);

                switch (pill.Orientation)
                {
                    case Orientation.HORIZONTAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = halfPill1;
                        gameBoard[pill.Position.X + 1, pill.Position.Y] = halfPill2;

                        halfPill1.OtherHalf = halfPill2;
                        halfPill2.OtherHalf = halfPill1;
                        halfPill1.Position = new Position(pill.Position.X, pill.Position.Y);
                        halfPill2.Position = new Position(pill.Position.X + 1, pill.Position.Y);
                        break;

                    case Orientation.VERTICAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = halfPill1;
                        gameBoard[pill.Position.X, pill.Position.Y + 1] = halfPill2;

                        halfPill1.Position = new Position(pill.Position.X, pill.Position.Y);
                        halfPill2.Position = new Position(pill.Position.X, pill.Position.Y + 1);
                        break;

                    case Orientation.REVERSE_HORIZONTAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = halfPill2;
                        gameBoard[pill.Position.X + 1, pill.Position.Y] = halfPill1;

                        halfPill1.OtherHalf = halfPill2;
                        halfPill2.OtherHalf = halfPill1;
                        halfPill1.Position = new Position(pill.Position.X + 1, pill.Position.Y);
                        halfPill2.Position = new Position(pill.Position.X, pill.Position.Y);
                        break;

                    case Orientation.REVERSE_VERTICAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = halfPill1;
                        gameBoard[pill.Position.X, pill.Position.Y - 1] = halfPill2;

                        halfPill1.Position = new Position(pill.Position.X, pill.Position.Y);
                        halfPill2.Position = new Position(pill.Position.X, pill.Position.Y - 1);
                        break;
                }

                RemoveTiles(gameBoard);

                gameBoards.Add(pill, gameBoard);
            }

            return gameBoards;
        }

        private static void RemoveTiles(Tile[,] gameBoard)
        {
            while (true)
            {
                List<Position> removedTiles = new List<Position>();

                CheckRemovableTilesVertically(gameBoard, removedTiles);
                CheckRemovableTilesHorizontally(gameBoard, removedTiles);

                if (removedTiles.Count > 0)
                {
                    for (int row = 0; row < gameBoard.GetLength(0); row++)
                    {
                        for (int col = 0; col < gameBoard.GetLength(1); col++)
                        {
                            Tile tile = gameBoard[row, col];
                            if (tile != null && removedTiles.Any(pos => pos.Equals(tile.Position)))
                            {
                                Tile otherHalfTile = tile.OtherHalf;
                                if (otherHalfTile != null)
                                {
                                    otherHalfTile.OtherHalf = null;
                                }
                                gameBoard[row, col] = null;
                            }
                        }
                    }

                    DropUnsupportedPills(gameBoard);
                }
                else
                {
                    break;
                }
            }
        }

        private static void CheckRemovableTilesVertically(Tile[,] gameBoard, List<Position> removedTiles)
        {
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                Color? lastColor = null;
                int consecutiveColorsLength = 0;

                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Tile tile = gameBoard[row, col];
                    Color? tileColor = tile?.Color;
                    if (tileColor != lastColor)
                    {
                        lastColor = tileColor;
                        consecutiveColorsLength = 1;
                    }
                    else
                    {
                        if (tileColor != null)
                        {
                            consecutiveColorsLength++;
                            if (consecutiveColorsLength >= 4)
                            {
                                for (int i = 0; i < consecutiveColorsLength; i++)
                                {
                                    Tile tileToBeRemoved = gameBoard[row, col - i];
                                    Position tileToBeRemovedPosition = tileToBeRemoved?.Position;
                                    if (!removedTiles.Any(pos => pos.Equals(tileToBeRemovedPosition)))
                                    {
                                        removedTiles.Add(tileToBeRemovedPosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckRemovableTilesHorizontally(Tile[,] gameBoard, List<Position> removedTiles)
        {
            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                Color? lastColor = null;
                int consecutiveColorsLength = 0;

                for (int row = 0; row < gameBoard.GetLength(0); row++)
                {
                    Tile tile = gameBoard[row, col];
                    Color? tileColor = tile?.Color;
                    if (tileColor != lastColor)
                    {
                        lastColor = tileColor;
                        consecutiveColorsLength = 1;
                    }
                    else
                    {
                        if (tileColor != null)
                        {
                            consecutiveColorsLength++;
                            if (consecutiveColorsLength >= 4)
                            {
                                for (int i = 0; i < consecutiveColorsLength; i++)
                                {
                                    Tile tileToBeRemoved = gameBoard[row - i, col];
                                    Position tileToBeRemovedPosition = tileToBeRemoved?.Position;
                                    if (!removedTiles.Any(pos => pos.Equals(tileToBeRemovedPosition)))
                                    {
                                        removedTiles.Add(tileToBeRemovedPosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void DropUnsupportedPills(Tile[,] gameBoard)
        {
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Tile tile = gameBoard[row, col];
                    if (tile != null && !tile.IsVirus)
                    {
                        Tile tile2 = tile.OtherHalf;

                        if (tile2 != null)
                        {
                            Position position1 = tile.Position;
                            Position position2 = tile2.Position;

                            if (position1.Y == position2.Y)
                            {
                                int dropIndex = col;
                                while (dropIndex > 0 && gameBoard[position1.X, dropIndex - 1] == null && gameBoard[position2.X, dropIndex - 1] == null)
                                {
                                    dropIndex -= 1;
                                }

                                if (dropIndex != col)
                                {
                                    gameBoard[position1.X, dropIndex] = tile;
                                    gameBoard[position1.X, col] = null;
                                    gameBoard[position2.X, dropIndex] = tile2;
                                    gameBoard[position2.X, col] = null;
                                    tile.Position.Y = dropIndex;
                                    tile2.Position.Y = dropIndex;
                                }
                            }
                        }
                        else
                        {
                            int dropIndex = col;
                            while (dropIndex > 0 && gameBoard[row, dropIndex - 1] == null)
                            {
                                dropIndex -= 1;
                            }

                            if (dropIndex != col)
                            {
                                gameBoard[row, dropIndex] = tile;
                                gameBoard[row, col] = null;
                                tile.Position.Y = dropIndex;
                            }
                        }
                    } 
                }
            }
        }
    }
}
