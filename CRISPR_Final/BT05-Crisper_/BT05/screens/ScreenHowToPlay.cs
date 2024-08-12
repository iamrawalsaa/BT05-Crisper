using BT05;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{


    public class ScreenHowToPlay : GameScreenExtended
    {
        private float _timeInState; // :H: used to change the timer
        private bool _leavingToNextState = false;


        public override void ScreenArriving()
        {
            _timeInState = 10f;
            _leavingToNextState = false;
            base.ScreenArriving();
        }

        public ScreenHowToPlay(MyGameBase game, shared.GamePhase phase) : base(game, phase) { 
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_WAVE;

            //Rectangle animScreenRect = new Rectangle(1650, 540, 200, 200);
            //_defaultAnimation = new OnScreenAnimation(Game, "handwave",animScreenRect,-90, 15, AnimationType.LOOPING);

            Rectangle animScreenRect1 = new Rectangle(1650, 540, 200, 200);
            _defaultAnimation1 = new OnScreenAnimation(Game, "handwave", animScreenRect1, -90, 15, AnimationType.LOOPING);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);
            GameTimeout(gameTime); // :H: Added timeout to attract screen
            base.Update(gameTime);
        }

        private void DrawTimer()
        {
            Vector2 screenCentre = new Vector2(90, 930);
            //Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontTimer, _advanceModeTime.ToString(), screenCentre, Color.Red); // :H: Cannot use draw string directly as the game is in portrtait mode
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ((int)_timeInState).ToString(), screenCentre, -90, Color.White);
        }
        private void GameTimeout(GameTime gameTime)
        {
            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState < 0)
            {
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                    GameManager.Instance.GotoPhase(GamePhase.LEVELSELECT);
                }
            }
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();
            DrawTimer();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {


            DrawTimer();

            //DrawSpriteSheetAnim(_animLooping, _animScreenRect, -90);
            base.DrawInner(gameTime);
        }
    }
}