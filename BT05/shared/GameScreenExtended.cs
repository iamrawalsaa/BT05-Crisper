using BT05;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Timers;
using shared;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;

namespace SharedMonoGame
{
    public enum ScreenAdvanceMode
    {
        ADVANCE_TIMER,
        ADVANCE_NONE,
        ADVANCE_WAVE
    }

    public abstract class GameScreenExtended : GameScreen
    {
        private ScreenAdvanceMode _advanceMode = ScreenAdvanceMode.ADVANCE_NONE;
        private float _advanceModeTimer = 0;

        protected float _advanceModeTime = 5.0f;
        protected ScreenAdvanceMode _advanceModeSet = ScreenAdvanceMode.ADVANCE_NONE;

        private bool _isWaveValid = false; // prevent wave for 2-3 seconds

        protected new MyGameBase Game => (MyGameBase)base.Game;
        GamePhase _phase = GamePhase.NONE;

        public GamePhase Phase
        {
            get { return _phase; }
        }

        List<TweenerSprite> _generalTweenerSprites = new List<TweenerSprite>();

        protected void AddTweenerSprite( TweenerSprite newTweenerSprite)
        {
            _generalTweenerSprites.Add(newTweenerSprite);
        }

        protected void RemoveTweenerSprite(TweenerSprite newTweenerSprite)
        {
            _generalTweenerSprites.Remove(newTweenerSprite);
        }

        protected void ClearAllTweenerSprites()
        {
            _generalTweenerSprites.Clear();
        }

        protected GameScreenExtended(Game game, GamePhase phase) : base(game)
        {
            _phase = phase;
            Initialise();
        }

        protected virtual void Initialise()
        {

        }

        const int MAX_SINGLE_LINE = 60;

        protected ScreenOrientation _primaryTextOrientation = ScreenOrientation.Landscape;
        protected ScreenOrientation _secondaryTextOrientation = ScreenOrientation.Landscape;

        int _primaryTextWidth = -1;
        int _secondaryTextWidth = -1;

        public int PrimaryTextWidth { get
            {
                return _primaryTextWidth;
            }
            set
            {
                _primaryTextWidth = value;
                _primaryTextureNeedsReCreating = true;
            } 
        }
        public int SecondaryTextWidth
        {
            get { return _secondaryTextWidth; }
            set { 
                _secondaryTextWidth = value;
                _secondaryTextureNeedsReCreating = true;
            }
        }

        public ScreenOrientation PrimaryTextOrientation
        {
            get { return _primaryTextOrientation; }
            set { _primaryTextOrientation = value; }
        }

        public ScreenOrientation SecondaryTextOrientation
        {
            get { return _secondaryTextOrientation; }
            set { _secondaryTextOrientation = value; }
        }

        public void NextOrientation()
        {
            ++_primaryTextOrientation;

            if (_primaryTextOrientation == ScreenOrientation.MAX)
            {
                _primaryTextOrientation = ScreenOrientation.Landscape;
            }

            ++_secondaryTextOrientation;

            if (_secondaryTextOrientation == ScreenOrientation.MAX)
            {
                _secondaryTextOrientation = ScreenOrientation.Landscape;
            }

            _primaryTextureNeedsReCreating = true;
            _secondaryTextureNeedsReCreating = true;
        }

        string _primaryText = "Default Text";
        string _secondaryText = "Default Text";
        public bool ShowPrimaryText { get; set; } = true;
        public bool ShowSecondaryText { get; set; } = true;

        public Vector2 PrimaryTextOffset { get; set; } = Vector2.Zero;
        public Vector2 SecondaryTextOffset { get; set; } = Vector2.Zero;



        protected List<ExtendedDrawableGameComponent> _drawables = new List<ExtendedDrawableGameComponent>();

        protected void AddDrawable(ExtendedDrawableGameComponent drawable)
        {
            _drawables.Add(drawable);
        }

        protected bool RemoveDrawable(ExtendedDrawableGameComponent drawable)
        {
            return _drawables.Remove(drawable);
        }

        public void RecreateTextures()
        {
            _primaryTextureNeedsReCreating = true;
            _secondaryTextureNeedsReCreating = true;
        }

        public string SecondaryText
        {
            get
            {
                return _secondaryText;
            }
            set
            {
                _secondaryText = value;
                _secondaryTextureNeedsReCreating = true;
            }
        }

