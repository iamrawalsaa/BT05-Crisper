using BT05;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class ScreenSummaryGame : GameScreenExtended
    {
        private float _timeInState; // :H: used to change the timer
        private bool _leavingToNextState = false;

        public ScreenSummaryGame(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_TIMER; // :H: if this is set to advance timer mode then we can upadte the time
            _advanceModeTime = 20.0f; // :H: TODO can update the timer of the screen

            //Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            //_defaultAnimation = new OnScreenAnimation(Game, "circlefill", animScreenRect, -90, 10, AnimationType.ONCE);
        }

        public override void ScreenArriving()
        {
            _timeInState = 10f;
            _leavingToNextState = false;
            base.ScreenArriving();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            GameTimeout(gameTime); // :H: Created a text timer
            UpdateDrawables(gameTime);
            base.Update(gameTime);
        }

        private void GameTimeout(GameTime gameTime)
        {
            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState < 0)
            {
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                }
            }
        }

        public override void DrawInner(GameTime gameTime)
        {
            DrawScore();
            DrawTimer();

            base.DrawInner(gameTime);
        }

        private void DrawScore()
        {
            Vector2 screenCentre = new Vector2(1920 / 2, 1080 / 2);
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ChallengeManager.Instance.ResultsString(), screenCentre, -90, ChallengeManager.Instance.ScoreColour());
        }

        private void DrawTimer()
        {
            Vector2 screenCentre = new Vector2(90, 930);
            //Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontTimer, _advanceModeTime.ToString(), screenCentre, Color.Red); // :H: Cannot use draw string directly as the game is in portrtait mode
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ((int)_timeInState).ToString(), screenCentre, -90, Color.White);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            DrawScore();

            if (_defaultAnimation != null)
            {
                _defaultAnimation.DrawSpriteSheetAnim();
            }

            base.DrawSecondScreenInner(gameTime);
        }
    }
}