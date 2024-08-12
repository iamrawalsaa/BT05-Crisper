using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using screens;

namespace BT05.screens
{
    public class GameConfig
    {
        public string ComPort { get; set; }
        public string LogsPath{ get; set; }
    }

    public class Game1 : Game
    {
        public GameConfig Config { get; private set; }

        private static readonly Game1 _instance = new Game1();

        public static Game1 Instance
        {
            get { return _instance; }
        }

        public void LoadConfiguration()
        {
            Content.RootDirectory = "Content";
            string configFilePath = Path.Combine(Content.RootDirectory, "config.json");
            if (File.Exists(configFilePath))
            {
                string json = File.ReadAllText(configFilePath);
                Config = JsonConvert.DeserializeObject<GameConfig>(json);
            }
            else
            {
                throw new FileNotFoundException("Configuration file not found.", configFilePath);
            }
        }
    }

}
