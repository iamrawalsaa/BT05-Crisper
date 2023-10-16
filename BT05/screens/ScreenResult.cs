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
        private float _timeInState;
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

            if (GameManager.Instance.Turns < 4)
            {
                PrimaryText = "{{WHITE}}Challenge " + GameManager.Instance.Turns + " of 3";
                SecondaryText = "{{GREEN}}Your next challenge will start shortly";
            }
            else
            {
                PrimaryText = "";
                SecondaryText = "{{RED}}That was your final challenge!";
            }

            if ( GameManager.Instance.LastRoundWinner)
            {
                VideoManager.Instance.Play(VideoKey.success);
            }
            else
            {
                VideoManager.Instance.Play(VideoKey.failure);
            }

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
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                    if (GameManager.Instance.Turns > 3)
                    {
                        GameManager.Instance.NextGameState();
                    }
                    else
                    {
                        GameManager.Instance.GotoPhase(GamePhase.LEVELSELECT);
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
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