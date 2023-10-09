using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMonoGame
{
    /// <summary>
    /// Extension methods specific to XNA
    /// </summary>
    static public class XNAExtensionMethods
    {
        /// <summary>
        /// This just centers the Texture when it draws it. Usually, textures are drawn from the top left
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="texture2D"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void DrawCentered(this SpriteBatch sprite, Texture2D texture2D, Vector2 position, Color color)
        {
            Vector2 offset = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            sprite.Draw(texture2D, position - offset, color);
        }

        public static void DrawCentered(this SpriteBatch sprite, Texture2D texture2D, Vector2 position, float scale, Color color)
        {
            Rectangle rect = new Rectangle(0, 0, (int)(texture2D.Width * scale), (int)(texture2D.Height * scale));

            Vector2 offset = new Vector2(rect.Width / 2, rect.Height / 2);

            rect.X = (int)(position.X - offset.X);
            rect.Y = (int)(position.Y - offset.Y);

            sprite.Draw(texture2D, rect, color);
        }

        public static void DrawTextCentered(this SpriteBatch sprite, SpriteFont font, string text, Vector2 position, Color color)
        {
            Vector2 offset = font.MeasureString(text) / 2;
            sprite.DrawString(font, text, position - offset, color);
        }

        /// <summary>
        /// Aspect ratio as a decimal
        /// width:height - e.g 16:9
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static float AspectRatio(this Texture2D texture)
        {
            return texture.Width / (float)texture.Height;
        }

//        public static void SetCentre(this Rectangle rect, Vector2 centre)
//        {
//  //          if (rect.Width != 0 && rect.Height != 0)
//            //{
//                rect.X = (int)(centre.X - rect.Width / 2);
//                rect.Y = (int)(centre.Y - rect.Height / 2);
////            }
//        }
    }
}