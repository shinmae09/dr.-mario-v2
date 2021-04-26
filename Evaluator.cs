using DrMarioPlayer.Converter;
using DrMarioPlayer.Model;
using DrMarioPlayer.Utility;
using System;

namespace DrMarioPlayer
{
    internal static class Evaluator
    {
        public static double Evaluate(Tile[,] gameBoard)
        {
            return 100.0 * measureViruses(gameBoard)
                + measureTiles(gameBoard)
                //+ measureStackHeight(gameBoard)
                + measureChangeInColors(gameBoard)
                + measureVirusProximity(gameBoard)
                + measureBlockings(gameBoard)
                + measureHeight(gameBoard);

        }

        private static double measureTiles(Tile[,] gameBoard)
        {
            double totalArea = Config.Environment.HEIGHT * Config.Environment.WIDTH;
            double totalTiles = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    if (gameBoard[row, col] != Tile.NONE)
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
                    if (TileHelper.IsVirus(gameBoard[row, col]))
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
                    Color? tileColor = ColorConverter.Convert(tile);
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
                    Color? tileColor = ColorConverter.Convert(tile);
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
                    if (gameBoard[row, col] != Tile.NONE)
                    {
                        if (col < Config.Environment.HEIGHT - 1)
                        {
                            totalStackHeight += 14;
                        }
                        totalStackHeight += Config.Environment.HEIGHT - col - 1;
                        break;
                    }
                    else if (gameBoard[row, col] == Tile.NONE && col == 0)
                    {
                        totalStackHeight += 28;
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
                    if (TileHelper.IsVirus(tile))
                    {
                        Color virusColor = ColorConverter.Convert(tile).Value;
                        totalViruses++;

                        for (int iRow = 0; iRow < gameBoard.GetLength(0); iRow++)
                        {
                            if (iRow != row)
                            {
                                Tile scanTile = gameBoard[iRow, col];
                                if (scanTile != Tile.NONE)
                                {
                                    Color scanTileColor = ColorConverter.Convert(scanTile).Value;
                                    if (scanTileColor == virusColor)
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
                                if (scanTile != Tile.NONE)
                                {
                                    Color scanTileColor = ColorConverter.Convert(scanTile).Value;
                                    if (scanTileColor == virusColor)
                                    {
                                        int dist = iCol - col;
                                        totalVirusProximity += (maxDistance - dist * dist) * Config.Environment.WIDTH + 10;
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

            return totalVirusProximity / (19840.0 * totalViruses);
            //return totalVirusProximity / (18910.0 * totalViruses);
            //return totalVirusProximity / (17360.0 * totalViruses);
            //return totalVirusProximity / (19840.0 * totalViruses);
        }

        private static double measureBlockings(Tile[,] gameBoard)
        {
            double totalArea = (Config.Environment.HEIGHT * Config.Environment.WIDTH);
            double totalBlockings = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Tile tile = gameBoard[row, col];
                    if (TileHelper.IsVirus(tile))
                    {
                        Color lastColor = ColorConverter.Convert(tile).Value;
                        int colorStreak = 0;
                        for (int iCol = col + 1; iCol < Config.Environment.HEIGHT; iCol++)
                        {
                            Tile topTile = gameBoard[row, iCol];
                            if (topTile != Tile.NONE)
                            {
                                if (TileHelper.IsVirus(topTile))
                                {
                                    break;
                                }
                                else
                                {
                                    Color topTileColor = ColorConverter.Convert(topTile).Value;
                                    if (topTileColor != lastColor)
                                    {
                                        totalBlockings += 14;
                                        lastColor = topTileColor;
                                        colorStreak = 1;
                                    }
                                    else
                                    {
                                        if (gameBoard[row, iCol - 1] == Tile.NONE && colorStreak > 1)
                                        {
                                            totalBlockings += 14;
                                            colorStreak = 1;
                                        }
                                        else
                                        {
                                            colorStreak++;
                                        }
                                    }
                                }
                            }
                        }

                        if (colorStreak > 1 && colorStreak < 4)
                        {
                            totalBlockings -= 3.5 * colorStreak;
                        }
                    }
                }
            }

            return (totalArea - totalBlockings) / totalArea;
        }

        private static double measureHeight(Tile[,] gameBoard)
        {
            double totalHeight = 0.0;
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = Config.Environment.HEIGHT - 1; col > 10; col--)
                {
                    if (gameBoard[row, col] != Tile.NONE)
                    {
                        totalHeight += col - 10;
                    }
                }
            }

            return 40.0 - totalHeight / 40.0;
        }
    }
}
