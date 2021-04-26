using DrMarioPlayer.Model;

namespace DrMarioPlayer.Converter
{
    internal static class ColorConverter
    {
        public static Color? Convert(char code)
        {
            switch (code)
            {
                case 'R': return Color.RED;
                case 'G': return Color.GREEN;
                case 'B': return Color.BLUE;
                case 'Y': return Color.YELLOW;
                case 'S':
                case 'P': return Color.WHITE;
            }

            return null;
        }

        public static Color? Convert(Tile tile)
        {
            switch (tile)
            {
                case Tile.RED:
                case Tile.RED_VIRUS: return Color.RED;
                case Tile.GREEN:
                case Tile.GREEN_VIRUS: return Color.GREEN;
                case Tile.BLUE:
                case Tile.BLUE_VIRUS: return Color.BLUE;
                case Tile.YELLOW:
                case Tile.YELLOW_VIRUS: return Color.YELLOW;
                case Tile.SPECIAL: return Color.WHITE;
            }

            return null;
        }
    }
}
