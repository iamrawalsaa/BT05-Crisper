
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using screens;
using shared;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMonoGame
{
    public sealed class MyScreenManager
    {
        private static readonly MyScreenManager _instance = new MyScreenManager();

        public static MyScreenManager Instance
        {
            get { return _instance; }
        }

        Dictionary<GamePhase, GameScreenExtended> _gameScreens = new Dictionary<GamePhase, GameScreenExtended>();

        MyGameBase _myClimateChangeGame;
        ScreenManager _screenManager;
        private GameScreenExtended _currentScreen;
        private GamePhase _currentPhase = GamePhase.NONE;

        public GamePhase CurrentPhase { get { return _currentPhase; } }


        public GameScreenExtended CurrentScreen
        {
            get { return this._currentScreen; }
        }

        public void Initialise(MyGameBase game, ScreenManager screenManager)
        {
            _myClimateChangeGame = game;
            _screenManager = screenManager;

            CreateAllScreens(game);
        }

        void CreateAllScreens(MyGameBase game)
        {
            _gameScreens.Add(GamePhase.ATTRACT, new ScreenAttract(game, GamePhase.ATTRACT));
            _gameScreens.Add(GamePhase.LANGUAGE, new ScreenLanguage(game, GamePhase.LANGUAGE));
            _gameScreens.Add(GamePhase.LEVELSELECT, new ScreenLevelSelect(game, GamePhase.LEVELSELECT));
            _gameScreens.Add(GamePhase.ENDGAME, new ScreenEndGame(game, GamePhase.ENDGAME));
            _gameScreens.Add(GamePhase.INSTRUCTIONS, new ScreenInstructions(game, GamePhase.INSTRUCTIONS));
            _gameScreens.Add(GamePhase.HOWTOPLAY, new ScreenHowToPlay(game, GamePhase.HOWTOPLAY));
            _gameScreens.Add(GamePhase.THANKS, new ScreenThanks(game, GamePhase.THANKS));
            _gameScreens.Add(GamePhase.GAME, new ScreenGame(game, GamePhase.GAME));
            _gameScreens.Add(GamePhase.PROTOTYPEMESSAGES, new ScreenPrototypeMessages(game, GamePhase.PROTOTYPEMESSAGES));
            _gameScreens.Add(GamePhase.RESULT, new ScreenResult(game, GamePhase.RESULT));
            _gameScreens.Add(GamePhase.TEST3D, new ScreenTest3D(game, GamePhase.TEST3D));
        }

        public void FadeToScreen(GamePhase gamephase)
        {
            var screen = _gameScreens[gamephase];

            if (screen != _currentScreen)
            {
                if (_currentScreen != null)
                {
                    _currentScreen.ScreenLeaving();
                }
                _currentScreen = screen;
                _currentPhase = gamephase;
                _currentScreen.ScreenArriving();

                _screenManager.LoadScreen(screen, new FadeTransition(_myClimateChangeGame.GraphicsDevice, Microsoft.Xna.Framework.Color.Black));
                //_screenManager.LoadScreen(screen, new FadeTransition(_myClimateChangeGame.GraphicsDevice, SharedAssetManager.Instance.GetColor(GameColours.THEME_COLOUR)));
            }
            //var transition = new SlideTransition(new Vector2(GraphicsDevice.Viewport.Width, 0), Vector2.Zero);
        }

        public void HideAllScreens()
        {
            // this doesn't work
           //_screenManager.LoadScreen(null, null);
        }

        public GameScreenExtended GetScreen(GamePhase gamePhase)
        {
            return _gameScreens[gamePhase];
        }

        public void MouseLeftClicked(int x, int y)
        {
            if (_currentScreen != null)
            {
                if (x >= 0 && x <= 1920 && y >= 0 && y <= 1080)
                {
                    _currentScreen.MouseLeftClicked(x, y);
                }
            }
        }

        public void MouseRightClicked(int x, int y)
        {
            if (_currentScreen != null)
            {
                if (x >= 0 && x <= 1920 && y >= 0 && y <= 1080)
                {
                    _currentScreen.MouseRightClicked(x, y);
                }
            }
        }

        public void MouseMiddleClicked(int x, int y)
        {
            if (_currentScreen != null)
            {
                if (x >= 0 && x <= 1920 && y >= 0 && y <= 1080)
                {
                    _currentScreen.MouseMiddleClicked(x, y);
                }
            }
        }


        public void MouseDragBegins(int x, int y)
        {
            if (_currentScreen != null)
            {
                _currentScreen.MouseDragBegins(x, y);
            }
        }

        public void MouseDragChanged(int xChange, int yChange)
        {
            if (_currentScreen != null)
            {
                _currentScreen.MouseDragChanged(xChange, yChange);
            }
        }

        public void MouseDragComplete()
        {
            if (_currentScreen != null)
            {
                _currentScreen.MouseDragComplete();
            }
        }

        
    }
}