        public string PrimaryText { 
            get
            {
                return _primaryText;
            }
            set
            {
                _primaryText = value;
                _primaryTextureNeedsReCreating = true;
            } 
        }

        public Color PrimaryTextFontColour
        {
            get {
                return _primaryTextFontColour;
            }

            set {
                _primaryTextFontColour = value;
                _primaryTextureNeedsReCreating = true;
            }
        }

        public Color SecondaryTextFontColour
        {
            get
            {
                return _secondaryTextFontColour;
            }

            set
            {
                _secondaryTextFontColour = value;
                _secondaryTextureNeedsReCreating = true;
            }
        }

        protected void CheckTextureNeedsRecreating()
        {
            if (_primaryTextureNeedsReCreating)
            {
                RenderPrimaryTextTexture();
            }

            if (_secondaryTextureNeedsReCreating)
            {
                RenderSecondaryTextTexture();
            }

            RecreateDrawableTextures();
        }

        private void RenderSecondaryTextTexture()
        {
            var maxLineLength = 60;
            if (_secondaryTextOrientation == ScreenOrientation.Portrait || _secondaryTextOrientation == ScreenOrientation.ReversePortrait)
            {
                maxLineLength = 40;
            }

            if (SecondaryTextWidth != -1) maxLineLength = SecondaryTextWidth;

            var paragraph = TextRendering.Instance.ConvertTextToParagraph(_secondaryText, TextAlignment.center, maxLineLength);
            _secondaryTextTexture = TextRendering.Instance.RenderParagraphToTexture(paragraph, SharedAssetManager.Instance.FontRegular);

            // landscape
            var x = 0;
            var y = 0;

            if (_secondaryTextTexture != null)
            {

                x = (Game.ScreenWidth) / 2;
                y = 4 * (Game.ScreenHeight) / 5;

                switch (_secondaryTextOrientation)
                {
                    case ScreenOrientation.Landscape:
                        break;
                    case ScreenOrientation.Portrait:
                        y = (Game.ScreenHeight) / 2;
                        x = 4 * (Game.ScreenWidth) / 5;
                        break;
                    case ScreenOrientation.ReverseLandscape:
                        y = Game.ScreenHeight - y;
                        break;
                    case ScreenOrientation.ReversePortrait:
                        y = (Game.ScreenHeight) / 2;
                        x = 4 * (Game.ScreenWidth) / 5;
                        x = Game.ScreenWidth - x;
                        break;
                    default:
                        break;
                }
            }


            _secondaryTextTexturePosition = new Vector2(x, y);
            _secondaryTextTexturePosition += SecondaryTextOffset;
            _secondaryTextureNeedsReCreating = false;
        }

        private void RenderPrimaryTextTexture()
        {
            //var lines = DrawHelper.Instance.SplitTextToMultiLinesSimple(_primaryText);
            var maxLineLength = 60;
            if (_primaryTextOrientation == ScreenOrientation.Portrait || _primaryTextOrientation == ScreenOrientation.ReversePortrait)
            {
                maxLineLength = 40;
            }

            if (PrimaryTextWidth != -1) maxLineLength = PrimaryTextWidth;

            var paragraph = TextRendering.Instance.ConvertTextToParagraph(_primaryText, TextAlignment.center,maxLineLength);
            _primaryTextTexture = TextRendering.Instance.RenderParagraphToTexture(paragraph, SharedAssetManager.Instance.FontRegular);

            //if (AnyLineIsTooLong(lines, MAX_SINGLE_LINE))
            //{
            //    _primaryTextTexture = DrawHelper.Instance.ProduceOutlineFontTexture(_primaryText, SharedAssetManager.Instance.FontRegular, MAX_SINGLE_LINE, _primaryTextFontColour);
            //}
            //else
            //{
            //    _primaryTextTexture = DrawHelper.Instance.DrawStringMulti(SharedAssetManager.Instance.FontRegular, lines, _primaryTextFontColour);
            //}

            var x = 0;
            var y = 0;

            if (_primaryTextTexture != null)
            {
                // landscape
                x = (Game.ScreenWidth) / 2;
                y = (Game.ScreenHeight) / 4;

                switch (_secondaryTextOrientation)
                {
                    case ScreenOrientation.Landscape:
                        break;
                    case ScreenOrientation.Portrait:
                        y = (Game.ScreenHeight) / 2;
                        x = (Game.ScreenWidth) / 5;
                        break;
                    case ScreenOrientation.ReverseLandscape:
                        y = Game.ScreenHeight - y;
                        break;
                    case ScreenOrientation.ReversePortrait:
                        y = (Game.ScreenHeight) / 2;
                        x = (Game.ScreenWidth) / 5;
                        x = Game.ScreenWidth - x;
                        break;
                    default:
                        break;
                }
            }

            _primaryTextTexturePosition = new Vector2(x, y);
            _primaryTextTexturePosition += PrimaryTextOffset;
            _primaryTextureNeedsReCreating = false;
        }

