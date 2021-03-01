using DrMarioPlayer.Model;
using System.Threading.Tasks;

namespace DrMarioPlayer.Utility
{
    internal static class TileArrayExtension
    {
        /*
        public static Position PositionOf(this Tile[,] gameBoard, Tile tile)
        {
            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < gameBoard.GetLength(1); col++)
                {
                    if (gameBoard[row, col] != null && gameBoard[row, col].Equals(tile))
                    {
                        return new Position(row, col);
                    }
                }
            }

            return null;
        }
        */

        public static string LogGameBoard(this Tile[,] gameBoard)
        {
            string board = "";

            for (int col = gameBoard.GetLength(1) - 1; col >= 0; col--)
            {
                for (int row = 0; row < gameBoard.GetLength(0); row++)
                {
                    if (gameBoard[row, col] == null)
                    {
                        board += "NN  ";
                    }
                    else
                    {
                        switch(gameBoard[row, col].Color)
                        {
                            case Color.BLUE:
                                board += "BB  ";
                                break;
                            case Color.RED:
                                board += "RR  ";
                                break;
                            case Color.GREEN:
                                board += "GG  ";
                                break;
                            case Color.YELLOW:
                                board += "YY  ";
                                break;
                            case Color.WHITE:
                                board += "WW  ";
                                break;
                        }
                    }
                }
                board += "\n";
            }
            
            
            return board;
        }

        public static Tile[,] DeepClone(this Tile[,] gameBoard)
        {
            Tile[,] boardClone = new Tile[Config.Environment.WIDTH, Config.Environment.HEIGHT];

            Parallel.For(0, gameBoard.GetLength(0), (row) =>
            {
                Parallel.For(0, gameBoard.GetLength(1), (col) =>
                {
                    if (gameBoard[row, col] != null)
                    {
                        boardClone[row, col] = gameBoard[row, col].DeepClone();
                    }
                });
            });

            return boardClone;
        }
    }
}
