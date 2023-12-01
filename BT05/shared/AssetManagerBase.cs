using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using shared;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharedMonoGame
{
    public class AssetManagerBase
    {
        protected MyGameBase _game;

        protected Dictionary<string, Texture2D> _textureDatabase = new Dictionary<string, Texture2D>();

        protected Dictionary<GamePhase, Texture2D> _backgrounds = new Dictionary<GamePhase, Texture2D>();
        //Dictionary<GameColours, Color> _colors = new Dictionary<GameColours, Color>();

        Dictionary<string, Texture2D> _particles = new Dictionary<string, Texture2D>();
        public SpriteFont FontTitle { get; private set; }
        public SpriteFont FontRegular { get; private set; }
        public SpriteFont FontConsole { get; private set; }
        public SpriteFont FontTimer { get; private set; }
        public SpriteFont FontSpeechBubble { get; private set; }


        Texture2D _blank;
        public Texture2D Blank { get { return _blank; } }

        Texture2D _missing;
        public Texture2D Missing { get { return _missing; } }

        protected AssetManagerBase()
        {
            
        }

        protected void LoadGenericTextures(MyGameBase game)
        {
            LoadBasicTextures(game);
            LoadFonts(game);
            //LoadBackgrounds(game);
            InitColours();
            //LoadParticles(game);
        }

        private void LoadParticles(MyGameBase game)
        {
            foreach(var file in Directory.EnumerateFiles("Content/particles"))
            {
                int index = file.LastIndexOf("\\");
                string name = file.Substring(index+1);
                name = name.Substring(0, name.LastIndexOf("."));

                _particles.Add(name, game.Content.Load<Texture2D>("particles/" + name));
            }
        }

        public Texture2D GetParticleTexture(string name)
        {
            if (_particles.ContainsKey(name))
            {
                return _particles[name];
            }

            return Blank;
        }

        private void LoadBasicTextures(MyGameBase game)
        {
            _blank = game.Content.Load<Texture2D>("blank");
            _missing = game.Content.Load<Texture2D>("missing");
            _textureDatabase.Add("blank", _blank);
            _textureDatabase.Add("missing", _missing);
        }

        private void InitColours()
        {
            //_colors.Add(GameColours.ORANGE, new Color(222, 156, 46));
            //_colors.Add(GameColours.YELLOW, new Color(250, 236, 133));
            //_colors.Add(GameColours.LIGHT_GREEN, new Color(175, 211, 28));
            //_colors.Add(GameColours.BLUE, new Color(148, 194, 242));
            //_colors.Add(GameColours.BROWN, new Color(102, 75, 52));
            //_colors.Add(GameColours.DARK_GREEN, new Color(39, 52, 15));
        }

        //public Color GetContinentColour(ContinentName continent)
        //{
        //    var c = ContinentColours.Instance.GetColour(continent);
        //    return new Color(c.R, c.G, c.B, c.A);
        //}

        private void LoadFonts(MyGameBase game)
        {
            FontTitle = game.Content.Load<SpriteFont>("fonts/FontTitle");
            FontRegular = game.Content.Load<SpriteFont>("fonts/FontNormal");
            FontConsole = game.Content.Load<SpriteFont>("fonts/FontConsole");
            FontTimer = game.Content.Load<SpriteFont>("fonts/FontTimer");
            FontSpeechBubble = game.Content.Load<SpriteFont>("fonts/FontSpeechBubble");
        }

        private void LoadBackgrounds(MyGameBase game)
        {
            foreach (GamePhase phase in Enum.GetValues(typeof(GamePhase)))
            {
                if (phase != GamePhase.NONE)
                {
                    AddBackground(game, phase);
                }
            }
        }

        private void AddBackground(MyGameBase game, GamePhase phase)
        {
            _backgrounds.Add(phase, game.Content.Load<Texture2D>("backgrounds/background-" + phase));
        }

        public Texture2D GetBackground(GamePhase phase)
        {
            if (_backgrounds.ContainsKey(phase))
            {
                return _backgrounds[phase];
            }

            return Blank;
        }

        public Texture2D GetTextureFromName(string textureName)
        {
            if (_textureDatabase.ContainsKey(textureName)) return _textureDatabase[textureName];
            return _textureDatabase["missing"];
        }

        protected Texture2D LoadTexture(string filename)
        {
            return _game.Content.Load<Texture2D>(filename);
        }

        public Texture2D ConvertToGreyScale(Texture2D inputTexture)
        {
            Color[] color = new Color[inputTexture.Width * inputTexture.Height];
            inputTexture.GetData<Color>(color);

            Texture2D outputTexture = new Texture2D(_game.GraphicsDevice, inputTexture.Width, inputTexture.Height);
            int count = 0;

            for (int i = 0; i < color.Length; ++i)
            {
                ++count;
                float grey = (0.299f * color[i].R) + (0.587f * color[i].G) + (0.114f * color[i].B);
                byte b = (byte)grey;

                color[i].R = b;
                color[i].G = b;
                color[i].B = b;
            }

            outputTexture.SetData<Color>(color);

            return outputTexture;
        }

        protected Texture2D LoadTextureSafe(string filename)
        {
            Texture2D texture = null;

            if (!File.Exists(@"content\" + filename + ".xnb"))
            {
                DebugOutput.Instance.WriteError("Expected File is missing: " + filename);
                System.Windows.Forms.MessageBox.Show("Expected File is missing: " + filename, "Asset file missing");
            }
            else
            {
                try
                {
                    texture = _game.Content.Load<Texture2D>(filename);
                }
                catch (Exception ex)
                {
                    DebugOutput.Instance.WriteError("File exists but there is an error loading file: " + filename);
                    DebugOutput.Instance.WriteError(ex.ToString());
                    System.Windows.Forms.MessageBox.Show("File exists but there is an error loading file: " + filename + "\nPossible image is too large in pixels. Must be 4096 or below. Ideally smaller.", "Asset file loading problem");
                }
            }

            return texture;
        }


        /// <summary>
        /// Without using the Content Pipeline
        /// It searches for .png and .pjg files and loads them directly
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        protected Texture2D LoadTextureSafeDirect(string filename)
        {
            Texture2D texture = null;

            string pngFilename = @"graphics\" + filename + ".png";
            string jpgFilename = @"graphics\" + filename + ".jpg";

            bool pngExists = File.Exists(pngFilename);
            bool jpgExists = File.Exists(jpgFilename);

            if ( !pngExists && !jpgExists)
            {
                DebugOutput.Instance.WriteError("Expected File is missing: " + filename);
                System.Windows.Forms.MessageBox.Show("Expected File is missing: " + filename, "Asset file missing");
            }
            else
            {
                try
                {
                    if ( pngExists )
                    {
                        using (var stream = File.OpenRead(pngFilename))
                        {
                            
                            texture = Texture2D.FromStream(_game.Graphics.GraphicsDevice, stream);
                        }
                    }

                    if (jpgExists)
                    {
                        using (var stream = File.OpenRead(pngFilename))
                        {
                            texture = Texture2D.FromStream(_game.Graphics.GraphicsDevice, stream);
                        }
                    }

                }
                catch (Exception ex)
                {
                    DebugOutput.Instance.WriteError("File exists but there is an error loading file: " + filename);
                    DebugOutput.Instance.WriteError(ex.ToString());
                    System.Windows.Forms.MessageBox.Show("File exists but there is an error loading file: " + filename + "\nPossible image is too large in pixels. Must be 4096 or below. Ideally smaller.", "Asset file loading problem");
                }
            }

            return texture;
        }
    }
}