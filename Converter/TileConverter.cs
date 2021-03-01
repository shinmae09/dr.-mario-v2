using DrMarioPlayer.Model;

namespace DrMarioPlayer.Converter
{
    internal static class TileConverter
    {
        public static Tile Convert(string code, int row, int col)
        {
            Tile tile;

            switch (code)
            {
                case "Red":
                    tile = new Tile(Color.RED);
                    break;

                case "Green":
                    tile = new Tile(Color.GREEN);
                    break;

                case "Blue":
                    tile = new Tile(Color.BLUE);
                    break;

                case "Yellow":
                    tile = new Tile(Color.YELLOW);
                    break;

                case "Special":
                    tile = new Tile(Color.WHITE);
                    break;

                case "RV":
                    tile = new Tile(Color.RED, true);
                    break;

                case "GV":
                    tile = new Tile(Color.GREEN, true);
                    break;

                case "BV":
                    tile = new Tile(Color.BLUE, true);
                    break;

                case "YV":
                    tile = new Tile(Color.YELLOW, true);
                    break;

                default: return null;
            }

            tile.Position = new Position(row, col);
            return tile;
        }
    }
}
