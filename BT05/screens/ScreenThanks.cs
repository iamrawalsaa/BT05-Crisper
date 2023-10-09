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
        private float _timeInState;
        private bool _leavingToNextState;

        public ScreenThanks(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

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

            _timeInState = 3.0f;
            _leavingToNextState = false;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);

            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState < 0)
            {
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                    GameManager.Instance.GotoPhase(GamePhase.ATTRACT);
                }
            }
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            //CheckTextureNeedsRecreating();

            //Game.GraphicsDevice.Clear(Color.GhostWhite);
            //var transformMatrix = Game.Camera.GetViewMatrix();
            //Game._spriteBatch.Begin(transformMatrix: transformMatrix);
            //DrawPre(gameTime);
            //DrawTextTexture();
            //DrawPost(gameTime);

            //Game._spriteBatch.End();
        }
    }
}