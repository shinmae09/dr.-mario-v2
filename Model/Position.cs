namespace DrMarioPlayer.Model
{
    internal class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool IsEqual(object obj)
        {
            return obj is Position objPos
                && this.X == objPos.X 
                && this.Y == objPos.Y;
        }
    }
}
