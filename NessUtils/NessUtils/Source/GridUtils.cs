using System;


namespace Ness.Utils.Utils
{
    /// <summary>
    /// Utils to handle grid.
    /// </summary>
    public static class GridUtils
    {
        /// <summary>
        /// Grid index.
        /// </summary>
        public struct GridIndex
        {
            GridIndex(int x, int y)
            {
                X = x;
                Y = y;
            }
            public int X;
            public int Y;
        }

        /// <summary>
        /// Put grid index in valid range.
        /// </summary>
        public static GridIndex PutInRange(GridIndex index, GridIndex max)
        {
            if (index.X < 0) index.X = 0;
            if (index.Y < 0) index.Y = 0;
            if (index.X > max.X) index.X = max.X;
            if (index.Y > max.Y) index.Y = max.Y;
            return index;
        }

        /// <summary>
        /// A class to iterate a grid in an outward spiral.
        /// Usage: Instanciate this class and call GoNext() every time you want to advance step.
        /// Use X and Y to get current step, relative to 0.
        /// </summary>
        public class SpiralOut
        {
            int _layer;
            int _leg;

            /// <summary>
            /// Output X for every step.
            /// </summary>
            public int X { get; private set; }

            /// <summary>
            /// Output Y for every step.
            /// </summary>
            public int Y { get; private set; }

            /// <summary>
            /// Create the spiral walker.
            /// </summary>
            public SpiralOut()
            {
                Reset();
            }

            /// <summary>
            /// Reset the spiral.
            /// </summary>
            public void Reset()
            {
                _layer = 1;
                _leg = 0;
                X = 0;
                Y = 0;
            }

            /// <summary>
            /// Go to next step.
            /// </summary>
            public void GoNext()
            {
                switch (_leg)
                {
                    case 0: ++X; if (X == _layer) ++_leg; break;
                    case 1: ++Y; if (Y == _layer) ++_leg; break;
                    case 2: --X; if (-X == _layer) ++_leg; break;
                    case 3: --Y; if (-Y == _layer) { _leg = 0; ++_layer; } break;
                }
            }
        };
    }
}
