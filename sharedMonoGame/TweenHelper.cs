using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMonoGame
{
    public enum OffScreenDirection
    {
        North, South, East, West
    }

    /// <summary>
    /// This is used to store general Tweening functionality
    /// e.g. standard animations that we use again and again. Fade In / Fade Out.
    /// TODO: can probably be combined with TweenerSprite
    /// </summary>
    public sealed class TweenHelper
    {
        private static readonly TweenHelper _instance = new TweenHelper();

        public static TweenHelper Instance
        {
            get { return _instance; }
        }

        int width = 1920;
        int height = 1080;

        /// <summary>
        /// Used to help animate a sliding on TS
        /// Given an existing location of a TS and a direction. This calculates the offscreen starting position to animate from.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Vector2 CalculateOffscreen_StraightLine(TweenerSprite ts, OffScreenDirection direction, float offset = 50)
        {
            var rect = ts.ScreenSpaceRenderRect;

            Vector2 result = ts.MyTransform.Position;

            switch (direction)
            {
                case OffScreenDirection.North:
                    // x stays the same
                    // y goes to 0 - half height - offset
                    result.Y = -offset - rect.Height / 2;
                    break;
                case OffScreenDirection.South:
                    result.Y = height + offset + rect.Height / 2;
                    break;
                case OffScreenDirection.West:
                    // y stays the same
                    result.X = -offset - rect.Width / 2;
                    break;
                case OffScreenDirection.East:
                    result.X = width + offset + rect.Width / 2;
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
