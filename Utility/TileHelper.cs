using DrMarioPlayer.Model;

namespace DrMarioPlayer.Utility
{
    internal static class TileHelper
    {
        public static bool IsVirus(Tile tile)
        {
            switch (tile)
            {
                case Tile.RED_VIRUS:
                case Tile.GREEN_VIRUS:
                case Tile.BLUE_VIRUS:
                case Tile.YELLOW_VIRUS: return true;
            }

            return false;
        }

        /*
        public static bool IsTileSimilar(Tile firstTile, Tile secondTile)
        {
            return firstTile == secondTile
                || ( firstTile is Tile && secondTile is Tile
                && firstTile.Color == secondTile.Color
                && firstTile.IsVirus == secondTile.IsVirus);
        }
        */
    }
}
