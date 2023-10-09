using Microsoft.Xna.Framework;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class ScreenHowToPlay : GameScreenExtended
    {
        public ScreenHowToPlay(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
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