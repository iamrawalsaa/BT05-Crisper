using BT05;
using Microsoft.Xna.Framework;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class ScreenLevelSelect : GameScreenExtended
    {
        float _changeTime = 1.0f;
        float _totalTime = 5.0f;
        private bool _leavingToNextState = false;

        public ScreenLevelSelect(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            _leavingToNextState = false;
            _changeTime = 3.0f;
            _totalTime = 10.0f;

            PrimaryText = "{{HOT_PINK}}Randomly choosing your level!";
            SecondaryText = "{{WHITE}}Skip to level with keys:\n1. SickleCell\n2. CowMethane\n3. Wheat\n4. Mosquito\n5. HeartDisease";

            base.ScreenArriving();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateDrawables(gameTime);
            _changeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _totalTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_totalTime>0)
            {
                if (_changeTime < 0.0f)
                {
                    _changeTime = 0.5f;
                    RandomlyChooseNewLevel();
                }
            }
            else
            {
                if (_totalTime < 0.0f)
                {
                    LevelHasBeenChosen();
                }

                if (_totalTime < -5.0f)
                {
                    if (!_leavingToNextState)
                    {
                        GameManager.Instance.NextGameState();
                    }
                    _leavingToNextState = true;
                }
            }
        }

        void LevelHasBeenChosen()
        {
            PrimaryText = "{{RED}}" + GameManager.Instance.Level;
            SecondaryText = "{{WHITE}}Knock out this gene to:\n" + LevelDatabase.Instance.GetDescription(GameManager.Instance.Level);
        }

        private void RandomlyChooseNewLevel()
        {
            GameManager.Instance.GenerateNewLevelChoice();
            PrimaryText = "{{WHITE}}"+GameManager.Instance.Level;
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {

        }

        /// <summary>
        /// From a key press
        /// </summary>
        /// <param name="sickleCell"></param>
        public void ChooseLevel(Level newLevel)
        {
            GameManager.Instance.Level = newLevel;
            _totalTime = 0;
            LevelHasBeenChosen();
        }


    }
}