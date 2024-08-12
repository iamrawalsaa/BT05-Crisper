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
    public class ScreenResult : GameScreenExtended
    {
        private float _timeInState; // :H: TODO can be used to change the timer
        private bool _leavingToNextState = false;

        public ScreenResult(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            //_advanceModeSet = ScreenAdvanceMode.ADVANCE_TIMER;
            //_advanceModeTime = 10.0f;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "circlefill", animScreenRect, -90, 10, AnimationType.ONCE);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            base.ScreenArriving();

            //if (GameManager.Instance.Turns < 4)
            //if (!ChallengeManager.Instance.IsFinalChallenge())
            //{
            //    PrimaryText = "{{WHITE}}Challenge " + ChallengeManager.Instance.Turns + " of 3";
            //    SecondaryText = "{{GREEN}}Your next challenge will start shortly";
            //}
            //else
            //{
            //    PrimaryText = "";
            //    SecondaryText = "{{RED}}That was your final challenge!";
            //}

            //if ( GameManager.Instance.LastRoundWinner)
            //if (ChallengeManager.Instance.CurrentLevelSuccess)
            //{
                //VideoManager.Instance.Play(VideoKey.success);
            //}
            //else
            //{
                //VideoManager.Instance.Play(VideoKey.failure);
            //}

            ShowPrimaryText = true;
            ShowSecondaryText = true;

            _timeInState = 7.5f;
            _leavingToNextState = false;
    }

        public override void ScreenLeaving()
        {
            VideoManager.Instance.Stop();
            base.ScreenLeaving();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);

            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState<0)
            {
                _timeInState = 0;
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                    GameManager.Instance.NextGameState(_nextPhase);

                    //if (GameManager.Instance.Turns > 3)
                    //if (ChallengeManager.Instance.IsFinalChallenge())
                    //{
                    //    GameManager.Instance.NextGameState(_nextPhase);
                    //}
                    //else
                    //{
                    //    GameManager.Instance.GotoPhase(GamePhase.LEVELSELECT);
                    //}
                }
            }

            base.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            //DrawVideoFrame();
            DrawSuccessFailure();
            DrawTimer();
            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            //DrawVideoFrame();
            
            DrawSuccessFailure();
            DrawTimer();
        }

        private void DrawSuccessFailure()
        {
            if (ChallengeManager.Instance.CurrentLevelSuccess)
            {
                //VideoManager.Instance.Play(VideoKey.success);
                if (GameManager.Instance.Language == Language.english)
                {
                    Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlaySuccess"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                }
                else
                {
                    Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlaySuccessHindi"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                }
                LogManager.Instance.SuccessFailureText = "Success";
            }
            else
            {
                //VideoManager.Instance.Play(VideoKey.failure);
                if (GameManager.Instance.Language == Language.english)
                {
                    Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlayFailure"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                }
                else
                {
                    Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlayFailureHindi"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
                }
                LogManager.Instance.SuccessFailureText = "Failure";
            }
        }

        private void DrawTimer()
        {
            Vector2 screenCentre = new Vector2(90, 930);
            //Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontTimer, _advanceModeTime.ToString(), screenCentre, Color.Red); // :H: Cannot use draw string directly as the game is in portrtait mode
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ((int)_timeInState).ToString(), screenCentre, -90, Color.White);
        }

        private void DrawVideoFrame()
        {
            var texture = VideoManager.Instance.GetVideoTexture();

            if (texture != null)
            {
                Rectangle screenRect = new Rectangle(0, 1080, 1080, 1920);
                Vector2 centre = new Vector2(0, 0);
                float rotation = (float)-Math.PI / 2.0f;

                Game._spriteBatch.Draw(texture, screenRect, null, Color.White, rotation, centre, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            }
        }
    }
}