using System;
using System.Collections.Generic;
using System.Text;

namespace shared
{
    /// <summary>
    /// Global constants shared between client, viewer and server
    /// 
    /// This is where we can update the Game version
    /// </summary>
    public sealed class GlobalConstants
    {
        DateTime _launchDateTime = DateTime.Now;

        private static readonly GlobalConstants _instance = new GlobalConstants();

        public static GlobalConstants Instance
        {
            get { return _instance; }
        }

        public DateTime LaunchDateTime
        {
            get { return _launchDateTime; }
            set { _launchDateTime = value; }
        }

        public string LaunchDateTimeString
        {
            get { return _launchDateTime.ToString("yyMMdd_HHmmss"); }
        }


        public bool SAVE_LOG_FILE = true;

        public bool TESTING_NO_TEXT_RENDER = true; // this is a test bit of code to disable Text rendering as I think it has a memory leak

        public bool SONGS_ENABLED = true;
        public float VOLUME_MUSIC = 0.5f;
        public float VOLUME_SFX = 0.5f;
        public float DEFAULT_MIN_PLAY_INTERVAL_PER_SOUND = 0.5f;

        public string VERSION_STRING = "Playtest5";
    }
}