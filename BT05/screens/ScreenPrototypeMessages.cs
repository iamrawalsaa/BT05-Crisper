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
using SharpDX.Direct3D9;

namespace screens
{
    public class ScreenPrototypeMessages : GameScreenExtended
    {
        public ScreenPrototypeMessages(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void ScreenArriving()
        {
            var anim = AnimationManager.Instance.GetAnimation("timer");

            if (anim != null)
            {
                anim.SetAnimationLength(8);
                anim.Reset();
            }
            base.ScreenArriving();
        }

        public override void Update(GameTime gameTime)
        {
            AnimationManager.Instance.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {

            var anim = AnimationManager.Instance.GetAnimation("timer");

            if (anim != null)
            {
                Game._spriteBatch.Draw(anim.Texture, Vector2.Zero, anim.GetCurrentFrameRect(), Microsoft.Xna.Framework.Color.White);
            }

            base.DrawInner(gameTime);
        }
    }
}