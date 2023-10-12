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

        Animation _anim;

        public override void ScreenArriving()
        {
            //_anim = AnimationManager.Instance.GetAnimation("timer");
            //_anim = AnimationManager.Instance.GetAnimation("circlefill");
            _anim = AnimationManager.Instance.GetAnimation("countdown");
            //_anim = AnimationManager.Instance.GetAnimation("dna");
            //_anim = AnimationManager.Instance.GetAnimation("handwave");
            //_anim = AnimationManager.Instance.GetAnimation("sandtimer");



            if (_anim != null)
            {
                _anim.SetAnimationLength(10);
                _anim.Reset();
            }
            base.ScreenArriving();
        }

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

            //var anim = AnimationManager.Instance.GetAnimation("timer");

            if (_anim != null)
            {
                Game._spriteBatch.Draw(_anim.Texture, Vector2.Zero, _anim.GetCurrentFrameRect(), Microsoft.Xna.Framework.Color.White);
            }

            base.DrawInner(gameTime);
        }
    }
}