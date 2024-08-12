using MonoGame.Extended.Tweening;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using MonoGame.Extended.Sprites;
//using SharpDX.Direct3D9;
//using System.Drawing;

namespace SharedMonoGame
{
    public enum DockingLocation { TopLeft, TopRight, TopCentre, BottomLeft, BottomCentre, BottomRight, MiddleLeft, MiddleRight, Center }

    /// <summary>
    /// Tweener Sprite combines Sprite, Tweener and a Transform together
    /// It simplifies the animation (movement / fading)
    /// 
    /// This also supports docking to the side of the screen
    /// </summary>
    public class TweenerSprite
    {
        public Sprite MySprite { get; set; }
        public Tweener MyTweener { get; set; }
        public Transform2 MyTransform { get; set; }

        public TweenerSprite(Texture2D myTexture)//, Tweener myTweener)
        {
            MySprite = new Sprite(myTexture);
            MyTweener = new Tweener();
            MyTransform = new Transform2();
        }

        /// <summary>
        /// When the sprite is drawn to the screen - what is it exact screen coords?
        /// </summary>
        public Rectangle ScreenSpaceRenderRect
        {
            get
            {
                Rectangle r = new Rectangle();

                float textureWidth = MySprite.TextureRegion.Width * MyTransform.Scale.X;
                float textureHeight = MySprite.TextureRegion.Height * MyTransform.Scale.Y;

                r.Width = (int)textureWidth;
                r.Height = (int)textureHeight;

                r.X = (int)(MyTransform.Position.X - textureWidth / 2);
                r.Y = (int)(MyTransform.Position.Y - textureHeight / 2);

                return r;
            }
        }

        /// <summary>
        /// This calculates the MyTransform position (taking into account scale) when passed a Docking Location
        /// You can also pass in an offset
        /// </summary>
        /// <param name="dockingLocation"></param>
        /// <param name="offset"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void DockTo(DockingLocation dockingLocation, Vector2 offset)
        {
            Vector2 screenCoordinates = Vector2.Zero;

            float textureWidth = MySprite.TextureRegion.Width * MyTransform.Scale.X;
            float textureHeight = MySprite.TextureRegion.Height * MyTransform.Scale.Y;

            Vector2 centreOffset = new Vector2(textureWidth, textureHeight) / 2;

            int screenWidth = 1920;
            int screenHeight = 1080;

            switch (dockingLocation)
            {
                case DockingLocation.TopLeft:
                    screenCoordinates = Vector2.Zero;
                    break;
                case DockingLocation.TopRight:
                    screenCoordinates = new Vector2(screenWidth - textureWidth, 0);
                    break;
                case DockingLocation.TopCentre:
                    screenCoordinates = new Vector2(screenWidth - textureWidth / 2, 0);
                    break;
                case DockingLocation.BottomLeft:
                    screenCoordinates = new Vector2(0, screenHeight - textureHeight);
                    break;
                case DockingLocation.BottomRight:
                    screenCoordinates = new Vector2(screenWidth - textureWidth, screenHeight - textureHeight);
                    break;
                case DockingLocation.BottomCentre:
                    screenCoordinates = new Vector2(screenWidth - textureWidth / 2, 0);
                    break;
                case DockingLocation.MiddleLeft:
                    screenCoordinates = new Vector2(0, (screenHeight - textureHeight) / 2);
                    break;
                case DockingLocation.MiddleRight:
                    screenCoordinates = new Vector2(screenWidth - textureWidth, (screenHeight - textureHeight) / 2);
                    break;
                case DockingLocation.Center:
                    screenCoordinates = new Vector2((screenWidth - textureWidth) / 2, (screenHeight - textureHeight) / 2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dockingLocation), $"Unsupported docking location: {dockingLocation}");
            }

            MyTransform.Position = screenCoordinates + centreOffset;
        }
    }
}
