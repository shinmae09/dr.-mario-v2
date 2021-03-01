using System;

namespace DrMarioPlayer.Model
{
    [Serializable]
    internal class Tile
    {
        public Color Color { get; set; }
        public bool IsVirus { get; set; }
        public Tile OtherHalf { get; set; }
        public Position Position { get; set; }

        public Tile(Color color)
        {
            this.Color = color;
        }

        public Tile(Color color, bool isVirus) : this(color)
        {
            this.IsVirus = isVirus;
        }

        public Tile Clone()
        {
            Tile tile = new Tile(Color, IsVirus)
            {
                Position = new Position(Position.X, Position.Y),
            };

            if (OtherHalf != null)
            {
                Tile otherHalf = new Tile(OtherHalf.Color, OtherHalf.IsVirus)
                {
                    Position = new Position(OtherHalf.Position.X, OtherHalf.Position.Y),
                };

                tile.OtherHalf = otherHalf;
                otherHalf.OtherHalf = tile;
            }

            return tile;
        }
    }
}
