using DrMarioPlayer.Model;

namespace DrMarioPlayer.Converter
{
    internal static class TileConverter
    {
        public static Tile Convert(string code)
        {
            switch (code)
            {
                case "Red": return Tile.RED;
                case "Green": return Tile.GREEN;
                case "Blue": return Tile.BLUE;
                case "Yellow": return Tile.YELLOW;
                case "Special": return Tile.SPECIAL;
                case "RV": return Tile.RED_VIRUS;
                case "GV": return Tile.GREEN_VIRUS;
                case "BV": return Tile.BLUE_VIRUS;
                case "YV": return Tile.YELLOW_VIRUS;
                default: return Tile.NONE;
            }
        }

        public static Tile Convert(Color color)
        {
            switch (color)
            {
                case Color.RED: return Tile.RED;
                case Color.GREEN: return Tile.GREEN;
                case Color.BLUE: return Tile.BLUE;
                case Color.YELLOW: return Tile.YELLOW;
                case Color.WHITE: return Tile.SPECIAL;
                default: return Tile.NONE;
            }
        }
    }
}
