using BT05;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class ScreenThanks : GameScreenExtended
    {
        public ScreenThanks(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_TIMER;
            _advanceModeTime = 10.0f;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "circlefill", animScreenRect, -90, 10, AnimationType.ONCE);

            _nextPhase = GamePhase.ATTRACT;
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            base.ScreenArriving();

            PrimaryText = "{{WHITE}}Thanks for playing!\n Learn more about CAS9 and the other gene editing tools here in the Frontiers Gallery.";
            SecondaryText = "{{GREEN}}Game will restart shortly!";

            ShowPrimaryText = true;
            ShowSecondaryText = true;

            //_timeInState = 3.0f;
            //_leavingToNextState = false;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);

            //_timeInState -= gameTime.GetElapsedSeconds();

            //if (_timeInState < 0)
            //{
            //    if (!_leavingToNextState)
            //    {
            //        _leavingToNextState = true;
            //        GameManager.Instance.GotoPhase(GamePhase.ATTRACT);
            //    }
            //}

            base.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            if (_defaultAnimation != null)
            {
                _defaultAnimation.DrawSpriteSheetAnim();
            }

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            base.DrawInner(gameTime);
        }
    }
}