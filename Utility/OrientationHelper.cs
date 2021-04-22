using DrMarioPlayer.Model;

namespace DrMarioPlayer.Utility
{
    internal static class OrientationHelper
    {
        public static Orientation Rotate(Orientation orientation, int times)
        {
            int val = (int)orientation + times;
            val = val % 4;
            return (Orientation) val;
        }
    }
}