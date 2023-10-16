using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using MonoGame;
using MonoGame.SplineFlower.Spline;
using MonoGame.SplineFlower.Content;
using MonoGame.SplineFlower;
using Newtonsoft.Json;
//using System.Windows.Forms;
using Newtonsoft.Json.Converters;
using MonoGame.SplineFlower.Content.Pipeline;
using MonoGame.SplineFlower.Spline.Types;
using SharpDX.XInput;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using SharpDX.DXGI;
using System.ComponentModel.DataAnnotations;
using shared;
using SharedMonoGame;
using screens;

namespace BT05
{
    public class GameBT05 : SharedMonoGame.MyGameBase
    {
        private KeyboardState _previousState;
        private KeyboardState _currentState;

        public GameBT05()
        {
            //_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            LevelDatabase.Instance.Initialise();

            //MyScreenManager.Instance.Initialise();
            //GameManager.Instance.Initialise();
            TextRendering.Instance.Initialise(this);
            ArduinoManager.Instance.OpenFirstSerialPort();

            base.Initialize();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            ArduinoManager.Instance.CloseSerialPort();
            base.OnExiting(sender, args);
        }

        protected override void LoadContent()
        {
            SharedAssetManager.Instance.LoadContent(this);
            VideoManager.Instance.LoadContent( this );

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GameManager.Instance.GamePhase == GamePhase.NONE)
            {
                GameManager.Instance.NextGameState();
            }

            AnimationManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        bool _mouseMode = false;

        private void CheckScissorKeys()
        {
            if (_mouseMode)
            {
                MouseAndKeyboard_Scissors();
            }
            else
            {
                Arduino_Scissors();
            }
        }

        private void Arduino_Scissors()
        {
            ScreenGame screenGame = MyScreenManager.Instance.CurrentScreen as ScreenGame;

            if (screenGame != null)
            {
                if (ArduinoManager.Instance.IsScissorOpen)
                {
                    screenGame.ScissorsOpen();
                }

                if (ArduinoManager.Instance.IsScissorClosed)
                {
                    screenGame.ScissorsClosed();
                }
            }

            ScreenLanguage screenLanguage = MyScreenManager.Instance.CurrentScreen as ScreenLanguage;
            if (screenLanguage != null)
            {
                if (ArduinoManager.Instance.IsScissorOpen)
                {
                    screenLanguage.ScissorsOpen();
                }

                if (ArduinoManager.Instance.IsScissorClosed)
                {
                    screenLanguage.ScissorsClosed();
                }
            }
        }

        private static void MouseAndKeyboard_Scissors()
        {
            Keys scissorsLeft = Keys.Z;
            Keys scissorsRight = Keys.X;

            bool leftDown = Keyboard.GetState().IsKeyDown(scissorsLeft);
            bool rightDown = Keyboard.GetState().IsKeyDown(scissorsRight) || Mouse.GetState().RightButton == ButtonState.Pressed;

            ScreenGame screenGame = MyScreenManager.Instance.CurrentScreen as ScreenGame;

            if (screenGame != null)
            {
                if (leftDown && rightDown)
                {
                    screenGame.ScissorsClosed();
                }
                else
                {
                    if (!leftDown && !rightDown)
                    {
                        screenGame.ScissorsOpen();
                    }
                    else
                    {
                        screenGame.OneSideClosed();
                    }
                }
            }

            ScreenLanguage screenLanguage = MyScreenManager.Instance.CurrentScreen as ScreenLanguage;
            if (screenLanguage != null)
            {
                if (leftDown && rightDown)
                {
                    screenLanguage.ScissorsClosed();
                }
                else
                {
                    if (!leftDown && !rightDown)
                    {
                        screenLanguage.ScissorsOpen();
                    }
                    else
                    {
                        screenLanguage.OneSideClosed();
                    }
                }
            }
        }

        bool _dualScreens = false;

        public bool IsDualScreen
        {
            get { return _dualScreens; }
        }

        bool _showMainPlayerFirst = true;
        public bool ShowMainPlayerFirst
        {
            get { return _showMainPlayerFirst; }
        }

        bool _showMini = false;
        public bool ShowMini
        {
            get { return _showMini; }
        }

