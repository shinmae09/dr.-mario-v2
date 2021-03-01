using DrMarioPlayer.Model;
using System;

namespace DrMarioPlayer
{
    internal static class Evaluator
    {
        public static double Evaluate(Tile[,] gameBoard)
        {
            return 100.00 * measureViruses(gameBoard) 
                + measureTiles(gameBoard) 
                + measureStackHeight(gameBoard) 
                + measureChangeInColors(gameBoard)
                + measureVirusProximity(gameBoard);
        }

        private static double measureTiles(Tile[,] gameBoard)
        {
            double totalArea = Config.Environment.HEIGHT * Config.Environment.WIDTH;
            double totalTiles = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    if (gameBoard[row, col] != null)
                    {
                        totalTiles++;
                    }
                }
            }

            return (totalArea - totalTiles) / totalArea;
        }

        private static double measureViruses(Tile[,] gameBoard)
        {
            double totalArea = Config.Environment.HEIGHT * Config.Environment.WIDTH;
            double totalViruses = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    if (gameBoard[row, col] != null && gameBoard[row, col].IsVirus)
                    {
                        totalViruses++;
                    }
                }
            }

            return (totalArea - totalViruses) / totalArea;
        }

        private static double measureChangeInColors(Tile[,] gameBoard)
        {
            double totalArea = (Config.Environment.HEIGHT * Config.Environment.WIDTH) * 2;
            double totalColorChanges = 0.0;

            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                Color? lastColor = null;
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Tile tile = gameBoard[row, col];
                    Color? tileColor = tile?.Color;
                    if (tileColor != null && tileColor != lastColor)
                    {
                        lastColor = tileColor;
                        totalColorChanges++;
                    }
                }
            }

            for (int col = 0; col < gameBoard.GetLength(1); col++)
            {
                Color? lastColor = null;
                for (int row = 0; row < gameBoard.GetLength(0); row++)
                {
                    Tile tile = gameBoard[row, col];
                    Color? tileColor = tile?.Color;
                    if (tileColor != null && tileColor != lastColor)
                    {
                        lastColor = tileColor;
                        totalColorChanges++;
                    }
                }
            }

            return (totalArea - totalColorChanges) / totalArea;
        }

        private static double measureStackHeight(Tile[,] gameBoard)
        {
            double totalArea = (Config.Environment.HEIGHT * Config.Environment.WIDTH) * 2;
            double totalStackHeight = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = Config.Environment.HEIGHT - 1; col >= 0; col--)
                {
                    if (gameBoard[row, col] != null)
                    {
                        if (col < Config.Environment.HEIGHT - 1)
                        {
                            totalStackHeight += Config.Environment.WIDTH * 2;
                        }
                        totalStackHeight += Config.Environment.HEIGHT - col - 1;
                        break;
                    }
                }
            }

            return totalStackHeight / (totalArea);
        }

        private static double measureVirusProximity(Tile[,] gameBoard)
        {
            double maxDistance = Math.Pow(Config.Environment.HEIGHT - 1, 2);
            double totalVirusProximity = 0.0;
            double totalViruses = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = Config.Environment.HEIGHT - 1; col >= 0; col--)
                {
                    Tile tile = gameBoard[row, col];
                    if (tile?.IsVirus == true)
                    {
                        Color virusColor = tile.Color;
                        totalViruses++;

                        for (int iRow = 0; iRow < gameBoard.GetLength(0); iRow++)
                        {
                            if (iRow != row)
                            {
                                Tile scanTile = gameBoard[iRow, col];
                                if (scanTile != null)
                                {
                                    if (scanTile.Color == virusColor)
                                    {
                                        int dist = iRow - row;
                                        totalVirusProximity += (maxDistance - dist * dist) * Config.Environment.WIDTH;
                                    }                   
                                }
                                else
                                {
                                    int dist = iRow - row;
                                    totalVirusProximity += (maxDistance - dist * dist);
                                }
                            }
                        }

                        for (int iCol = 0; iCol < gameBoard.GetLength(1); iCol++)
                        {
                            if (iCol != col)
                            {
                                Tile scanTile = gameBoard[row, iCol];
                                if (scanTile != null)
                                {
                                    if (scanTile.Color == virusColor)
                                    {
                                        int dist = iCol - col;
                                        totalVirusProximity += (maxDistance - dist * dist) * Config.Environment.WIDTH;
                                    }
                                }
                                else
                                {
                                    int dist = iCol - col;
                                    totalVirusProximity += (maxDistance - dist * dist);
                                }
                            }
                        }
                    }
                }
            }

            if (totalViruses == 0)
            {
                return 1.0;
            }

            return totalVirusProximity / (18910.0 * totalViruses);
        }
    }
}
