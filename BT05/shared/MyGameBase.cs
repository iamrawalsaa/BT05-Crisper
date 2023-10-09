using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.IO;
using shared;
using System.Threading;

namespace SharedMonoGame
{
    /// <summary>
    /// This has all the generic stuff that the Viewer and Monogame Client Share
    /// </summary>
    public abstract class MyGameBase : Game
    {
        private OrthographicCamera _camera;
        private GraphicsDeviceManager _graphics;
        public Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;
        private readonly ScreenManager _screenManager;
        public KeyboardState _oldKeyboardState = Keyboard.GetState();
        public MouseState _oldMouseState = Mouse.GetState();

        public bool _isShiftDown = false;
        public bool _isCtrlDown = false;

        public Vector2 _mouseMove = new Vector2();
        private Vector2 _mouseStart = Vector2.Zero;
        private Vector2 _mouseEnd = Vector2.Zero;
        public Rectangle LastRect { get; set; } = Rectangle.Empty;
        protected string _connectedMessage = "Waiting to connect... ";
        private bool _mouseDrag;

        protected bool _fullscreen = false;
        SynchronizationContext _mainSyncContext;
        
        public GraphicsDeviceManager Graphics
        {
            get {  return _graphics; }
        }

        public OrthographicCamera Camera
        {
            get { return _camera; }
        }

        //public GraphicsDeviceManager Graphics
        //{
        //    get
        //    {
        //        return _graphics;
        //    }
        //}

        public MyGameBase()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _screenManager = new ScreenManager();
            Components.Add(_screenManager);
        }

        protected override void Initialize()
        {
            base.Initialize();
            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1920, 1080);
            _camera = new OrthographicCamera(viewportAdapter);

            if (_fullscreen)
            {
                _graphics.PreferredBackBufferWidth = 1920;
                _graphics.PreferredBackBufferHeight = 1080;
                //_graphics.ApplyChanges();
                //_graphics.IsFullScreen = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = 1280;
                _graphics.PreferredBackBufferHeight = 720;
            }

            
            this.GraphicsDevice.SetRenderTarget(new RenderTarget2D(this.GraphicsDevice, 1920, 1080));

            _graphics.ApplyChanges();

            MyScreenManager.Instance.Initialise(this, _screenManager);
            //SetConnectionInfo("Waiting to connect...");

