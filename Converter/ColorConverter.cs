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
    }
}
