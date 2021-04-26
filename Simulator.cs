using DrMarioPlayer.Converter;
using DrMarioPlayer.Model;
using DrMarioPlayer.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DrMarioPlayer
{
    internal static class Simulator
    {
        public static Dictionary<Pill, Tuple<Tile[,], Partner[,]>> LockPills(Tile[,] board, Partner[,] partner, List<Pill> pills)
        {
            Dictionary<Pill, Tuple<Tile[,], Partner[,]>> gameBoards = new Dictionary<Pill, Tuple<Tile[,], Partner[,]>>();

            foreach (var pill in pills)
            {            
                Tile[,] gameBoard = (Tile[,])board.Clone();
                Partner[,] partnerBoard = (Partner[,])partner.Clone();

                Tile tile1 = TileConverter.Convert(pill.Color1.Value);
                Tile tile2 = TileConverter.Convert(pill.Color2.Value);

                switch (pill.Orientation)
                {
                    case Orientation.HORIZONTAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = tile1;
                        gameBoard[pill.Position.X + 1, pill.Position.Y] = tile2;

                        partnerBoard[pill.Position.X, pill.Position.Y] = Partner.RIGHT;
                        partnerBoard[pill.Position.X + 1, pill.Position.Y] = Partner.LEFT;
                        break;

                    case Orientation.VERTICAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = tile1;
                        gameBoard[pill.Position.X, pill.Position.Y + 1] = tile2;
                        break;

                    case Orientation.REVERSE_HORIZONTAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = tile2;
                        gameBoard[pill.Position.X + 1, pill.Position.Y] = tile1;

                        partnerBoard[pill.Position.X, pill.Position.Y] = Partner.RIGHT;
                        partnerBoard[pill.Position.X + 1, pill.Position.Y] = Partner.LEFT;
                        break;

                    case Orientation.REVERSE_VERTICAL:
                        gameBoard[pill.Position.X, pill.Position.Y] = tile2;
                        gameBoard[pill.Position.X, pill.Position.Y + 1] = tile1;
                        break;
                }

                RemoveTiles(gameBoard, partnerBoard);

                gameBoards.Add(pill, new Tuple<Tile[,], Partner[,]>(gameBoard, partnerBoard));
            }

            return gameBoards;
        }

        private static void RemoveTiles(Tile[,] gameBoard, Partner[,] partnerBoard)
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
                            Position tilePosition = new Position(row, col);
                            if (removedTiles.Any(pos => pos.IsEqual(tilePosition)))
                            {
                                gameBoard[row, col] = Tile.NONE;

                                switch(partnerBoard[row, col])
                                {
                                    case Partner.LEFT:
                                        partnerBoard[row - 1, col] = Partner.NONE;
                                        break;

                                    case Partner.RIGHT:
                                        partnerBoard[row + 1, col] = Partner.NONE;
                                        break;
                                }
                                partnerBoard[row, col] = Partner.NONE;
                            }
                        }
                    }

                    DropUnsupportedPills(gameBoard, partnerBoard);
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
                    Color? tileColor = ColorConverter.Convert(tile);
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

                            if (tileColor == Color.WHITE && consecutiveColorsLength == 2)
                            {
                                int[] vaccinatedRows = new int[] { col, col - 1 };
                                foreach (int vaccinatedRow in vaccinatedRows)
                                {
                                    for (int x = 0; x < gameBoard.GetLength(0); x++)
                                    {
                                        Position tilePosition = new Position(x, vaccinatedRow);
                                        if (!removedTiles.Any(pos => pos.IsEqual(tilePosition)))
                                        {
                                            removedTiles.Add(tilePosition);
                                        }
                                    }
                                }
                            }
                            else if (consecutiveColorsLength >= 4)
                            {
                                for (int i = 0; i < consecutiveColorsLength; i++)
                                {
                                    Position tilePosition = new Position(row, col - i);
                                    if (!removedTiles.Any(pos => pos.IsEqual(tilePosition)))
                                    {
                                        removedTiles.Add(tilePosition);
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
                    Color? tileColor = ColorConverter.Convert(tile);
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

                            if (tileColor == Color.WHITE && consecutiveColorsLength == 2)
                            {
                                int[] vaccinatedCols = new int[] { row, row - 1 };
                                foreach(int vaccinatedCol in vaccinatedCols)
                                {
                                    for(int y = 0; y < gameBoard.GetLength(1); y++)
                                    {
                                        Position tilePosition = new Position(vaccinatedCol, y);
                                        if (!removedTiles.Any(pos => pos.IsEqual(tilePosition)))
                                        {
                                            removedTiles.Add(tilePosition);
                                        }
                                    }
                                }
                            }
                            else if (consecutiveColorsLength >= 4)
                            {
                                for (int i = 0; i < consecutiveColorsLength; i++)
                                {
                                    Position tilePosition = new Position(row - i, col);
                                    if (!removedTiles.Any(pos => pos.IsEqual(tilePosition)))
                                    {
                                        removedTiles.Add(tilePosition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void DropUnsupportedPills(Tile[,] gameBoard, Partner[,] partnerBoard)
        {
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    Tile tile = gameBoard[row, col];
                    if (tile != Tile.NONE && !TileHelper.IsVirus(tile))
                    {
                        Partner partner = partnerBoard[row, col];
                        if (partner != Partner.NONE)
                        {
                            int dropIndex = col;
                            int partnerX = 0;
                            if (partner == Partner.LEFT)
                            {
                                partnerX = row - 1;
                            }
                            else if (partner == Partner.RIGHT)
                            {
                                partnerX = row + 1;
                            }

                            while ( dropIndex > 0
                                    && gameBoard[row, dropIndex - 1] == Tile.NONE
                                    && gameBoard[partnerX, dropIndex - 1] == Tile.NONE)
                            {
                                dropIndex -= 1;
                            }

                            if (dropIndex != col)
                            {
                                gameBoard[row, dropIndex] = tile;
                                gameBoard[row, col] = Tile.NONE;
                                gameBoard[partnerX, dropIndex] = gameBoard[partnerX, col];
                                gameBoard[partnerX, col] = Tile.NONE;

                                partnerBoard[row, dropIndex] = partnerBoard[row, col];
                                partnerBoard[row, col] = Partner.NONE;
                                partnerBoard[partnerX, dropIndex] = partnerBoard[partnerX, col];
                                partnerBoard[partnerX, col] = Partner.NONE;
                            }
                        }
                        else
                        {
                            int dropIndex = col;
                            while (dropIndex > 0 && gameBoard[row, dropIndex - 1] == Tile.NONE)
                            {
                                dropIndex -= 1;
                            }

                            if (dropIndex != col)
                            {
                                gameBoard[row, dropIndex] = tile;
                                gameBoard[row, col] = Tile.NONE;
                            }
                        }
                    } 
                }
            }
        }
        
    }
}
