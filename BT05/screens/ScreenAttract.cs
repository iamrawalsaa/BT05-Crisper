using BT05;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class ScreenAttract : GameScreenExtended
    {
        public ScreenAttract(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            PrimaryTextFontColour = Color.White;
            SecondaryTextFontColour = Color.White;

            GameManager.Instance.ResetGame();

            VideoManager.Instance.Play( VideoKey.attract);

            base.ScreenArriving();
        }

        public override void ScreenLeaving()
        {
            VideoManager.Instance.Stop();
            base.ScreenLeaving();
        }

        public override void ScreenExitCompleted()
        {
            VideoManager.Instance.Stop();// VideoKey.attract);

            base.ScreenExitCompleted();
        }

        public override void Update(GameTime gameTime)
        {
//            PrimaryTextOffset = new Vector2(100, 100);

            UpdateDrawables(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            var texture = VideoManager.Instance.GetVideoTexture();

            if (texture != null )
            {
                Vector2 centre = new Vector2( texture.Width / 2, texture.Height / 2 );
                //centre = new Vector2(0, 0);
                Rectangle screenRect = new Rectangle(0,1080, 1080,1920);
                //screenRect = new Rectangle(100, 100, 500, 500);

                centre = new Vector2(screenRect.Width, screenRect.Height) / 2;
                centre = new Vector2(0, 0);
                float rotation = 0;
                rotation = (float)-Math.PI / 2.0f;

                Game._spriteBatch.Draw(texture, screenRect, null, Color.White, rotation, centre, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            }

            //CheckTextureNeedsRecreating();

            //Game.GraphicsDevice.Clear(Color.GhostWhite);
            //var transformMatrix = Game.Camera.GetViewMatrix();
            //Game._spriteBatch.Begin(transformMatrix: transformMatrix);
            //DrawPre(gameTime);
            //DrawTextTexture();
            //DrawPost(gameTime);

            //Game._spriteBatch.End();
        }
    }
}