        private void CheckKeyPress()
        {
            _currentState = Keyboard.GetState();

            if (JustPressed(Keys.V))
            {
                if (!_dualScreens)
                {
                    _dualScreens = true;
                    // make large and frameless
                    Graphics.HardwareModeSwitch = false;
                    //Graphics.IsFullScreen = true;
                    Window.IsBorderless = true;
                    Window.Position = new Point(0, 0);
                    Graphics.PreferredBackBufferWidth = 3840;
                    Graphics.PreferredBackBufferHeight = 1920;

                    Graphics.ApplyChanges();
                }
            }

            if (JustPressed(Keys.B))
            {
                _showMainPlayerFirst = !_showMainPlayerFirst;
            }

            if (JustPressed(Keys.M))
            {
                _showMini = !_showMini;
            }

            if (JustPressed(Keys.N))
            {
                _mouseMode = !_mouseMode;
            }

            if (JustPressed(Keys.E))
            {
                GameManager.Instance.LanguageChangedByArduino(Language.english);
            }

            if (JustPressed(Keys.H))
            {
                GameManager.Instance.LanguageChangedByArduino(Language.hindi);
            }

            if (JustPressed(Keys.D))
            {
                DebugOutput.Instance.ShowDebug = !DebugOutput.Instance.ShowDebug;
            }

            if (JustPressed(Keys.F))
            {
                Graphics.ToggleFullScreen();

                if (Graphics.IsFullScreen)
                {
                    //Mouse.SetCursor(null);
                }
            }

            if (JustPressed(Keys.H))
            {
                //_helixSplineLeft.
                Setup.ShowPoints = !Setup.ShowPoints;
                Setup.ShowDirectionVectors = Setup.ShowPoints;
                Setup.ShowLines = Setup.ShowPoints;
                Setup.ShowBaseLine = Setup.ShowPoints;
                Setup.ShowTangents = Setup.ShowPoints;
                Setup.ShowCurves = Setup.ShowPoints;
                Setup.ShowSplineWalker = Setup.ShowPoints;
                Setup.ShowCenterSpline = Setup.ShowPoints;
            }

            if (JustPressed(Keys.G))
            {
                Graphics.PreferredBackBufferWidth = 1920;
                Graphics.PreferredBackBufferHeight = 1080;
                Graphics.ApplyChanges();
            }

            if (JustPressed(Keys.D0))
            {
                //_splineScale += 0.1f;
                //_helixSplineLeft.Scale(1.1f);
            }

            if (JustPressed(Keys.D9))
            {
                //_splineScale -= 0.1f;
                //_helixSplineLeft.Scale(1 / 1.1f);
            }

            if (JustPressed(Keys.T))
            {
                if (_isShiftDown)
                {
                    TakeScreenCap();
                }
                else
                {
                    var currentScreen = MyScreenManager.Instance.CurrentScreen;
                    if (currentScreen != null)
                    {
                        currentScreen.WriteNextFrameToDisk();
                    }
                }
            }

            ScreenPrototypeMessages screen = MyScreenManager.Instance.CurrentScreen as ScreenPrototypeMessages;
            if (screen != null)
            {
                if (JustPressed(Keys.OemTilde))
                {
                    GameManager.Instance.GotoPhase(GamePhase.TEST3D);
                }

                if (JustPressed(Keys.OemPipe))
                {
                    ChallengeManager.Instance.JumpToLevel(ChallengeDifficulty.easy, Level.SickleCell);
                    
                    GameManager.Instance.GotoPhase(GamePhase.GAME);
                }
            }

            if (JustPressed(Keys.Enter))
            {
                GameManager.Instance.NextGameState();
            }

            KeyPressesMainGame();
            KeyPressesLanguage();
            KeyPressesAttract();

            _previousState = _currentState;
        }

        private void KeyPressesAttract()
        {
            ScreenAttract screenAttract = MyScreenManager.Instance.CurrentScreen as ScreenAttract;

            if (screenAttract != null)
            {
                if (JustPressed(Keys.Y))
                //if (JustPressed(Keys.NumPad1))
                {
                    VideoManager.Instance.Play(VideoKey.success);
                }

                if (JustPressed(Keys.U))
                //if (JustPressed(Keys.NumPad2))
                {
                    VideoManager.Instance.Play(VideoKey.failure);
                }

                if (JustPressed(Keys.I))
                //if (JustPressed(Keys.NumPad3))
                {
                    VideoManager.Instance.Play(VideoKey.attract);
                }

                //if (JustPressed(Keys.NumPad0))
                if (JustPressed(Keys.O))
                {
                    VideoManager.Instance.Stop();
                }

                //if (JustPressed(Keys.NumPad9))

                if (JustPressed(Keys.P))
                {
                    VideoManager.Instance.ToggleRenderTextures();
                }

                if (JustPressed(Keys.NumPad7))
                {
                    //VideoManager.Instance.StartAllVideos();
                }
            }
        }

        private void KeyPressesLanguage()
        {
            ScreenLanguage screenLanguage = MyScreenManager.Instance.CurrentScreen as ScreenLanguage;

            if (screenLanguage != null)
            {
                if (JustPressed(Keys.S))
                {
                    screenLanguage.Setup(true);
                }
            }
        }