            _mainSyncContext = SynchronizationContext.Current;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
        }

        public bool JustPressed(Keys key)
        {
            return (Keyboard.GetState().IsKeyDown(key) && _oldKeyboardState.IsKeyUp(key));
        }


        public int ScreenWidth
        {
            get
            {
                //return _graphics.PreferredBackBufferWidth;
                return 1920;
            }
        }

        public int ScreenHeight
        {
            get
            {
                //return _graphics.PreferredBackBufferHeight;
                return 1080;
            }
        }

        //public void ToggleFullScreen()
        //{
        //    _graphics.ToggleFullScreen();
        //}

        public void MaxResolution()
        {
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.ApplyChanges();
        }

        public void NormalResolution()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();
        }

        public void ToggleResolution()
        {
            if (_graphics.PreferredBackBufferWidth == 1280)
            {
                MaxResolution();
            }
            else
            {
                NormalResolution();
            }
        }

        public void TakeScreenCap()
        {
            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //force a frame to be drawn (otherwise back buffer is empty) 
            Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            var dateTimeNow = DateTime.Now;
            string timestamp = dateTimeNow.ToString("yyMMdd_HHmmss");

            //save to disk 
            Stream stream = File.OpenWrite("screenshot_" + timestamp + ".png");

            texture.SaveAsPng(stream, w, h);
            stream.Dispose();

            texture.Dispose();
        }

        private void CameraControls()
        {
            if (_isCtrlDown)
            {
                float dist = 10;
                float zoom = 0.1f;
                if (JustPressed(Keys.Left))
                {
                    _camera.Move(new Vector2(dist, 0));
                }

                if (JustPressed(Keys.Right))
                {
                    _camera.Move(new Vector2(-dist, 0));
                }

                if (JustPressed(Keys.Up))
                {
                    _camera.Move(new Vector2(0, dist));
                }
                if (JustPressed(Keys.Down))
                {
                    _camera.Move(new Vector2(0, -dist));
                }

                if (JustPressed(Keys.OemCloseBrackets))
                {
                    _camera.ZoomIn(zoom);
                }
                if (JustPressed(Keys.OemOpenBrackets))
                {
                    _camera.ZoomOut(zoom);
                }
            }
        }

        abstract protected void InnerUpdate(GameTime gameTime);

        protected override void Update(GameTime gameTime)
        {
            DebugOutput.Instance.ClearLiveString();

            float fps = 1.0f / gameTime.GetElapsedSeconds();

            DebugOutput.Instance.AppendToLiveString("FPS: " + fps.ToString("0.0") + " Current Mode: ");// + DebugOutput.Instance.CurrentScreen);


            KeyboardState keyboardState = Keyboard.GetState();
            _isCtrlDown = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
            _isShiftDown = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);

            MouseState mouseState = Mouse.GetState();

            var mouseDiff = mouseState.Position - _oldMouseState.Position;
            _mouseMove = new Vector2(mouseDiff.X, mouseDiff.Y);

            MouseDebug(mouseState);

            if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                // mouse just pressed
                MyScreenManager.Instance.MouseLeftClicked((int)(mouseState.X * ScreenRatio), (int)(mouseState.Y * ScreenRatio));
            }

            if (_oldMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Released)
            {
                // mouse just pressed
                MyScreenManager.Instance.MouseRightClicked((int)(mouseState.X * ScreenRatio), (int)(mouseState.Y * ScreenRatio));
            }

            if (_oldMouseState.MiddleButton == ButtonState.Pressed && mouseState.MiddleButton == ButtonState.Released)
            {
                // mouse just pressed
                MyScreenManager.Instance.MouseMiddleClicked((int)(mouseState.X * ScreenRatio), (int)(mouseState.Y * ScreenRatio));
            }


            if (_oldMouseState.LeftButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Pressed)
            {
                _mouseDrag = true;
                MyScreenManager.Instance.MouseDragBegins((int)(mouseState.X * ScreenRatio), (int)(mouseState.Y * ScreenRatio)); ;
            }

            if (_mouseDrag)
            {
                var x = (int)((mouseState.X * ScreenRatio) - (_oldMouseState.X * ScreenRatio));
                var y = (int)((mouseState.Y * ScreenRatio) - (_oldMouseState.Y * ScreenRatio));

                MyScreenManager.Instance.MouseDragChanged(x, y);

                if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    MyScreenManager.Instance.MouseDragComplete();
                    _mouseDrag = false;
                }
            }

            if (JustPressed(Keys.Escape))
                Exit();

            if (_isShiftDown && _isCtrlDown)
            {
                if (JustPressed(Keys.I))
                {
                    string info = "Preferred: " + Graphics.PreferredBackBufferWidth + " x " + Graphics.PreferredBackBufferHeight + "\n" +
                                    "Actual:" + GraphicsDevice.PresentationParameters.BackBufferWidth + " x " + GraphicsDevice.PresentationParameters.BackBufferHeight + "\n" +
                                    //"Primary Adapter" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width + " x " + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height +"\n" +
                                    "Yet another" + ActualScreenWidth + " x " + ActualScreenHeight;


                    DebugOutput.Instance.WriteInfo(info);
                }

                if (JustPressed(Keys.O))
                {
                    SetPreferredFromActual();
                }

                //if (JustPressed(Keys.P))
                //{
                //    ToggleFullScreenAndSetToNewSize();
                //}
            }

            InnerUpdate(gameTime);
            CameraControls();

            _oldKeyboardState = Keyboard.GetState();
            _oldMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        public void ToggleFullScreen()
        {
            if (Graphics.IsFullScreen)
            {
                _graphics.PreferredBackBufferWidth = 1280;
                _graphics.PreferredBackBufferHeight = 720;
                //_graphics.ApplyChanges();
//                Graphics.IsFullScreen = false;

                _graphics.ToggleFullScreen();
            }
            else
            {
                _graphics.PreferredBackBufferWidth = ActualScreenWidth;
                _graphics.PreferredBackBufferHeight = ActualScreenHeight;
                //_graphics.ApplyChanges();
                //Graphics.IsFullScreen = true;
                _graphics.ToggleFullScreen();

            }
        }

        public float ScreenRatio
        {
            get { return ScreenWidth / (float)_graphics.PreferredBackBufferWidth; }
        }

        public void SetPreferredFromActual()
        {
            _graphics.PreferredBackBufferWidth = ActualScreenWidth;
            _graphics.PreferredBackBufferHeight = ActualScreenHeight;
            _graphics.ApplyChanges();
        }

        public int ActualScreenWidth
        {
            get
            {
                GraphicsAdapter adapter = GraphicsDevice.Adapter;
                return adapter.CurrentDisplayMode.Width;
            }
        }

        public int ActualScreenHeight
        {
            get
            {
                GraphicsAdapter adapter = GraphicsDevice.Adapter;
                return adapter.CurrentDisplayMode.Height;

//                return GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
        }


        private void MouseDebug(MouseState mouseState)
        {
            //            DebugOutput.Instance.AppendToLiveString(" Mouse: " + mouseState.Position.X + "," + mouseState.Position.Y);
            float ratio = ScreenRatio; // this should be come from the camera

            if (mouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                // just pressed
                _mouseStart = new Vector2(mouseState.X, mouseState.Y) * ratio;
            }
            else
            {
                if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                {
                    // just released
                    //                    _mouseEnd = new Vector2(_oldMouseState.X, _oldMouseState.Y);
                    //Rectangle rect = CreateRectFromPoints(_mouseStart, _mouseEnd);
                }
                else
                {
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // dragged
                        _mouseEnd = new Vector2(mouseState.X, mouseState.Y) * ratio;
                        LastRect = CreateRectFromPoints(_mouseStart, _mouseEnd);

                        DebugOutput.Instance.AppendToLiveString("\nMouse: " + LastRect.Left + "," + LastRect.Top + "->" + LastRect.Right + "," + LastRect.Bottom + " (" + LastRect.Width + "," + LastRect.Height + ")");
                    }
                    else
                    {
                        if (LastRect == Rectangle.Empty)
                        {
                            DebugOutput.Instance.AppendToLiveString("\nMouse: " + mouseState.Position.X + "," + mouseState.Position.Y);
                        }
                        else
                        {
                            DebugOutput.Instance.AppendToLiveString("\nLast: " + LastRect.Left + "," + LastRect.Top + "->" + LastRect.Right + "," + LastRect.Bottom + " (" + LastRect.Width + "," + LastRect.Height + ")");
                        }
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                _mouseStart += _mouseMove;
                _mouseEnd += _mouseMove;

                LastRect = CreateRectFromPoints(_mouseStart, _mouseEnd);

                //   LastRect = Rectangle.Empty;
            }

            if (LastRect.Width == 0 && LastRect.Height == 0) LastRect = Rectangle.Empty;
        }

        private Rectangle CreateRectFromPoints(Vector2 mouseStart, Vector2 mouseEnd)
        {
            int xMin = (int)Math.Min(mouseStart.X, mouseEnd.X);
            int xMax = (int)Math.Max(mouseStart.X, mouseEnd.X);
            int yMin = (int)Math.Min(mouseStart.Y, mouseEnd.Y);
            int yMax = (int)Math.Max(mouseStart.Y, mouseEnd.Y);

            Rectangle rect = new Rectangle(xMin, yMin, xMax - xMin, yMax - yMin);

            return rect;
        }

        public void SetConnectionInfo(string info)
        {
            _connectedMessage = info;
        }

        public void SetWindowTitle(string windowTitle)
        {
            if (_mainSyncContext != null)
            {
                _mainSyncContext.Post(_ =>
                {
                    // Code to be executed on the main UI thread
                    Window.Title = windowTitle;

                }, null);
            }
        }
    }
}
