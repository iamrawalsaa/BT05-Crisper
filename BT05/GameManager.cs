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
        Random _random = new Random();

        private static readonly GameManager _instance = new GameManager();

        public static GameManager Instance
        {
            get { return _instance; }
        }

        Level _level = Level.CowMethane;
        GamePhase _phase = GamePhase.NONE;

        public Level Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public GamePhase GamePhase { get { return _phase; } }

        public int Turns { get; internal set; } = 0;

        bool _lastRoundWinnder = false;
        public bool LastRoundWinner {
            get { return _lastRoundWinnder; }
            set { _lastRoundWinnder = value;
                DebugOutput.Instance.WriteInfo("Last Round Winner: " + _lastRoundWinnder);
            } 
        }

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
            Turns = 1;
        }

        public void NextGameState()
        {
            GamePhase next = GamePhase.NONE;
            if (_phase == GamePhase.THANKS)
            {
                next = GamePhase.ATTRACT;
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

                    screenPrototype.PrimaryText = "{{WHITE}}This is a playable version of BT05.\nThe final game may be played across two screens. This version can be played on just one.\n{{RED}}See printed instructions.";
                    screenPrototype.SecondaryText = "{{WHITE}}Keys\n{{BLUE}}Enter: {{WHITE}}jump to next state\n{{BLUE}}G:{{WHITE}} Large Window\n{{BLUE}}F:{{WHITE}} Fullscreen";

                    screenPrototype.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenPrototype.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    
                    break;
                case GamePhase.ATTRACT:
                    ScreenAttract screenAttract = MyScreenManager.Instance.GetScreen(GamePhase.ATTRACT) as ScreenAttract;

                    screenAttract.PrimaryTextOffset = new Vector2(200, 0);
                    screenAttract.SecondaryTextOffset = new Vector2(100, 0);

                    screenAttract.PrimaryText = "{{WHITE}}Stand on the floor markers and operate the Scissors together.";
                    screenAttract.ShowPrimaryText = false;

                    screenAttract.SecondaryText = "{{WHITE}}Keys\n{{BLUE}}Z: {{WHITE}}Player 1 scissors\n{{BLUE}}RIGHT MOUSE:{{WHITE}} Player 2 scissors\n";

                    screenAttract.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenAttract.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.LANGUAGE:
                    ScreenLanguage screenLanguage = MyScreenManager.Instance.GetScreen(GamePhase.LANGUAGE) as ScreenLanguage;

                    screenLanguage.PrimaryTextOffset = new Vector2(-200, 0);
                    screenLanguage.SecondaryTextOffset = new Vector2(-200, 0);


                    screenLanguage.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenLanguage.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    screenLanguage.PrimaryText = "{{WHITE}}Turn rope to choose language\n\nभाषा चुनने के लिए रस्सी घुमाएँ";
                    screenLanguage.SecondaryText = "{{WHITE}}Operate scissors to select";

                    break;
                case GamePhase.INSTRUCTIONS:
                    ScreenInstructions screenInstructions = MyScreenManager.Instance.GetScreen(GamePhase.INSTRUCTIONS) as ScreenInstructions;

                    screenInstructions.PrimaryText = "{{WHITE}}You play the role of a {{GREEN}}CAS9{{WHITE}} Protein.\n\n{{GREEN}}CAS9{{WHITE}} can be used to knock out genes\n\n{{GREEN}}CAS9{{WHITE}}:\n{{RED}}1. {{YELLOW}}Unravels{{WHITE}} DNA into single strands\n{{RED}}2. {{WHITE}}Looks for a piece of {{YELLOW}}matching{{WHITE}} DNA\n{{RED}}3. {{YELLOW}}Cuts {{WHITE}}it.\n\nThis causes the cell to repair it's DNA with different pieces - rendering the sequence knocked out.";
                    screenInstructions.SecondaryText = "{{WHITE}}Read more about how CAS9 works in the other exhibits.\n";

                    screenInstructions.PrimaryTextWidth = 40;
                    screenInstructions.SecondaryTextWidth = 30;
                    screenInstructions.PrimaryTextOffset = new Vector2(300, 0);

                    screenInstructions.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenInstructions.SecondaryTextOrientation = ScreenOrientation.Portrait;

                    break;
                case GamePhase.HOWTOPLAY:
                    ScreenHowToPlay screenHowToPlay = MyScreenManager.Instance.GetScreen(GamePhase.HOWTOPLAY) as ScreenHowToPlay;
                    screenHowToPlay.PrimaryTextOffset = new Vector2(400, 0);
                    screenHowToPlay.PrimaryText = "{{GREEN}}HOW TO PLAY (SINGLE SCREEN)\n\n{{WHITE}}P1 (can't see screen): pulls the DNA through (using the mouse)\n\nP2: looks at the screen and says when a {{HOT_PINK}}PINK PAM{{WHITE}} marker is aligned\n\nTogether: decide if the RNA sequence matches\n\nIf they match then operate scissors\nIf not, move to next PAM marker";
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

                    ScreenEndGame screenEndGame = MyScreenManager.Instance.GetScreen(GamePhase.ENDGAME) as ScreenEndGame;

                    screenEndGame.PrimaryTextOrientation = ScreenOrientation.Portrait;
                    screenEndGame.SecondaryTextOrientation = ScreenOrientation.Portrait;
                    break;
                case GamePhase.THANKS:
                    ScreenThanks screenThanks = MyScreenManager.Instance.GetScreen(GamePhase.THANKS) as ScreenThanks;

                    screenThanks.PrimaryText = "{{WHITE}}You've played the role of a CAS9 protein used for gene editing. This game shows you a simplified view of this incredible process. CAS9 is at the cutting edge of gene editing. You can learn more in the other exhibits.";
                    screenThanks.SecondaryText = "{{BLUE}}Thanks for playing\n";

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

        public void GenerateNewLevelChoice()
        {
            _level = (Level)_random.Next(1, (int)(Level.MAX_LEVEL));
        }
    }
}