        private bool AnyLineIsTooLong(List<string> lines, int maxLength)
        {
            foreach (var line in lines)
            {
                if (line.Length > maxLength) return true;
            }
            return false;
        }

        protected void DrawTextTexture()
        {
            if ( ShowPrimaryText && _primaryTextTexture != null)
            {
                //Game._spriteBatch.Draw(_primaryTextTexture, _primaryTextTexturePostion + PrimaryTextOffset, Color.White);
                DrawCenteredTexture(_primaryTextTexture, _primaryTextTexturePosition, _primaryTextOrientation);
            }

            if ( ShowSecondaryText && _secondaryTextTexture != null)
            {
                DrawCenteredTexture(_secondaryTextTexture, _secondaryTextTexturePosition, _secondaryTextOrientation);

                //Game._spriteBatch.Draw(_secondaryTextTexture, _secondaryTextTexturePostion + SecondaryTextOffset, Color.White);
            }
        }

        void DrawCenteredTexture(Microsoft.Xna.Framework.Graphics.Texture2D tex, Vector2 position, ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.Landscape:
                    DrawCenteredTexture(tex, position, Color.White, 0);
                    break;
                case ScreenOrientation.Portrait:
                    DrawCenteredTexture(tex, position, Color.White, (float)-Math.PI/2);
                    break;
                case ScreenOrientation.ReverseLandscape:
                    DrawCenteredTexture(tex, position, Color.White, (float)Math.PI);
                    break;
                case ScreenOrientation.ReversePortrait:
                    DrawCenteredTexture(tex, position, Color.White, (float)Math.PI / 2);
                    break;
                default:
                    break;
            }
        }

        void DrawCenteredTexture(Microsoft.Xna.Framework.Graphics.Texture2D tex, Vector2 position, Color color, float rotation = 0 )
        {
            Vector2 centre = new Vector2(tex.Width, tex.Height) / 2;

            Game._spriteBatch.Draw(tex, position, null, color, rotation, centre, 1.0f, SpriteEffects.None, 0);
        }

        protected void DrawPre(GameTime gameTime)
        {
            //DrawAnimatedBackground(gameTime);
            DrawBackground();
        }

        Vector2 _animatedOffset = new Vector2();

        

