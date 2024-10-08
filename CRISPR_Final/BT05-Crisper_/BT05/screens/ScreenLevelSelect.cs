﻿using BT05;
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
    public class ScreenLevelSelect : GameScreenExtended
    {
        float _changeTime = 1.0f;
        float _totalTime = 5.0f;
        //private bool _leavingToNextState = false;

        Level _targetLevel = Level.None;
        bool _levelChosen = false;
        bool _firstShown = false;
        bool _showDesc = false;
        private float _timeInState; // :H: used to change the timer
        private float _randomizingTime;
        private bool _leavingToNextState = false;

        public enum LevelSelectionPhases
        {
            RandomlyShowAll,
            RandomUntilCorrect, // keep showing random items until we hit our target
            EndResult,
            TransitioningToNextPhase
        }

        LevelSelectionPhases _levelPhase = LevelSelectionPhases.RandomlyShowAll;

        public ScreenLevelSelect(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_NONE;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "countdown", animScreenRect, -90, 5, AnimationType.ONCE);
            _defaultAnimation.Pause();
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            ChallengeManager.Instance.NextChallenge();

            _defaultAnimation.Hide();
            _defaultAnimation.Reset();
            //_leavingToNextState = false;
            _changeTime = 3.0f;
            _totalTime = 8.0f;
            _timeInState = 20f;
            _randomizingTime = 10f;
            _leavingToNextState = false;
            _levelPhase = LevelSelectionPhases.RandomlyShowAll;
            _levelChosen = false;
            _firstShown = false;

            PrimaryText = "";// "{{HOT_PINK}}Randomly choosing your challenge!";
            SecondaryText = "";// "{{WHITE}}Skip to level with keys:\n1. SickleCell\n2. CowMethane\n3. Wheat\n4. Mosquito\n5. HeartDisease";

            //_targetLevel = LevelDatabase.Instance.GetRandomLevelOfDifficulty(ChallengeManager.Instance.CurrentLevelDifficulty()); // :H:

            base.ScreenArriving();
        }

        /// <summary>
        /// only when the timer has elapsed and the random one been the target one - can we freeze
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //_totalTime = 20;
            UpdateDrawables(gameTime);
            GameTimeout(gameTime);
            _changeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _totalTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            _randomizingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_levelPhase)
            {
                case LevelSelectionPhases.RandomlyShowAll:
                    if (_changeTime < 0.0f)
                    {
                        _changeTime = 1.0f;
                        RandomlyChooseNewLevel();
                        SoundEffectManager.Instance.PlaySound(SoundEffectEnum.LEVEL_SELECT_RANDOM);
                        _firstShown = true;
                    }

                    if (_totalTime < 0.0f)
                    {
                        _levelPhase = LevelSelectionPhases.RandomUntilCorrect;
                    }
                    break;
                case LevelSelectionPhases.RandomUntilCorrect:

                    //if (GameManager.Instance.CurrentLevel == _targetLevel) // :H: updated mechanism to chose randomly after certian amount of time is passed
                    if (_randomizingTime  < 0.0f)
                    {
                        _levelPhase = LevelSelectionPhases.EndResult;
                        _defaultAnimation.Reset();
                        _defaultAnimation.Play();
                        _defaultAnimation.Show();
                        LevelHasBeenChosen();
                        SoundEffectManager.Instance.PlaySound(SoundEffectEnum.LEVEL_SELECT_CHOSEN);

                        _totalTime = 5.0f;
                        _levelChosen = true;
                    }
                    else
                    {
                        if (_changeTime < 0.0f)
                        {
                            _changeTime = 1.0f;
                            RandomlyChooseNewLevel();
                            SoundEffectManager.Instance.PlaySound(SoundEffectEnum.LEVEL_SELECT_RANDOM);
                        }
                    }

                    break;
                case LevelSelectionPhases.EndResult:
                    if (_totalTime < -5.0f)
                    {
                        GameManager.Instance.NextGameState(_nextPhase);
                        _levelPhase = LevelSelectionPhases.TransitioningToNextPhase;
                    }
                    break;
                case LevelSelectionPhases.TransitioningToNextPhase:
                    break;
                default:
                    break;
            }

            //if (_totalTime>0)
            //{
            //    if (_changeTime < 0.0f)
            //    {
            //        _changeTime = 0.5f;
            //        RandomlyChooseNewLevel();
            //    }
            //}
            //else
            //{
            //    if (_totalTime < 0.0f)
            //    {
            //        LevelHasBeenChosen();
            //    }

            //    if (_totalTime < -5.0f)
            //    {
            //        if (!_leavingToNextState)
            //        {
            //            GameManager.Instance.NextGameState();
            //        }
            //        _leavingToNextState = true;
            //    }
            //}
            base.Update(gameTime);
        }

        private void DrawTimer()
        {
            Vector2 screenCentre = new Vector2(900, 500);
            //Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontTimer, _advanceModeTime.ToString(), screenCentre, Color.Red); // :H: Cannot use draw string directly as the game is in portrtait mode
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ((int)_timeInState).ToString(), screenCentre, -90, Color.White);
        }
        private void GameTimeout(GameTime gameTime)
        {
            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState < 0)
            {
                _timeInState = 0;
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                }
            }
        }

        void LevelHasBeenChosen()
        {
            _showDesc = true;
            //DrawDiseaseDesc();
            //PrimaryText = "{{RED}}" + GameManager.Instance.CurrentLevel;
            //SecondaryText = "{{WHITE}}Knock out this gene to:\n" + LevelDatabase.Instance.GetDescription(GameManager.Instance.CurrentLevel);
        }

        private void RandomlyChooseNewLevel()
        {
            GameManager.Instance.GenerateNewLevelChoice();
            //PrimaryText = "{{WHITE}}" + GameManager.Instance.CurrentLevel;
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();
            DrawTimer();

            //if (_levelPhase == LevelSelectionPhases.EndResult)
            //{
            //    if (_defaultAnimation != null)
            //    {
            //        _defaultAnimation.DrawSpriteSheetAnim();
            //    }
            //}
            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            DrawDiseaseTexture();
            DrawDiseaseName();

            if (_showDesc)
            { 
                DrawDiseaseDesc();
                //_showDesc = false;
            }

            base.DrawInner(gameTime);
        }

        private void DrawDiseaseTexture()
        {
            if (_firstShown)
            {
                var level = GameManager.Instance.CurrentLevel;
                var tex = SharedAssetManager.Instance.GetLevelTexture(level, _levelChosen);
                var origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
                Rectangle rect = new Rectangle(900, 540, 800, 800);
                float rot = MathHelper.ToRadians(-90);

                if (tex != null)
                {
                    Game._spriteBatch.Draw(tex, rect, null, Microsoft.Xna.Framework.Color.White, rot, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                }
            }
        }

        private void DrawDiseaseName()
        {
            if (_firstShown)
            {
                var level = GameManager.Instance.CurrentLevel;
                var tex = SharedAssetManager.Instance.GetLevelName(level, _levelChosen);
                var origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
                Rectangle rect = new Rectangle(350, 540, 339, 76);
                float rot = MathHelper.ToRadians(-90);

                if (tex != null)
                {
                    Game._spriteBatch.Draw(tex, rect, null, Microsoft.Xna.Framework.Color.White, rot, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                }
            }
        }

        private void DrawDiseaseDesc()
        {
            if (_firstShown)
            {
                var level = GameManager.Instance.CurrentLevel;
                var tex = SharedAssetManager.Instance.GetLevelDesc(level, _levelChosen);
                var origin = new Vector2(tex.Width / 2f, tex.Height / 2f);
                Rectangle rect = new Rectangle(1450, 540, 734, 203);
                float rot = MathHelper.ToRadians(-90);

                if (tex != null)
                {
                    Game._spriteBatch.Draw(tex, rect, null, Microsoft.Xna.Framework.Color.White, rot, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// From a key press
        /// </summary>
        /// <param name="sickleCell"></param>
        public void ChooseLevel_DebugKey(Level newLevel)
        {
            _targetLevel = newLevel;
            //GameManager.Instance.Level = newLevel;
            ChallengeManager.Instance.CurrentLevel = newLevel;
            LevelHasBeenChosen();

            _levelPhase = LevelSelectionPhases.EndResult;
            _defaultAnimation.Reset();
            _defaultAnimation.Play();
            _defaultAnimation.Show();
            
            SoundEffectManager.Instance.PlaySound(SoundEffectEnum.LEVEL_SELECT_CHOSEN);

            _totalTime = 5.0f;
            _levelChosen = true;
        }
    }
}