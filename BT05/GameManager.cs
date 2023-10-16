using Microsoft.Xna.Framework;
using screens;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    public sealed class GameManager
    {
        //ChallengeDifficulty _gameDifficulty = ChallengeDifficulty.easy;

        //public ChallengeDifficulty GameDifficulty { get { return ChallengeManager.Instance. } }



        Random _random = new Random();

        private static readonly GameManager _instance = new GameManager();

        public static GameManager Instance
        {
            get { return _instance; }
        }

        //Level _level = Level.CowMethane;
        GamePhase _phase = GamePhase.NONE;

        public Level CurrentLevel
        {
            get { return ChallengeManager.Instance.CurrentLevel; }
        }

        public GamePhase GamePhase { get { return _phase; } }

        //public int Turns { get; internal set; } = 0;

        //bool _lastRoundWinnder = false;
        //public bool LastRoundWinner
        //{
        //    get { return _lastRoundWinnder; }
        //    set
        //    {
        //        _lastRoundWinnder = value;
        //        DebugOutput.Instance.WriteInfo("Last Round Winner: " + _lastRoundWinnder);
        //    }
        //}

        Language _language = Language.english;
        public Language Language
        {
            get { return _language; }
            set
            {
                _language = value;
                DebugOutput.Instance.WriteInfo("Language Selected: " + _language);
            }
        }

        public void ResetGame()
        {
            //Turns = 1;
            ChallengeManager.Instance.Reset();
        }


        public void NextGameState(GamePhase nextPhase = GamePhase.NONE)
        {
            GamePhase next = GamePhase.NONE;
            
            if (nextPhase != GamePhase.NONE)
            {
                next = nextPhase;
            }
            else
            {
                next = _phase;
                ++next;
            }

            GotoPhase(next);
        }

        public void GotoPhase(GamePhase next)
        {
            LeavingPhase(_phase);
            _phase = next;
            ArrivingPhase(_phase);
        }

        private void ArrivingPhase(GamePhase phase)
        {
            MyScreenManager.Instance.FadeToScreen(phase);

            switch (phase)
            {
                case GamePhase.NONE:
                    break;
                case GamePhase.PROTOTYPEMESSAGES:
                    ScreenPrototypeMessages screenPrototype = MyScreenManager.Instance.GetScreen(GamePhase.PROTOTYPEMESSAGES) as ScreenPrototypeMessages;

                    screenPrototype.PrimaryText = "{{HOT_PINK}}Version: 5.00 16th Oct 2023\n\n{{WHITE}}This is a playable version of BT05.\nThis runs across two screens.\nIt requires input from Third Space on the text content & Bakarmax on the graphics\n";
                    //screenPrototype.SecondaryText = "{{HOT_PINK}}Keys\n{{BLUE}}Enter: {{WHITE}}jump to next state\n{{BLUE}}G:{{WHITE}} Large Window\n{{BLUE}}F:{{WHITE}} Fullscreen\n{{BLUE}}V:{{WHITE}} Dual screen mode\n{{BLUE}}M:{{WHITE}} Mini screens (dual screen mode)\n{{BLUE}}B:{{WHITE}} Swap screens (dual screen mode)\n{{BLUE}}W:{{WHITE}} Wave\n{{BLUE}}N:{{WHITE}} Toggle mouse in Game Mode\n{{BLUE}}";
                    screenPrototype.SecondaryText = "{{HOT_PINK}}Keys\n{{BLUE}}Enter: {{WHITE}}jump to next state\n{{BLUE}}V:{{WHITE}} Dual screen mode\n{{BLUE}}M:{{WHITE}} Mini screens (dual screen mode)\n{{BLUE}}B:{{WHITE}} Swap screens (dual screen mode)\n{{BLUE}}W:{{WHITE}} Wave\n{{BLUE}}N:{{WHITE}} Toggle mouse in Game Mode\n{{BLUE}}";

                    //screenPrototype.PrimaryText = "";// "{{WHITE}}Selecting RANDOM {{HOT_PINK}}challenge{{WHITE}} on partner's screen.\n\nYou partner will describe the challenge {{BLUE}}ICON{{WHITE}} to you.  Look up the {{GREEN}}DNA{{WHITE}} sequence.\n\nYou need the opposing sequence.\nIt can be helpful to build it to help you remember.";
                    //screenPrototype.SecondaryText = "";// "{{WHITE}}Keys\n{{BLUE}}Enter: {{WHITE}}jump to next state\n{{BLUE}}G:{{WHITE}} Large Window\n{{BLUE}}F:{{WHITE}} Fullscreen\n{{BLUE}}V:{{WHITE}} Dual screen mode\n{{BLUE}}W:{{WHITE}} Wave";

                    screenPrototype.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenPrototype.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    screenPrototype.PrimaryTextOffset = new Vector2(100, 0);
                    screenPrototype.SecondaryTextOffset = new Vector2(-300, 0);

                    break;
                case GamePhase.ATTRACT:
                    ScreenAttract screenAttract = MyScreenManager.Instance.GetScreen(GamePhase.ATTRACT) as ScreenAttract;

                    screenAttract.PrimaryTextOffset = new Vector2(200, 0);
                    screenAttract.SecondaryTextOffset = new Vector2(100, 0);

                    screenAttract.PrimaryText = "{{WHITE}}Stand on the floor markers and operate the Scissors together to begin.";
                    screenAttract.ShowPrimaryText = true;

                    //screenAttract.SecondaryText = "{{WHITE}}Keys\n{{BLUE}}Z: {{WHITE}}Player 1 scissors\n{{BLUE}}RIGHT MOUSE:{{WHITE}} Player 2 scissors\n";
                    screenAttract.SecondaryText = "";

                    screenAttract.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenAttract.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.LANGUAGE:
                    ScreenLanguage screenLanguage = MyScreenManager.Instance.GetScreen(GamePhase.LANGUAGE) as ScreenLanguage;

                    screenLanguage.PrimaryTextOffset = new Vector2(-200, 0);
                    screenLanguage.SecondaryTextOffset = new Vector2(-200, 0);


                    screenLanguage.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenLanguage.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    screenLanguage.PrimaryText = "{{WHITE}}Turn knob to choose language\n\nभाषा चुनने के लिए रस्सी घुमाएँ";
                    screenLanguage.SecondaryText = "{{WHITE}}Operate scissors with partner to select";

                    break;
                case GamePhase.INSTRUCTIONS:
                    ScreenInstructions screenInstructions = MyScreenManager.Instance.GetScreen(GamePhase.INSTRUCTIONS) as ScreenInstructions;

                    screenInstructions.PrimaryText = "";// {{WHITE}}You play the role of a {{GREEN}}CAS9{{WHITE}} Protein.\n\n{{GREEN}}CAS9{{WHITE}} can be used to knock out genes\n\n{{GREEN}}CAS9{{WHITE}}:\n{{RED}}1. {{YELLOW}}Unravels{{WHITE}} DNA into single strands\n{{RED}}2. {{WHITE}}Looks for a piece of {{YELLOW}}matching{{WHITE}} DNA\n{{RED}}3. {{YELLOW}}Cuts {{WHITE}}it.\n\nThis causes the cell to repair it's DNA with different pieces - rendering the sequence knocked out.";
                    screenInstructions.SecondaryText = "";// {{WHITE}}Read more about how CAS9 works in the other exhibits.\n";

                    screenInstructions.PrimaryTextWidth = 40;
                    screenInstructions.SecondaryTextWidth = 30;
                    screenInstructions.PrimaryTextOffset = new Vector2(300, 0);

                    screenInstructions.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenInstructions.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.HOWTOPLAY:
                    ScreenHowToPlay screenHowToPlay = MyScreenManager.Instance.GetScreen(GamePhase.HOWTOPLAY) as ScreenHowToPlay;
                    screenHowToPlay.PrimaryTextOffset = new Vector2(400, 0);
                    //screenHowToPlay.PrimaryText = "{{GREEN}}HOW TO PLAY (SINGLE SCREEN)\n\n{{WHITE}}P1 (can't see screen): pulls the DNA through (using the mouse)\n\nP2: looks at the screen and says when a {{HOT_PINK}}PINK PAM{{WHITE}} marker is aligned\n\nTogether: decide if the RNA sequence matches\n\nIf they match then operate scissors\nIf not, move to next PAM marker";
                    //screenHowToPlay.PrimaryText = "{{GREEN}}A two player collaborate game\n\n{{WHITE}}You are the EYES\nYour partner has the CONTROLS\n\nGive your partner instructions to align the {{HOT_PINK}}PINK PAM{{WHITE}} marker\n\nTogether you must decide if the RNA sequence matches\n\nIf this is the matching sequence then CUT with the {{BLUE}}CRISPR{{WHITE}} 'genetic scissors'\nIf not, move to next {{HOT_PINK}}PAM{{WHITE}} marker\n\n{{RED}}You will face {{WHITE}}3{{RED}} challenges";
                    //screenHowToPlay.PrimaryText = "{{GREEN}}A two player collaborate game\n\n{{WHITE}}You have the CONTROLS\nYour partner will be your EYES\n\nListen to your partner's instructions to align the {{HOT_PINK}}PINK PAM{{WHITE}} marker\n\nYou must decide if the sequence on your partner's screen matches your RNA sequence\n\nIf this is the matching sequence then CUT with the {{BLUE}}CRISPR{{WHITE}} 'genetic scissors'\nIf not, move to next {{HOT_PINK}}PAM{{WHITE}} marker\n\n{{RED}}You will face {{WHITE}}3{{RED}} challenges";
                    screenHowToPlay.PrimaryText = "";

                    //screenHowToPlay.SecondaryText = "{{BLACK}}Read more about how CAS9 works in the other exhibits.\n";

                    screenHowToPlay.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenHowToPlay.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.LEVELSELECT:
                    ScreenLevelSelect screenLevelSelect = MyScreenManager.Instance.GetScreen(GamePhase.LEVELSELECT) as ScreenLevelSelect;

                    screenLevelSelect.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenLevelSelect.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.GAME:
                    ScreenGame screenGame = MyScreenManager.Instance.GetScreen(GamePhase.GAME) as ScreenGame;

                    screenGame.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenGame.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.RESULT:
                    ScreenResult screenResult = MyScreenManager.Instance.GetScreen(GamePhase.RESULT) as ScreenResult;

                    screenResult.PrimaryTextOffset = new Vector2(-200, 0);
                    screenResult.SecondaryTextOffset = new Vector2(100, 0);
                    screenResult.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenResult.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.ENDGAME:

                    ScreenSummaryGame screenEndGame = MyScreenManager.Instance.GetScreen(GamePhase.ENDGAME) as ScreenSummaryGame;

                    screenEndGame.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenEndGame.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    break;
                case GamePhase.THANKS:
                    ScreenThanks screenThanks = MyScreenManager.Instance.GetScreen(GamePhase.THANKS) as ScreenThanks;

                    // screenThanks.PrimaryText = "{{WHITE}}You've played the role of a CAS9 protein used for gene editing. This game shows you a simplified view of this incredible process. CRISPR CAS9 is at the cutting edge of gene editing. You can learn more in the other exhibits in the BioTech Zone.";
                    // screenThanks.SecondaryText = "{{BLUE}}Thanks for playing\n";

                    screenThanks.PrimaryText = "";
                    screenThanks.SecondaryText = "";

                    screenThanks.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenThanks.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.TEST3D:
                    ScreenTest3D screenTest3D = MyScreenManager.Instance.GetScreen(GamePhase.TEST3D) as ScreenTest3D;

                    screenTest3D.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenTest3D.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    break;
                default:
                    break;
            }
        }

        private void LeavingPhase(GamePhase phase)
        {
        }

        /// <summary>
        /// Generate a new Level
        /// Make sure it's not the same as the current one
        /// </summary>
        public void GenerateNewLevelChoice()
        {
            Level newLevel = Level.None;
            do
            {
                newLevel = (Level)_random.Next(1, (int)(Level.MAX_LEVEL));
            } while (ChallengeManager.Instance.CurrentLevel == newLevel);
            ChallengeManager.Instance.CurrentLevel = newLevel;
        }

        /// <summary>
        /// TODO: probably need to update this
        /// </summary>
        /// <param name="newValue"></param>
        public void LanguageChangedByArduino(Language newValue)
        {
            Language = newValue;
        }

        internal void NextGameState(object nextPhase)
        {
            throw new NotImplementedException();
        }
    }
}