using BT05;
using Microsoft.Xna.Framework;
using MonoGame.SplineFlower.Content;
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
        public ScreenLanguage(MyGameBase game, shared.GamePhase phase) : base(game, phase) {
            _advanceModeSet = ScreenAdvanceMode.ADVANCE_WAVE;

            Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            _defaultAnimation = new OnScreenAnimation(Game, "handwave", animScreenRect, -90, 15, AnimationType.LOOPING);
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
            MusicManager.Instance.PlaySong(MusicEnum.INTRO);

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
                GameManager.Instance.NextGameState();
            }

            if(_language== Language.hindi)
            {
                GameManager.Instance.Language = Language.hindi;
                GameManager.Instance.NextGameState();
            }
        }

        public void Setup(bool force = false)
        {
            _arrowRotation = 0;

            if (_arrow== null || force)
            {
                CreateScissors();

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
            }
        }

        public override void Update(GameTime gameTime)
        {
            _arrow.MyTransform.Rotation = _arrowRotation;

            Language oldLang = _language;

            _language = Language.none;

            if (_arrowRotation < -Math.PI/8) { 
                _language = Language.hindi; 
            }

            if (_arrowRotation > Math.PI/8) { 
                _language = Language.english; 
            }


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
                    ShowSecondaryText = true;
                    SecondaryText = "{{WHITE}}Operate scissors to choose English";
                    _scissorsComponent.Visible = true;

                    break;
                case Language.hindi:
                    ShowPrimaryText = false;
                    ShowSecondaryText = true;
                    SecondaryText = "{{WHITE}}हिंदी चुनने के लिए कैंची चलाएं";
                    _scissorsComponent.Visible = true;
                    break;
            }
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            base.DrawSecondScreenInner(gameTime);
        }

        public override void DrawInner(GameTime gameTime)
        {
            //Rectangle animScreenRect = new Rectangle(1700, 540, 200, 200);
            //DrawSpriteSheetAnim(_animLooping, animScreenRect, -90);

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