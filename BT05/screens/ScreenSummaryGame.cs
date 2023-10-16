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
    public class ScreenSummaryGame : GameScreenExtended
    {
        public ScreenSummaryGame(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_TIMER;
            _advanceModeTime = 20.0f;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "circlefill", animScreenRect, -90, 10, AnimationType.ONCE);
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

        public override void DrawInner(GameTime gameTime)
        {
            DrawScore();

            base.DrawInner(gameTime);
        }

        private void DrawScore()
        {
            Vector2 screenCentre = new Vector2(1920 / 2, 1080 / 2);
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ChallengeManager.Instance.ResultsString(), screenCentre, -90, ChallengeManager.Instance.ScoreColour());
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