        private void DrawBackground()
        {
            Game._spriteBatch.Draw(SharedAssetManager.Instance.GetBackground(_phase), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
        }

        /// <summary>
        /// Any kind of global overlay
        /// </summary>
        protected void DrawPost(GameTime gameTime)
        {
            DrawGenericTweenerSprites(gameTime);
            DrawDrawables(gameTime);

            if (DebugOutput.Instance.ShowDebug)
            {
                Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontConsole, "DebugInfo:\n" + DebugOutput.Instance.DebugString, new Vector2(0, 200), Color.White);

                Color color = Color.Red;
                color.A = 128;

                Game._spriteBatch.FillRectangle(Game.LastRect, color);                    
            }
            
            Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontConsole, DebugOutput.Instance.LiveString, new Vector2(0, 0), Color.White);
        }

        private void DrawGenericTweenerSprites(GameTime gameTime)
        {
            foreach (var tweenerSprite in _generalTweenerSprites)
            {
                Game._spriteBatch.Draw(tweenerSprite.MySprite, tweenerSprite.MyTransform);
            }
        }

        private void DrawDrawables(GameTime gameTime)
        {
            foreach(var draw in _drawables)
            {
                draw.Draw(gameTime);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (_advanceMode == ScreenAdvanceMode.ADVANCE_TIMER)
            {
                if (_advanceModeTimer >0)
                {
                    _advanceModeTimer-= gameTime.GetElapsedSeconds();

                    if ( _advanceModeTimer <= 0 )
                    {
                        GameManager.Instance.NextGameState();
                    }
                }
            }

            _ignoreWaveTimer -= gameTime.GetElapsedSeconds();

            if (_ignoreWaveTimer <= 0 )
            {
                _isWaveValid = true;
            }
        }

        protected void UpdateDrawables(GameTime gameTime)
        {
            foreach (var draw in _drawables)
            {
                draw.Update(gameTime);
            }

            foreach (var tweenerSprite in _generalTweenerSprites)
            {
                tweenerSprite.MyTweener.Update(gameTime.GetElapsedSeconds());
            }
        }

        private void RecreateDrawableTextures()
        {
            foreach (var draw in _drawables)
            {
                draw.RecreateTextures();
            }
        }

        public void SetTimer(int timer)
        {
            _currentTimer = timer;
        }

        protected int _currentTimer = 0;

        protected Color _primaryTextFontColour = Color.White;
        protected Microsoft.Xna.Framework.Graphics.Texture2D _primaryTextTexture = null;
        protected Vector2 _primaryTextTexturePosition = Vector2.Zero;
        protected bool _primaryTextureNeedsReCreating = false;

        protected Color _secondaryTextFontColour = Color.White;
        protected Microsoft.Xna.Framework.Graphics.Texture2D _secondaryTextTexture = null;
        protected Vector2 _secondaryTextTexturePosition = Vector2.Zero;
        protected bool _secondaryTextureNeedsReCreating = false;

        public void DrawStringCentered(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 centre, Color color)
        {
            Vector2 stringSize = font.MeasureString(text);
            spriteBatch.DrawString(font, text, centre - (stringSize / 2.0f), color);
        }

        /// <summary>
        /// This prevents slight fluctuations in movement when individual font flyths are different sizes
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="font"></param>
        /// <param name="text"></param>
        /// <param name="centre"></param>
        /// <param name="color"></param>
        public void DrawStringCentered_Average(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 centre, Color color)
        {
            string measureString = "";
            for(int i =0; i< text.Length; ++i)
            {
                measureString += "8";
            }

            Vector2 stringSize = font.MeasureString(measureString);
            spriteBatch.DrawString(font, text, centre - (stringSize / 2.0f), color);
        }

        /// <summary>
        /// Called when a screen is about to be transitioned off
        /// </summary>
        public virtual void ScreenLeaving()
        {
            _advanceMode = ScreenAdvanceMode.ADVANCE_NONE;
            _isWaveValid = false;
            _ignoreWaveTimer = 2.0f;
        }

        public void WaveHappened()
        {
            if (_isWaveValid && _advanceMode == ScreenAdvanceMode.ADVANCE_WAVE)
            {
                GameManager.Instance.NextGameState();
            }
        }

        float _ignoreWaveTimer = 2.0f;

        /// <summary>
        /// Called when a screen is about to be transitioned on
        /// </summary>
        public virtual void ScreenArriving()
        {
            _advanceMode = _advanceModeSet;
            _advanceModeTimer = _advanceModeTime;
            _ignoreWaveTimer = 2.0f;
            _isWaveValid = false;
        }

        public virtual void MouseLeftClicked(int x, int y)
        {

        }

        public virtual void MouseRightClicked(int x, int y)
        {
        }

        public virtual void MouseMiddleClicked(int x, int y)
        {
        }

        public virtual void MouseDragBegins(int x, int y)
        { }
        public virtual void MouseDragChanged(int xChange, int yChange) { }
        public virtual void MouseDragComplete() { }
        public virtual void ScreenExitCompleted()
        {
            //DebugOutput.Instance.WriteInfo("Screen Exit Complete: " + _phase);

            //if (_advisor != null)
            //{
            //    _advisor.ResetAdvisor();
            //}
        }

        RenderTarget2D _renderTarget = null, _renderTargetSecondScreen = null;

        public override void Draw(GameTime gameTime)
        {
            if (_renderTarget == null)
            {
                _renderTarget = new RenderTarget2D(Game.GraphicsDevice, 1920, 1080);
            }

            if (_renderTargetSecondScreen == null)
            {
                _renderTargetSecondScreen = new RenderTarget2D(Game.GraphicsDevice, 1920, 1080);
            }

            DrawToRenderTarget(gameTime);
            DrawToRenderTargetSecondScreen(gameTime);

            GraphicsDevice.Clear(Color.Black);

            Game._spriteBatch.Begin();

            // I've written better code for this many times
            // check the Speeduko
            
            int RENDER_WIDTH = Game.ActualScreenWidth;
            float ratio = 1920.0f / 1080.0f;
            int RENDER_HEIGHT = (int)(RENDER_WIDTH / ratio);
            int RENDER_Y_OFFSET = (Game.ActualScreenHeight - RENDER_HEIGHT) / 2;
            int RENDER_X_OFFSET = 0;

            if (!Game.Graphics.IsFullScreen)
            {
                RENDER_WIDTH = Game.Graphics.PreferredBackBufferWidth;
                RENDER_HEIGHT = (int)(RENDER_WIDTH / ratio);
                //RENDER_HEIGHT = Game.Graphics.PreferredBackBufferHeight;
                RENDER_Y_OFFSET = (Game.Graphics.PreferredBackBufferHeight - RENDER_HEIGHT) / 2;
                RENDER_X_OFFSET = 0;
            }

            // Addition of dual screen rendering
            GameBT05 gameBT05 = Game as GameBT05;
            if (gameBT05 != null)
            {
                if ( gameBT05.IsDualScreen )
                {
                    Game._spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, 1920, 1080), Color.White);
                    Game._spriteBatch.Draw(_renderTargetSecondScreen, new Rectangle(1920, 0, 1920, 1080), Color.White);
                }
                else
                {
                    // central rendering on 1 screen - full screen
                    Game._spriteBatch.Draw(_renderTarget, new Rectangle(RENDER_X_OFFSET, RENDER_Y_OFFSET, RENDER_WIDTH, RENDER_HEIGHT), Color.White);
                }
            }

            //Game._spriteBatch.Draw(_renderTarget, new Rectangle(0,200, 720, 400), Color.White);
            //Game._spriteBatch.Draw(SharedAssetManager.Instance.Missing, new Rectangle(0, 0,550,50), Color.Red);
            //Game._spriteBatch.Draw(SharedAssetManager.Instance.Missing, new Rectangle(0, 0, 50, 1280), Color.Green);
            //Game._spriteBatch.Draw(SharedAssetManager.Instance.Missing, new Rectangle(700, 0, 20, 1280), Color.Blue);




            Game._spriteBatch.End();
        }

        private void DrawToRenderTarget(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);

            CheckTextureNeedsRecreating();

            //Game.GraphicsDevice.Clear(SharedAssetManager.Instance.GetColor(GameColours.THEME_COLOUR));
            Game.GraphicsDevice.Clear(Color.White);

            var transformMatrix = Game.Camera.GetViewMatrix();
            Game._spriteBatch.Begin(transformMatrix: transformMatrix);
            DrawPre(gameTime);

            DrawInner(gameTime);


            DrawTextTexture();
            DrawPost(gameTime);

            Game._spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawToRenderTargetSecondScreen(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTargetSecondScreen);

            //CheckTextureNeedsRecreating();

            //Game.GraphicsDevice.Clear(SharedAssetManager.Instance.GetColor(GameColours.THEME_COLOUR));
            Game.GraphicsDevice.Clear(Color.White);

            var transformMatrix = Game.Camera.GetViewMatrix();
            Game._spriteBatch.Begin(transformMatrix: transformMatrix);
            //DrawPre(gameTime);

            DrawSecondScreenInner(gameTime);


            //DrawTextTexture();
            //DrawPost(gameTime);

            Game._spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }


        public virtual void DrawInner(GameTime gameTime)
        {

        }

        public virtual void DrawSecondScreenInner(GameTime gameTime)
        {

        }

        public void PreviousOrientation()
        {
            // TODO
        }

        protected void DisplaySecondScreenBackground()
        {
            Rectangle rect = new Rectangle(0, 0, 1920, 1080);

            var background = SharedAssetManager.Instance.GetBackgroundSecondScreen(Phase);
            if (background != null)
            {
                Game._spriteBatch.Draw(background, rect, Color.White);
            }
        }
    }
}