using BT05;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared;
using System.DirectoryServices;

namespace screens
{
    public class ScreenPrototypeMessages : GameScreenExtended
    {
        public ScreenPrototypeMessages(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void Update(GameTime gameTime)
        {
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {

            //var texture = SharedAssetManager.Instance.GetBackground(GamePhase.GAME);
            //Game._spriteBatch.Draw(texture, Vector2.Zero, Microsoft.Xna.Framework.Color.White);

            base.DrawInner(gameTime);
        }
    }
}