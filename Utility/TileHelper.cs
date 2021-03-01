using DrMarioPlayer.Model;

namespace DrMarioPlayer.Utility
{
    internal static class TileHelper
    {
        public static bool IsTileSimilar(Tile firstTile, Tile secondTile)
        {
            return firstTile == secondTile
                || ( firstTile is Tile && secondTile is Tile
                && firstTile.Color == secondTile.Color
                && firstTile.IsVirus == secondTile.IsVirus);
        }
    }
}
