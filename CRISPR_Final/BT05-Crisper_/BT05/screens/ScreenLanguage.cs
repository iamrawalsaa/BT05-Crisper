using BT05;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.SplineFlower.Content;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public enum Language
    {
        none,
        english,
        hindi
    }

    public class ScreenLanguage : GameScreenExtended
    {
        private float _timeInState; // :H: used to change the timer
        private bool _leavingToNextState = false;
        public ScreenLanguage(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_WAVE;

            //Rectangle animScreenRect = new Rectangle(1650, 540, 200, 200);
            //_defaultAnimation = new OnScreenAnimation(Game, "handwave", animScreenRect, -90, 15, AnimationType.LOOPING);

            Rectangle animScreenRect1 = new Rectangle(1650, 540, 200, 200);
            _defaultAnimation1 = new OnScreenAnimation(Game, "handwave", animScreenRect1, -90, 15, AnimationType.LOOPING);
        }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        TweenerSprite _arrow=null, _english=null, _hindi= null;
        float _arrowRotation = 0;
        Language _language = Language.none;

        public override void ScreenArriving()
        {
            LogManager.Instance.StartDateTime = DateTime.Now;
            MusicManager.Instance.PlaySong(MusicEnum.INTRO);
            _timeInState = 10f;
            _leavingToNextState = false;
            Setup();

            base.ScreenArriving();
        }

        ScissorsComponent _scissorsComponent=null;
        private bool _movementAllowed=true;

        void CreateScissors()
        {
            if (_scissorsComponent!=null)
            {
                RemoveDrawable( _scissorsComponent );
                _scissorsComponent = null;
            }

            _scissorsComponent = new ScissorsComponent(Game);
            _scissorsComponent.SetCompleteDelegates(ScissorCut_callback, ScissorAnimationComplete_callback);
            AddDrawable(_scissorsComponent);

            var open = SharedAssetManager.Instance.GetTextureFromName("ScissorsOpen");
            var closed = SharedAssetManager.Instance.GetTextureFromName("ScissorsClosed");
            var msg = SharedAssetManager.Instance.GetTextureFromName("ScissorsOneSideMsg");
            var notAvailable = SharedAssetManager.Instance.GetTextureFromName("ScissorsNotAvailable");
            _scissorsComponent.SetTextures(open, closed, msg, notAvailable);

            _scissorsComponent.SetPosition(new Vector2(1500, 350));
            _scissorsComponent.ScissorCountPosition = new Vector2(1500, 350);
            _scissorsComponent.ScissorsAvailable = true;
            _scissorsComponent.PlayScissorAnimationOnCompletion = false;

            // hide the scissors - doesn't work
            // _scissorsComponent.Visible = false; 

            LanguageStateChanged();
        }

        private void ScissorAnimationComplete_callback()
        {
        }

        private void ScissorCut_callback()
        {
            if(_language == Language.english)
            {
                GameManager.Instance.Language = Language.english;
                GameManager.Instance.NextGameState(_nextPhase);
            }

            if(_language== Language.hindi)
            {
                GameManager.Instance.Language = Language.hindi;
                GameManager.Instance.NextGameState(_nextPhase);
            }
        }

        public void Setup(bool force = false)
        {
            _arrowRotation = 0;

            if (_arrow== null || force)
            {
                // CreateScissors();

                CreateTweeners();
            }
        }

        /// <summary>
        /// This creates the buttons and arrow
        /// 
        /// I've moved this into the backgrounds so it is no longer needed.
        /// </summary>
        private void CreateTweeners()
        {
            if (_arrow != null) RemoveTweenerSprite(_arrow);
            if (_english != null) RemoveTweenerSprite(_english);
            if (_hindi != null) RemoveTweenerSprite(_hindi);

            _arrow = new TweenerSprite(SharedAssetManager.Instance.GetTextureFromName("Arrow"));
            _english = new TweenerSprite(SharedAssetManager.Instance.GetTextureFromName("LanguageEnglish"));
            _hindi = new TweenerSprite(SharedAssetManager.Instance.GetTextureFromName("LanguageHindi"));

            _arrow.MyTransform.Position = new Vector2(1000, 540);
            _arrow.MyTransform.Rotation = (float)0;
            _arrow.MyTransform.Scale = new Vector2(0.7f, 0.7f);

            _english.MyTransform.Position = new Vector2(500, 250);
            _english.MyTransform.Rotation = (float)-Math.PI / 2;
            _english.MyTransform.Scale = new Vector2(0.5f, 0.5f);

            _hindi.MyTransform.Position = new Vector2(500, 820);
            _hindi.MyTransform.Rotation = (float)-Math.PI / 2;
            _hindi.MyTransform.Scale = new Vector2(0.5f, 0.5f);

            AddTweenerSprite(_english);
            AddTweenerSprite(_hindi);
            AddTweenerSprite(_arrow);

            _english.MySprite.Alpha = 0.0f;
            _hindi.MySprite.Alpha = 0.0f;
            _arrow.MySprite.Alpha = 0.0f;
        }

        public override void Update(GameTime gameTime)
        {
            GameTimeout(gameTime); // :H: Added timeout to attract screen

            _arrow.MyTransform.Rotation = _arrowRotation;

            Language oldLang = _language;

            _language = Language.none;

            _language = GameManager.Instance.Language;
            if (_language == Language.hindi)
            {
                _arrowRotation = (float)(-Math.PI / 4);
            }

            if (_language == Language.english)
            {
                _arrowRotation = (float)(Math.PI / 4);
            }

            //if (_arrowRotation < -Math.PI/8) { 
            //    _language = Language.hindi; 
            //}

            //if (_arrowRotation > Math.PI/8) { 
            //    _language = Language.english; 
            //}

            switch (_language)
            {
                case Language.none:
                    _english.MySprite.Color = Color.White;
                    _hindi.MySprite.Color = Color.White;
                    break;
                case Language.english:
                    _english.MySprite.Color = Color.Green;
                    _hindi.MySprite.Color = Color.White;
                    break;
                case Language.hindi:
                    _english.MySprite.Color = Color.White;
                    _hindi.MySprite.Color = Color.Green;
                    break;
                default:
                    break;
            }

            if (oldLang != _language)
            {
                LanguageStateChanged();
            }

            UpdateDrawables(gameTime);
            base.Update(gameTime);
        }
        private void DrawTimer()
        {
            Vector2 screenCentre = new Vector2(90, 930);
            //Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontTimer, _advanceModeTime.ToString(), screenCentre, Color.Red); // :H: Cannot use draw string directly as the game is in portrtait mode
            Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontTimer, ((int)_timeInState).ToString(), screenCentre, -90, Color.White);
            //Game._spriteBatch.DrawTextCentered(SharedAssetManager.Instance.FontSpeechBubble, "ã´çâØæ ·Ô¤ ¥æ·¤æÚU ·¤è ·¤ôçàæ·¤æ", screenCentre, -90, Color.White);

        }
        private void GameTimeout(GameTime gameTime)
        {
            _timeInState -= gameTime.GetElapsedSeconds();

            if (_timeInState < 0)
            {
                if (!_leavingToNextState)
                {
                    _leavingToNextState = true;
                    GameManager.Instance.GotoPhase(GamePhase.HOWTOPLAY);
                }
            }
        }
        private void LanguageStateChanged()
        {
            switch (_language)
            {
                case Language.none:
                    ShowSecondaryText = false;
                    ShowPrimaryText = true;
                    _scissorsComponent.Visible = false;
                    break;
                case Language.english:
                    ShowPrimaryText = false;
                    ShowSecondaryText = false;
                    SecondaryText = "{{WHITE}}Operate scissors to choose English";
                    if (_scissorsComponent != null)
                    {
                        _scissorsComponent.Visible = true;
                    }
                    break;
                case Language.hindi:
                    ShowPrimaryText = false;
                    ShowSecondaryText = false;
                    SecondaryText = "{{WHITE}}हिंदी चुनने के लिए कैंची चलाएं";

                    if (_scissorsComponent != null)
                    {
                        _scissorsComponent.Visible = true;
                    }
                    break;
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

            base.DrawInner(gameTime);
        }

        public override void MouseDragChanged(int xChange, int yChange)
        {
            UpdateArrowRotation(xChange);

            base.MouseDragChanged(xChange, yChange);
        }

        public void EncoderChanged( int xChange )
        {
            UpdateArrowRotation(xChange);
        }

        private void UpdateArrowRotation(int xChange)
        {
            _arrowRotation += xChange * 0.001f;
            _arrowRotation = Math.Clamp(_arrowRotation, (float)-Math.PI / 4, (float)Math.PI / 4);
        }

        public void ScissorsClosed()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.ScissorsClosed();
            }
        }

        public void ScissorsOpen()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.ScissorsOpen();
            }
        }

        public void OneSideClosed()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.OneSideClosed();
            }
        }

    }
}