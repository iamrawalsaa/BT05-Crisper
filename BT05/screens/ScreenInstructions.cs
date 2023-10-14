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
    public class ScreenInstructions : GameScreenExtended
    {
        //Animation _animLooping;

        public override void ScreenArriving()
        {
            //_animLooping = AnimationManager.Instance.GetAnimation("handwave");

            //if (_animLooping != null)
            //{
            //    _animLooping.SetAnimationLength(15);
            //    _animLooping.Reset();
            //    _animLooping.AnimationType = AnimationType.LOOPING;
            //}
            base.ScreenArriving();
        }

        public ScreenInstructions(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_WAVE;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "handwave", animScreenRect, -90, 15, AnimationType.LOOPING);
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
            //Rectangle animScreenRect = new Rectangle(1700,540,200,200);
            //DrawSpriteSheetAnim(_animLooping, animScreenRect, -90);

            base.DrawInner(gameTime);

        }
    }
}