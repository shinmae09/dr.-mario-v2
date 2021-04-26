using DrMarioPlayer.Converter;
using DrMarioPlayer.Utility;

namespace DrMarioPlayer.Model
{
    internal class Pill
    {
        private static readonly Position SPAWN_POSITION = new Position(3, 13);
        private static readonly Orientation DEFAULT_ORIENTATION = Orientation.HORIZONTAL;

        public Color? Color1 { get; set; }
        public Color? Color2 { get; set; }
        public Orientation Orientation { get; set; }
        public Position Position { get; set; }

        public Pill()
        {
            Orientation = DEFAULT_ORIENTATION;
            Position = SPAWN_POSITION;
        }

        public Pill(string colorCode) : this()
        {
            Color1 = ColorConverter.Convert(colorCode[0]);
            Color2 = ColorConverter.Convert(colorCode[1]);
        }

        public Pill Move(Move move)
        {
            Pill pill = this.Clone();
            switch (move)
            {
                case Model.Move.LEFT:
                    pill.Position.X -= 1;
                    break;

                case Model.Move.RIGHT:
                    pill.Position.X += 1;
                    break;

                case Model.Move.DOWN:
                    pill.Position.Y -= 1;
                    break;

                case Model.Move.ROTATE_90:
                    pill.Orientation = OrientationHelper.Rotate(pill.Orientation, 1);
                    if (pill.Orientation == Orientation.REVERSE_VERTICAL)
                    {
                        pill.Position.Y -= 1;
                    }
                    break;

                case Model.Move.ROTATE_180:
                    pill.Orientation = OrientationHelper.Rotate(pill.Orientation, 2);
                    break;

                case Model.Move.ROTATE_270:
                    pill.Orientation = OrientationHelper.Rotate(pill.Orientation, 3);
                    break;
            }

            return pill;
        }

        public Pill Clone()
        {
            Pill pill = new Pill()
            {
                Color1 = this.Color1,
                Color2 = this.Color2,
                Orientation = this.Orientation,
                Position = new Position(this.Position.X, this.Position.Y),
            };

            return pill;
        }
    }
}
