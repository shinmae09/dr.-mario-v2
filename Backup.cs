using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrMarioPlayer
{
    class Backup
    {
        /*
        private static Pill ClearPill(List<Pill> pills)
        {
            Pill bestPill = null;
            double maxScore = -1;

            foreach (var pill in pills)
            {
                var newBoard = (Tile[,])gameBoard.DeepClone();

                var halfPill1 = new Tile(pill.Color1.Value);
                var halfPill2 = new Tile(pill.Color2.Value);

                switch (pill.Orientation)
                {
                    case Orientation.HORIZONTAL:
                        halfPill1.OtherHalf = halfPill2;
                        halfPill2.OtherHalf = halfPill1;
                        newBoard[pill.Position.X, pill.Position.Y] = halfPill1;
                        newBoard[pill.Position.X + 1, pill.Position.Y] = halfPill2;
                        break;

                    case Orientation.VERTICAL:
                        newBoard[pill.Position.X, pill.Position.Y] = halfPill1;
                        newBoard[pill.Position.X, pill.Position.Y + 1] = halfPill2;
                        break;

                    case Orientation.REVERSE_HORIZONTAL:
                        halfPill1.OtherHalf = halfPill2;
                        halfPill2.OtherHalf = halfPill1;
                        newBoard[pill.Position.X, pill.Position.Y] = halfPill2;
                        newBoard[pill.Position.X + 1, pill.Position.Y] = halfPill1;
                        break;

                    case Orientation.REVERSE_VERTICAL:
                        newBoard[pill.Position.X, pill.Position.Y] = halfPill2;
                        newBoard[pill.Position.X, pill.Position.Y + 1] = halfPill1;
                        break;
                }

                List<Tile> removedTiles;
                do
                {
                    removedTiles = new List<Tile>();

                    //Vertical Checking for tiles that could be cleared
                    for (int row = 0; row < newBoard.GetLength(0); row++)
                    {
                        Color? lastColor = null;
                        int consecutiveColorsLength = 0;

                        for (int col = 0; col < newBoard.GetLength(1); col++)
                        {
                            Tile tile = newBoard[row, col];
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
                                            Tile tileToBeRemoved = newBoard[row, col - i];
                                            if (!removedTiles.Contains(tileToBeRemoved))
                                            {
                                                removedTiles.Add(tileToBeRemoved);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Horizontal Checking for tiles that could be cleared
                    for (int col = 0; col < newBoard.GetLength(1); col++)
                    {
                        Color? lastColor = null;
                        int consecutiveColorsLength = 0;

                        for (int row = 0; row < newBoard.GetLength(0); row++)
                        {
                            Tile tile = newBoard[row, col];
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
                                            Tile tileToBeRemoved = newBoard[row - i, col];
                                            if (!removedTiles.Contains(tileToBeRemoved))
                                            {
                                                removedTiles.Add(tileToBeRemoved);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Remove the tiles
                    for (int row = 0; row < newBoard.GetLength(0); row++)
                    {
                        for (int col = 0; col < newBoard.GetLength(1); col++)
                        {
                            Tile tile = newBoard[row, col];
                            if (tile != null && removedTiles.Contains(tile))
                            {
                                Tile otherHalfTile = tile.OtherHalf;
                                if (otherHalfTile != null)
                                {
                                    otherHalfTile.OtherHalf = null;
                                }
                                newBoard[row, col] = null;
                            }
                        }
                    }

                    //Drop Unsupported Pills
                    for (int row = 0; row < newBoard.GetLength(0); row++)
                    {
                        for (int col = 0; col < newBoard.GetLength(1); col++)
                        {
                            Tile tile = newBoard[row, col];
                            Tile tile2 = tile?.OtherHalf;

                            if (tile2 != null)
                            {
                                Position position1 = new Position(row, col);
                                Position position2 = tile2.Position;

                                if (position1.Y == position2.Y)
                                {
                                    int dropIndex = col;
                                    while (dropIndex > 0 && newBoard[position1.X, dropIndex - 1] == null && newBoard[position2.X, dropIndex - 1] == null)
                                    {
                                        dropIndex -= 1;
                                    }

                                    if (dropIndex != col)
                                    {
                                        newBoard[position1.X, dropIndex] = tile;
                                        newBoard[position1.X, col] = null;
                                        newBoard[position2.X, dropIndex] = tile2;
                                        newBoard[position2.X, col] = null;
                                    }
                                }
                            }
                            else
                            {
                                int dropIndex = col;
                                while (dropIndex > 0 && newBoard[row, dropIndex - 1] == null)
                                {
                                    dropIndex -= 1;
                                }

                                if (dropIndex != col)
                                {
                                    newBoard[row, dropIndex] = tile;
                                    newBoard[row, col] = null;
                                }
                            }
                        }
                    }
                } while (removedTiles.Count > 0);

                double score = Evaluator.Evaluate(newBoard);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPill = pill;
                }
            }

            return bestPill;
        }
        */
    }

}
