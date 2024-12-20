﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Content;
using screens;
using shared;
using SharedMonoGame;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BT05
{
    public enum GameOverlays
    {
        none,
        actual,
        cut,
        damaged,
        find,
        knock,
        max
    }

    public sealed class SharedAssetManager : AssetManagerBase
    {
        Random _random = new Random();
        private static readonly SharedAssetManager _instance = new SharedAssetManager();

        public static SharedAssetManager Instance
        {
            get { return _instance; }
        }

        Dictionary<GamePhase, Texture2D> _backgroundsSecondScreen = new Dictionary<GamePhase, Texture2D>();

        Dictionary<GamePhase, Texture2D> _backgroundsSecondScreenHindi = new Dictionary<GamePhase, Texture2D>();
        Dictionary<GamePhase, Texture2D> _backgroundsHindi = new Dictionary<GamePhase, Texture2D>();

        Dictionary<GameOverlays, Texture2D> _gameOverlaysEnglish = new Dictionary<GameOverlays, Texture2D>();
        Dictionary<GameOverlays, Texture2D> _gameOverlaysHindi = new Dictionary<GameOverlays, Texture2D>();


        Dictionary<Level, Texture2D> _levelTextures = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelTexturesGrey = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelNameEng = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelDescriptionEng = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelNameHindi = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelNameEngGame = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelNameHindiGame = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelDescriptionHindi = new Dictionary<Level, Texture2D>();
        Dictionary<Level, Texture2D> _levelTextEmpty = new Dictionary<Level, Texture2D>();


        public Texture2D GetLevelTexture(Level level, bool levelChosen)
        {
            if( levelChosen)
            {
                if (_levelTextures.ContainsKey(level)) return _levelTextures[level];
            }
            else
            {
                if (_levelTexturesGrey.ContainsKey(level)) return _levelTexturesGrey[level];
            }

            return Missing;
        }

        public Texture2D GetChallengeText()
        {
            if (GameManager.Instance.Language == Language.english)
            {
                var tex = LoadTextureSafeDirect("challengeEng");
                return tex;
            }
            else
            {
                var tex = LoadTextureSafeDirect("challengeHindi");
                return tex;
            }
        }

        public Texture2D GetLevelName(Level level, bool levelChosen)
        {
            if (GameManager.Instance.Language == Language.english)
            {
                if (_levelNameEng.ContainsKey(level)) return _levelNameEng[level];
            }
            else
            {
                if (_levelNameHindi.ContainsKey(level)) return _levelNameHindi[level];
            }
            return Missing;
        }

        public Texture2D GetLevelNameGame(Level level, bool levelChosen)
        {
            if (GameManager.Instance.Language == Language.english)
            {
                if (_levelNameEngGame.ContainsKey(level)) return _levelNameEngGame[level];
            }
            else
            {
                if (_levelNameHindiGame.ContainsKey(level)) return _levelNameHindiGame[level];
            }
            return Missing;
        }

        public Texture2D GetLevelDesc(Level level, bool levelChosen)
        {
            if (GameManager.Instance.Language == Language.english)
            {
                if (levelChosen)
                {
                    if (_levelDescriptionEng.ContainsKey(level)) return _levelDescriptionEng[level];
                }
                else
                {
                    if (_levelTextEmpty.ContainsKey(level)) return _levelTextEmpty[level];
                }
            }
            else
            {
                if (levelChosen)
                {
                    if (_levelDescriptionHindi.ContainsKey(level)) return _levelDescriptionHindi[level];
                }
                else
                {
                    if (_levelTextEmpty.ContainsKey(level)) return _levelTextEmpty[level];
                }
            }

            return Missing;
        }

        public void LoadContent(MyGameBase game)
        {
            _game = game;
            LoadGenericTextures(game);

            LoadNucleotides(game);
            AltLoadNucleotides(game);

            var texture = game.Content.Load<Texture2D>("foregroundHints");
            _textureDatabase.Add("GameForeground", texture);

            texture = game.Content.Load<Texture2D>("foregroundMarker");
            _textureDatabase.Add("ForegroundMarker", texture);

            texture = game.Content.Load<Texture2D>("overlayFailure");
            _textureDatabase.Add("OverlayFailure", texture);

            texture = game.Content.Load<Texture2D>("overlaySuccess");
            _textureDatabase.Add("OverlaySuccess", texture);

            texture = game.Content.Load<Texture2D>("overlayFailureHindi");
            _textureDatabase.Add("OverlayFailureHindi", texture);

            texture = game.Content.Load<Texture2D>("overlaySuccessHindi");
            _textureDatabase.Add("OverlaySuccessHindi", texture);


            texture = game.Content.Load<Texture2D>("sprites/arrow");
            _textureDatabase.Add("Arrow", texture);

            texture = game.Content.Load<Texture2D>("sprites/language-english");
            _textureDatabase.Add("LanguageEnglish", texture);

            texture = game.Content.Load<Texture2D>("sprites/language-hindi");
            _textureDatabase.Add("LanguageHindi", texture);

            LoadBackgroundsSecondScreen();
            LoadAnimations();
            LoadLevels();
            LoadLevelTextsEng();
            LoadLevelTextsHindi();

            LoadGameOverlays();
        }

        /// <summary>
        /// These are the text overlays converted to graphics so that they can be Hindi
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadGameOverlays()
        {
            foreach( GameOverlays o in Enum.GetValues(typeof(GameOverlays)))
            {
                if ( o != GameOverlays.none && o!= GameOverlays.max )
                {
                    string englishTextureName = "overlays\\english\\game_" + o;
                    string hindiTextureName = "overlays\\hindi\\game_" + o;

                    var textureEnglish = LoadTextureSafeDirect(englishTextureName);
                    if(textureEnglish != null )
                    {
                        _gameOverlaysEnglish.Add(o, textureEnglish);
                    }
                    
                    var textureHindi = LoadTextureSafeDirect(hindiTextureName);
                    if (textureHindi != null)
                    {
                        _gameOverlaysHindi.Add(o, textureHindi);
                    }
                }
            }
        }

        public Texture2D GetGameOverlay(GameOverlays gameOverlay, Language lang)
        {
            if (lang == Language.english)
            {
                if (_gameOverlaysEnglish.ContainsKey(gameOverlay))
                {
                    return _gameOverlaysEnglish[gameOverlay];
                }
            }

            if (lang == Language.hindi)
            {
                if (_gameOverlaysHindi.ContainsKey(gameOverlay))
                {
                    return _gameOverlaysHindi[gameOverlay];
                }
            }

            return Blank;
        }

        private void LoadAnimations()
        {
            LoadAnimation("timer", 50, 200, 200, 8, 2, 2);
            LoadAnimation("circlefill", 56, 200, 241, 8, 2, 2);
            LoadAnimation("countdown", 144, 200, 200, 12, 2, 2);
            LoadAnimation("dna", 64, 200, 200, 8, 2, 2);
            LoadAnimation("handwave", 121, 200, 200, 11, 2, 2);
            LoadAnimation("sandtimer", 60, 200, 200, 8, 2, 2); // pretty rubbish
        }

        void LoadLevels()
        {
            foreach(Level l in Enum.GetValues(typeof(Level)))
            {
                if (l!= Level.None && l!= Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levels/"+l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelTextures.Add(l, tex);
                        _levelTexturesGrey.Add(l, ConvertToGreyScale(tex));
                    }
                }
            }
        }

        void LoadLevelTextsEng()
        {
            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelNameEng/" + l.ToString().ToLower());
                    var texEmpty = LoadTextureSafeDirect("levelTextEmpty/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelNameEng.Add(l, tex);
                        _levelTextEmpty.Add(l, texEmpty);
                    }
                }
            }

            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelNameEngGame/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelNameEngGame.Add(l, tex);
                    }
                }
            }

            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelDescriptionEng/" + l.ToString().ToLower());
                    //var texEmpty = LoadTextureSafeDirect("levelTextEmpty/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelDescriptionEng.Add(l, tex);
                        //_levelTextEmpty.Add(l, texEmpty);
                    }
                }
            }
        }

        void LoadLevelTextsHindi()
        {
            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelNameHindi/" + l.ToString().ToLower());
                    //var texEmpty = LoadTextureSafeDirect("levelTextEmpty/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelNameHindi.Add(l, tex);
                        //_levelTextEmpty.Add(l, texEmpty);
                    }
                }
            }

            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelNameHindiGame/" + l.ToString().ToLower());
                    //var texEmpty = LoadTextureSafeDirect("levelTextEmpty/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelNameHindiGame.Add(l, tex);
                        //_levelTextEmpty.Add(l, texEmpty);
                    }
                }
            }

            foreach (Level l in Enum.GetValues(typeof(Level)))
            {
                if (l != Level.None && l != Level.MAX_LEVEL)
                {
                    var tex = LoadTextureSafeDirect("levelDescriptionHindi/" + l.ToString().ToLower());
                    //var texEmpty = LoadTextureSafeDirect("levelTextEmpty/" + l.ToString().ToLower());
                    if (tex != null)
                    {
                        _levelDescriptionHindi.Add(l, tex);
                        //_levelTextEmpty.Add(l, texEmpty);
                    }
                }
            }
        }

        private void LoadAnimation(string animName, int frames, int width, int height, int columns, int paddingX, int paddingY)
        {
            var texture = LoadTextureSafe("animations/" + animName);
            AnimationManager.Instance.AddAnimation(animName, texture, frames, width, height, columns, paddingX, paddingY);
        }

        Video _attractVideo, _failureVideo, _successVideo;

        public Video AttractVideo { get { return _attractVideo; } }
        public Video FailureVideo { get { return _failureVideo; } }
        public Video SuccessVideo { get { return _successVideo; } }

        Dictionary<string, Video> _videoDict = new Dictionary<string, Video>();

        private void LoadVideos(MyGameBase game)
        {
            _attractVideo = game.Content.Load<Video>("videos/attract");
            _failureVideo = game.Content.Load<Video>("videos/failure");
            _successVideo = game.Content.Load<Video>("videos/success");

            _videoDict.Add("attract", _attractVideo);
            _videoDict.Add("failure", _failureVideo);
            _videoDict.Add("success", _successVideo);
        }

        public Video GetVideo(string name)
        {
            if (_videoDict.ContainsKey(name)) return _videoDict[name];

            return null;
        }

        Model _PAMModel, _cylinderModel, _cylinderWithTopModel, _testModel;
        public Model PAMModel { get { return _PAMModel; } }
        public Model CylinderModel { get { return _cylinderModel; } }
        public Model CylinderWithTopModel { get { return _cylinderWithTopModel; } }

        public Model TestModel { get { return _testModel; } }

        public Model DoubleHelixMeshModel { get { return _doubleHelixModel; } }

        Texture2D _testModelTexture = null;
        private Model _doubleHelixModel;
        private Texture2D _modelTexturePAM;
        private Texture2D _modelTextureDoubleHelix, _modelTextureDoubleHelixTrans;
        private Texture2D _modelTextureA;
        private Texture2D _modelTextureC;
        private Texture2D _modelTextureG;
        private Texture2D _modelTextureT;
        private Texture2D _modelTextureNone;

        public Texture2D TestModelTexture { get { return _testModelTexture; } }

        public Texture2D ModelTexturePAM { get { return _modelTexturePAM; } }
        public Texture2D ModelTextureA { get { return _modelTextureA; } }
        public Texture2D ModelTextureC { get { return _modelTextureC; } }
        public Texture2D ModelTextureG { get { return _modelTextureG; } }
        public Texture2D ModelTextureT { get { return _modelTextureT; } }
        public Texture2D ModelTextureNone { get { return _modelTextureNone; } }

        public Texture2D ModelTextureDoubleHelix { get { return _modelTextureDoubleHelix; } }
        public Texture2D ModelTextureDoubleHelixTrans { get { return _modelTextureDoubleHelixTrans; } }

        //private void Load3DModels(MyGameBase game)
        //{
        //    _PAMModel = game.Content.Load<Model>("models/CylinderPAM");
        //    _cylinderModel = game.Content.Load<Model>("models/CylinderPAM");
        //    _cylinderWithTopModel = game.Content.Load<Model>("models/CylinderPAM");
        //    _testModel = game.Content.Load<Model>("models/trees_pine");
        //    _testModelTexture = game.Content.Load<Texture2D>("models/palette");
        //    _doubleHelixModel = game.Content.Load<Model>("models/doubleHelixMesh");

        //    LoadModelTextures(game);
        //}

        //private void LoadModelTextures(MyGameBase game)
        //{
        //    // Load the Textures
        //    _modelTextureA = game.Content.Load<Texture2D>("models/A");
        //    _modelTextureC = game.Content.Load<Texture2D>("models/C");
        //    _modelTextureG = game.Content.Load<Texture2D>("models/G");
        //    _modelTextureT = game.Content.Load<Texture2D>("models/T");
        //    _modelTextureNone = game.Content.Load<Texture2D>("models/None");

        //    _modelTexturePAM = game.Content.Load<Texture2D>("models/PAM");

        //    _modelTextureDoubleHelix = game.Content.Load<Texture2D>("models/doubleHelixPalette");
        //    _modelTextureDoubleHelixTrans = game.Content.Load<Texture2D>("models/doubleHelixPaletteTrans");

        //    // A C G T PAM None
        //}

        void LoadNucleotides(MyGameBase game)
        {
            LoadTextureIntoDictionary(game,"NucleotideLeftA");
            LoadTextureIntoDictionary(game,"NucleotideLeftC");
            LoadTextureIntoDictionary(game,"NucleotideLeftG");
            LoadTextureIntoDictionary(game,"NucleotideLeftT");
            LoadTextureIntoDictionary(game,"NucleotideLeftNone");
            LoadTextureIntoDictionary(game,"NucleotideRightA");
            LoadTextureIntoDictionary(game,"NucleotideRightC");
            LoadTextureIntoDictionary(game,"NucleotideRightG");
            LoadTextureIntoDictionary(game,"NucleotideRightT");
            LoadTextureIntoDictionary(game,"NucleotideRightNone");
            LoadTextureIntoDictionary(game,"NucleotideRightPAM_A", "NucleotideRightPAMA");
            LoadTextureIntoDictionary(game,"NucleotideRightPAM_C", "NucleotideRightPAMC");
            LoadTextureIntoDictionary(game,"NucleotideRightPAM_G", "NucleotideRightPAMG");
            LoadTextureIntoDictionary(game,"NucleotideRightPAM_T", "NucleotideRightPAMT");
            // Hack: I've put these in twice because I'm too lazy to find this issue when lots of other stuff is wrong.
            LoadTextureIntoDictionary(game,"NucleotideRightPAMA", "NucleotideRightPAMA");
            LoadTextureIntoDictionary(game,"NucleotideRightPAMC", "NucleotideRightPAMC");
            LoadTextureIntoDictionary(game,"NucleotideRightPAMG", "NucleotideRightPAMG");
            LoadTextureIntoDictionary(game,"NucleotideRightPAMT", "NucleotideRightPAMT");

            LoadTextureIntoDictionary(game,"ScissorsOpen", "scissors-open");
            LoadTextureIntoDictionary(game,"ScissorsClosed", "scissors-closed");
            LoadTextureIntoDictionary(game,"ScissorsOneSideMsg", "scissors-onesidemsg");
            LoadTextureIntoDictionary(game, "ScissorsNotAvailable", "scissors-notavailable");
            LoadTextureIntoDictionary(game, "ScissorsNotAvailableHindi", "scissors-notavailable-hindi");
        }

        void AltLoadNucleotides(MyGameBase game)
        {
            LoadTextureIntoDictionary(game, "AltNucleotideLeftA");
            LoadTextureIntoDictionary(game, "AltNucleotideLeftC");
            LoadTextureIntoDictionary(game, "AltNucleotideLeftG");
            LoadTextureIntoDictionary(game, "AltNucleotideLeftT");
            LoadTextureIntoDictionary(game, "AltNucleotideLeftNone");
            LoadTextureIntoDictionary(game, "AltNucleotideRightA");
            LoadTextureIntoDictionary(game, "AltNucleotideRightC");
            LoadTextureIntoDictionary(game, "AltNucleotideRightG");
            LoadTextureIntoDictionary(game, "AltNucleotideRightT");
            LoadTextureIntoDictionary(game, "AltNucleotideRightNone");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAM_A", "AltNucleotideRightPAMA");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAM_C", "AltNucleotideRightPAMC");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAM_G", "AltNucleotideRightPAMG");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAM_T", "AltNucleotideRightPAMT");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAMA",  "AltNucleotideRightPAMA");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAMC",  "AltNucleotideRightPAMC");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAMG",  "AltNucleotideRightPAMG");
            LoadTextureIntoDictionary(game, "AltNucleotideRightPAMT", "AltNucleotideRightPAMT");
        }

        private void LoadTextureIntoDictionary(MyGameBase game,string name)
        {
            LoadTextureIntoDictionary(game,name, name);
        }

        private void LoadTextureIntoDictionary(MyGameBase game,string name, string filename)
        {
            var texture = game.Content.Load<Texture2D>("sprites/"+filename);
            _textureDatabase.Add(name,texture);
        }

        /// <summary>
        /// This is a useful tool to help the graphic designers
        /// You can change files in the graphics folder and hot reload them midgame
        /// </summary>
        public void HotReloadTextures()
        {
            _textureDatabase.Remove("BackgroundGameBroken");

            _backgrounds.Clear();
            _backgroundsHindi.Clear();
            _backgroundsSecondScreen.Clear();
            _backgroundsSecondScreenHindi.Clear();

            LoadBackgroundsSecondScreen();
        }

        private void LoadBackgroundsSecondScreen()
        {
            Texture2D texture = LoadTextureSafeDirect("backgrounds/game-broken");
            _textureDatabase.Add("BackgroundGameBroken", texture);

            foreach (GamePhase phase in Enum.GetValues(typeof(GamePhase)))
            {
                if (phase != GamePhase.NONE)
                {
                    AddBackgroundSecondScreenEnglish(phase);
                    AddBackgroundSecondScreenHindi(phase);
                    AddBackgroundHindi(phase);
                    AddBackgroundEnglish(phase);
                }
            }
        }

        private void AddBackgroundEnglish(GamePhase phase)
        {
            string filename = "backgrounds/english55/" + phase;
            var texture = LoadTextureSafeDirect(filename);
            if (texture != null)
            {
                _backgrounds.Add(phase, texture);
            }
        }

        private void AddBackgroundHindi(GamePhase phase)
        {
            string filename = "backgrounds/hindi55/" + phase;
            var texture = LoadTextureSafeDirect(filename);
            if (texture != null)
            {
                _backgroundsHindi.Add(phase, texture);
            }
        }

        private void AddBackgroundSecondScreenHindi(GamePhase phase)
        {
            string filename = "backgrounds/hindi24/" + phase;
            var texture = LoadTextureSafeDirect(filename);
            if (texture != null)
            {
                _backgroundsSecondScreenHindi.Add(phase, texture);
            }
        }

        private void AddBackgroundSecondScreenEnglish(GamePhase phase)
        {
            string filename = "backgrounds/english24/" + phase;
            var texture = LoadTextureSafeDirect(filename);
            if (texture != null)
            {
                _backgroundsSecondScreen.Add(phase, texture);
            }
        }

        public Texture2D GetBackground(GamePhase phase, Language lang)
        {
            if (lang == Language.english)
            {
                if (_backgrounds.ContainsKey(phase))
                {
                    return _backgrounds[phase];
                }
            }

            if (lang == Language.hindi)
            {
                if (_backgroundsHindi.ContainsKey(phase))
                {
                    return _backgroundsHindi[phase];
                }
            }

            return Blank;
        }

        public Texture2D GetBackgroundSecondScreen(GamePhase phase, Language lang = Language.english)
        {
            if (lang == Language.english)
            {
                if (_backgroundsSecondScreen.ContainsKey(phase))
                {
                    return _backgroundsSecondScreen[phase];
                }
            }

            if (lang == Language.hindi)
            {
                if (_backgroundsSecondScreenHindi.ContainsKey(phase))
                {
                    return _backgroundsSecondScreenHindi[phase];
                }
            }

            return Blank;
        }
    }
}