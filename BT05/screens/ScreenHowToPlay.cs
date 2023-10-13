using BT05;
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
        Animation _animLooping;

        public override void ScreenArriving()
        {
            _animLooping = AnimationManager.Instance.GetAnimation("handwave");

            if (_animLooping != null)
            {
                _animLooping.SetAnimationLength(15);
                _animLooping.Reset();
                _animLooping.AnimationType = AnimationType.LOOPING;
            }
            base.ScreenArriving();
        }

        public ScreenHowToPlay(MyGameBase game, shared.GamePhase phase) : base(game, phase) { 
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_WAVE;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);
            base.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            Rectangle rect = new Rectangle(1650, 700, 200, 200);
            Vector2 origin = new Vector2(0, 0);

            if (_animLooping != null)
            {
                Game._spriteBatch.Draw(_animLooping.Texture, rect, _animLooping.GetCurrentFrameRect(), Microsoft.Xna.Framework.Color.White, -90, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
        }
    }
}