        private void KeyPressesMainGame()
        {
            ScreenGame screenGame = MyScreenManager.Instance.CurrentScreen as ScreenGame;

            if (screenGame != null)
            {
                if (JustPressed(Keys.P))
                {
                    screenGame.ScrollToCorrectPamMarker();
                }

                if (JustPressed(Keys.R))
                {
                    screenGame.ResetSpline();
                }

                if (JustPressed(Keys.OemPeriod))
                {
                    screenGame.RopeChanged(0.5f);
                }

                if (JustPressed(Keys.M))
                {
                    screenGame.RopeChanged(0);
                }

                if (JustPressed(Keys.OemComma))
                {
                    screenGame.RopeChanged(-0.5f);
                }
                if (JustPressed(Keys.J))
                {
                    screenGame.ToggleNucleotideDrawing();
                }

                if (JustPressed(Keys.S))
                {
                    screenGame.SpreadHorizontally();
                }

                if (JustPressed(Keys.L))
                {
                    screenGame.ToggleDrawLeft();
                }

                if (JustPressed(Keys.K)) {
                    screenGame.ToggleDrawRight();
                }

                if (JustPressed(Keys.Up))
                {
                    float ratio = 1.0f;
                    if (_isShiftDown) { ratio = 0.1f; }

                    screenGame.Up(ratio);
                }

                if (JustPressed(Keys.Down))
                {
                    float ratio = 1.0f;
                    if (_isShiftDown) { ratio = 0.1f; }
                    screenGame.Down(ratio);
                }

                if (JustPressed(Keys.Left))
                {
                    float ratio = 1.0f;
                    if (_isShiftDown) { ratio = 0.1f; }
                    screenGame.Left(ratio);
                }

                if (JustPressed(Keys.Right))
                {
                    float ratio = 1.0f;
                    if (_isShiftDown) { ratio = 0.1f; }
                    screenGame.Right(ratio);
                }

                if (JustPressed(Keys.Q))
                {
                    screenGame.SaveSplineJSON();
                }

                if (JustPressed(Keys.D0))
                {
                    Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * ScreenRatio;
                    screenGame.FindClosestSplinePoint(mouse);
                }

                if (JustPressed(Keys.D9))
                {
                    Vector2 mouse = new Vector2(Mouse.GetState().X, Mouse.GetState().Y) * ScreenRatio;
                    screenGame.FindClosestSplineTangent(mouse);
                }

                if (JustPressed(Keys.I))
                {
                    float dist = 10.0f;
                    if (_isShiftDown) { dist = 1f; }
                    screenGame.ShiftEntireX(dist);
                }

                if (JustPressed(Keys.U))
                {
                    float dist = 10.0f;
                    if (_isShiftDown) { dist = 1f; }
                    screenGame.ShiftEntireX(-dist);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            //_spriteBatch.Begin();
            //_spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void InnerUpdate(GameTime gameTime)
        {
            float xDiff = 0;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_mouseMode)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed || Graphics.IsFullScreen)
                {
                    xDiff = (float)Mouse.GetState().Position.X - _oldMouseState.Position.X;
                    ScreenGame screenGame = MyScreenManager.Instance.GetScreen(GamePhase.GAME) as ScreenGame;

                    if (screenGame != null)
                    {
                        screenGame.RopeChanged(xDiff);
                    }
                }
            }
            else
            {
                int xChange = ArduinoManager.Instance.GetLatestRotation;

                ScreenGame screenGame = MyScreenManager.Instance.GetScreen(GamePhase.GAME) as ScreenGame;

                if (screenGame != null)
                {
                    screenGame.RopeChanged(xChange);
                }

                ScreenLanguage screenLanguage = MyScreenManager.Instance.CurrentScreen as ScreenLanguage;
                if (screenLanguage != null)
                {
                    screenLanguage.EncoderChanged(xChange);
                }
            }

            //_myNucleotide.Update(gameTime);
            //GameTime gameTimeLocal = new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * xDiff);

            if (JustPressed(Keys.OemPeriod) && _isShiftDown)
            {
                MyScreenManager.Instance.CurrentScreen.NextOrientation();
            }

            if (JustPressed(Keys.OemComma) && _isShiftDown)
            {
                MyScreenManager.Instance.CurrentScreen.PreviousOrientation();
            }

            if (JustPressed(Keys.W))
            {
                MyScreenManager.Instance.CurrentScreen.WaveHappened();
            }

            CheckKeyPress();

            if (Graphics.IsFullScreen)
            {
                Mouse.SetPosition(Graphics.PreferredBackBufferWidth / 2, Graphics.PreferredBackBufferHeight / 2);
            }

            CheckLevelSelectKeys();

            CheckScissorKeys();

            _oldMouseState = Mouse.GetState();
        }

        private void CheckLevelSelectKeys()
        {
            ScreenLevelSelect screenLevelSelect = MyScreenManager.Instance.CurrentScreen as ScreenLevelSelect;
            if (screenLevelSelect != null)
            {
                // SickleCell
                // CowMethane
                // Wheat
                // Mosquito
                // HeartDisease

                if (JustPressed(Keys.D1))
                {
                    screenLevelSelect.ChooseLevel_DebugKey(Level.SickleCell);
                }

                if (JustPressed(Keys.D2))
                {
                    screenLevelSelect.ChooseLevel_DebugKey(Level.CowMethane);
                }

                if (JustPressed(Keys.D3))
                {
                    screenLevelSelect.ChooseLevel_DebugKey(Level.Wheat);
                }

                if (JustPressed(Keys.D4))
                {
                    screenLevelSelect.ChooseLevel_DebugKey(Level.Mosquito);
                }

                if (JustPressed(Keys.D5))
                {
                    screenLevelSelect.ChooseLevel_DebugKey(Level.HeartDisease);
                }


            }
        }
    }
}