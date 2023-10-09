using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared
{
    public class NucleotidePair
    {
        public NucleotideEnum Left = NucleotideEnum.None;
        public NucleotideEnum Right = NucleotideEnum.None;

        public NucleotidePair(NucleotideEnum left, NucleotideEnum right)
        {
            Left = left;
            Right = right;
        }
    }

    public enum GamePhase
    {
        NONE,
        PROTOTYPEMESSAGES,      // details about this prototype
        ATTRACT,                // play a video and tell people to use the scissors
        LANGUAGE,               // Choose language
        INSTRUCTIONS,           // play a video about CAS 9  
        HOWTOPLAY,              // How to play
        LEVELSELECT,            // randomly choose
        GAME,                   // the main game
        RESULT,                 // success / failure - goes back to level success
        ENDGAME,                // Result of 3
        THANKS,                  // thank you for playing
        TEST3D
    }

    public enum HelixSide
    {
        Left,
        Right
    }

    public enum NucleotideEnum
    {
        None,
        A,
        C,
        G,
        T,
        PAM_A,
        PAM_C,
        PAM_G,
        PAM_T,
    }

    public enum Level
    {
        None,
        SickleCell,
        CowMethane,
        Wheat,
        Mosquito,
        HeartDisease,
        MAX_LEVEL,
    }

    public enum ScreenOrientation
    {
        Landscape,
        Portrait,
        ReverseLandscape,
        ReversePortrait,
        MAX
    }
}
