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
        public ScreenPrototypeMessages(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_TIMER;
            _advanceModeTime = 10.0f;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "circlefill", animScreenRect, -90, 10, AnimationType.ONCE);
        }

        public override void ScreenArriving()
        {
            //_animLooping = AnimationManager.Instance.GetAnimation("timer");
            //_anim = AnimationManager.Instance.GetAnimation("circlefill");
            //_anim = AnimationManager.Instance.GetAnimation("countdown");
            //_anim = AnimationManager.Instance.GetAnimation("dna");
            //_anim = AnimationManager.Instance.GetAnimation("handwave");
            //_anim = AnimationManager.Instance.GetAnimation("sandtimer");

            base.ScreenArriving();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            base.DrawInner(gameTime);
        }
    }
}