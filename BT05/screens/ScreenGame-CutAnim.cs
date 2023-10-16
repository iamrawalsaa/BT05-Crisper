using BT05;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Timers;
using MonoGame.Extended.Tweening;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public enum CutAnim
    {
        CutAnim_ScissorsMove,
        CutAnim_DNABreaks,
        CutAnim_NucleotidesMoveOff,
        CutAnim_NucleotidesChange,
        CutAnim_NucleotidesReturn,
        CutAnim_ShowResultSuccess,
        CutAnim_ShowResultFailure,
        CutAnim_ShowCorrectPAMPause,
        CutAnim_ShowCorrectPAM,
        CutAnim_ShowCorrectPAMMessage,
        CutAnim_Complete,
        CutAnim_Stop,
    }

    /// <summary>
    /// This holds all the cut animation stuff
    /// </summary>
    public partial class ScreenGame
    {
        FSMSharp.FSM<CutAnim> _FSMCutAnim = null;
        float _elapsedFSMTime = 0;
        private bool _showDNABreakScreen = false;

        void CutAnim_CreateAllDataAndBegin()
        {
            _FSMCutAnim = new FSMSharp.FSM<CutAnim>("CutAnim");

            _FSMCutAnim.Add(CutAnim.CutAnim_ScissorsMove)
                .OnEnter(() => Begin_CutAnim_ScissorsMove())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: ScissorsMove is going on.. " + d.StateProgress * 100f))
                .Expires(2f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ScissorsMove is ending"))
                .GoesTo(CutAnim.CutAnim_DNABreaks);

            _FSMCutAnim.Add(CutAnim.CutAnim_DNABreaks)
                .OnEnter(() => Begin_CutAnim_DNABreaks())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: DNABreaks is going on.. " + d.StateProgress * 100f))
                .Expires(3f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_DNABreaks is ending"))
                .GoesTo(CutAnim.CutAnim_NucleotidesMoveOff);

            _FSMCutAnim.Add(CutAnim.CutAnim_NucleotidesMoveOff)
                .OnEnter(() => Begin_CutAnim_NucleotidesMoveOff())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: NucleotidesMoveOff is going on.. " + d.StateProgress * 100f))
                .Expires(5f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesMoveOff is ending"))
                .GoesTo(CutAnim.CutAnim_NucleotidesChange);

            _FSMCutAnim.Add(CutAnim.CutAnim_NucleotidesChange)
                .OnEnter(() => Begin_CutAnim_NucleotidesChange())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_NucleotidesChange is going on.. " + d.StateProgress * 100f))
                .Expires(2f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesChange is ending"))
                .GoesTo(CutAnim.CutAnim_NucleotidesReturn);

            _FSMCutAnim.Add(CutAnim.CutAnim_NucleotidesReturn)
                .OnEnter(() => Begin_CutAnim_NucleotidesReturn())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_NucleotidesReturn is going on.. " + d.StateProgress * 100f))
                .Expires(10f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesReturn is ending"));
                //.GoesTo(CutAnim.CutAnim_ShowResult);

            _FSMCutAnim.Add(CutAnim.CutAnim_ShowResultSuccess)
                .OnEnter(() => Begin_CutAnim_ShowResultSuccess())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_ShowResultSuccess is going on.. " + d.StateProgress * 100f))
                .Expires(5f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ShowResultSuccess is ending"))
                .GoesTo(CutAnim.CutAnim_Complete);

            _FSMCutAnim.Add(CutAnim.CutAnim_ShowResultFailure)
                .OnEnter(() => Begin_CutAnim_ShowResultFailure())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_ShowResultFailure is going on.. " + d.StateProgress * 100f))
                .Expires(5f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ShowResultFailure is ending"))
                .GoesTo(CutAnim.CutAnim_ShowCorrectPAMPause);

            _FSMCutAnim.Add(CutAnim.CutAnim_ShowCorrectPAMPause)
                .OnEnter(() => Begin_CutAnim_ShowCorrectPAMPause())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_ShowCorrectPAMPause is going on.. " + d.StateProgress * 100f))
                .Expires(3f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ShowCorrectPAMPause is ending"))
                .GoesTo(CutAnim.CutAnim_ShowCorrectPAM);

            _FSMCutAnim.Add(CutAnim.CutAnim_ShowCorrectPAM)
                .OnEnter(() => Begin_CutAnim_ShowCorrectPAM())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_ShowCorrectPAM is going on.. " + d.StateProgress * 100f))
                .Expires(7f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ShowCorrectPAM is ending"))
                .GoesTo(CutAnim.CutAnim_ShowCorrectPAMMessage);

            _FSMCutAnim.Add(CutAnim.CutAnim_ShowCorrectPAMMessage)
                .OnEnter(() => Begin_CutAnim_ShowCorrectPAMMessage())
                .Calls(d => DebugOutput.Instance.AppendToLiveString("CutAnim: CutAnim_ShowCorrectPAMMessage is going on.. " + d.StateProgress * 100f))
                .Expires(7f)
                .OnLeave(() => DebugOutput.Instance.WriteInfo("CutAnim_ShowCorrectPAMMessage is ending"))
                .GoesTo(CutAnim.CutAnim_Complete);

            _FSMCutAnim.Add(CutAnim.CutAnim_Complete)
                .OnEnter(() => Begin_CutAnim_Complete())
                .Expires(1f)
                .GoesTo(CutAnim.CutAnim_Stop);

            _FSMCutAnim.Add(CutAnim.CutAnim_Stop);

            _FSMCutAnim.CurrentState = CutAnim.CutAnim_ScissorsMove;
            _elapsedFSMTime = 0;
        }

        void UpdateFSM(GameTime gameTime)
        {
            if (_FSMCutAnim != null)
            {
                _elapsedFSMTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_FSMCutAnim.CurrentState == CutAnim.CutAnim_Stop)
                {
                    _FSMCutAnim = null;
                }
                else
                {
                    _FSMCutAnim.Process(_elapsedFSMTime);
                }
            }
        }

        private void Begin_CutAnim_ScissorsMove()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_ScissorsMove is starting");

            ShowSecondaryText = true;
            SecondaryTextOrientation = ScreenOrientation.Portrait;
            SecondaryTextOffset = new Vector2(50, 200);
            SecondaryTextWidth = 25;
            //SecondaryText = "CAS9 Scissors Cut the Double Helix";
            SecondaryText = "";
            _currentGameOverlay = GameOverlays.cut;

            _animationPlaying = true;
        }

        private void Begin_CutAnim_DNABreaks()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_DNABreaks is starting");
            _showDNABreakScreen = true;
        }

        private void Begin_CutAnim_NucleotidesMoveOff()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesMoveOff is starting");
            AnimateCutNucleotidesOffScreen();
            //SecondaryText = "Nucleotides are damaged and disappear";
            SecondaryText = "";
            _currentGameOverlay = GameOverlays.damaged;
        }

        private void Begin_CutAnim_NucleotidesChange()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesChange is starting");
            ChangeNucleotidesWhileOffsceen();
        }

        private void Begin_CutAnim_NucleotidesReturn()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_NucleotidesReturn is starting");
            AnimateCutNucloetidesBackOnScreen();
            //SecondaryText = "The cell repairs its own DNA and the gene is 'Knocked Out'";
            SecondaryText = "";
            _currentGameOverlay = GameOverlays.knock;

            // check for correctAnswer
            if (IsCorrectPAMMarker())
            {
                _FSMCutAnim.GetBehaviour(CutAnim.CutAnim_NucleotidesReturn).GoesTo(CutAnim.CutAnim_ShowResultSuccess);
            }
            else
            {
                _FSMCutAnim.GetBehaviour(CutAnim.CutAnim_NucleotidesReturn).GoesTo(CutAnim.CutAnim_ShowResultFailure);
            }
        }

        private void Begin_CutAnim_ShowResultSuccess()
        {
            _showDNABreakScreen = false;
            ShowSecondaryText = false;
            _currentGameOverlay = GameOverlays.none;
            DebugOutput.Instance.WriteInfo("CutAnim_ShowResultSuccess is starting");
            ChallengeManager.Instance.CurrentLevelSuccess = true;
            ShowSuccessOverlay();
        }

        private void Begin_CutAnim_ShowResultFailure()
        {
            _showDNABreakScreen = false;
            ShowSecondaryText = false;
            _currentGameOverlay = GameOverlays.none;

            DebugOutput.Instance.WriteInfo("CutAnim_ShowResultFailure is starting");
            ChallengeManager.Instance.CurrentLevelSuccess = false;
            ShowFailureOverlay();
            _scissorsComponent.Visible = false;
        }

        private void Begin_CutAnim_ShowCorrectPAMPause()
        {
            ShowSecondaryText = true;
            RemovePAMMarker();
            RemoveOverlayDisplay();
            _scissorsComponent.Visible = false;

            DebugOutput.Instance.WriteInfo("CutAnim_ShowCorrectPAMPause is starting");
            //SecondaryText = "Let's find the correct sequence";
            SecondaryText = "";
            _currentGameOverlay = GameOverlays.find;
        }

        private void Begin_CutAnim_ShowCorrectPAM()
        {
            DebugOutput.Instance.WriteInfo("CutAnim_ShowCorrectPAM is starting");
            ScrollToCorrectPamMarker();
        }

        bool _highlightCorrectAnswerOnSecondScreen = false;

        private void Begin_CutAnim_ShowCorrectPAMMessage()
        {
            _highlightCorrectAnswerOnSecondScreen = true;

            //SecondaryText = "This is the gene you should have knocked out";
            _currentGameOverlay = GameOverlays.actual;
        }

        private void Begin_CutAnim_Complete()
        {
            DebugOutput.Instance.WriteInfo("CutAnim is complete");
            SecondaryText = "";
            _currentGameOverlay = GameOverlays.none;

            ShowSecondaryText = false;

            // Remove PAM Marker
            RemovePAMMarker();
            RemoveOverlayDisplay();

            GotoNextState();
        }

        void AnimateCutNucleotidesOffScreen()
        {
            _offsetNucleotidesTweener.CancelAll();

            // This takes the current PAM marker ID and works backwards from there
            int PAMmarker = (_nucleotideGlobalIndex + 33);
            //_pamMarkerIndexes;

            int minMarker = PAMmarker - 4;

            for (int index = minMarker; index <= PAMmarker; ++index)
            {
                // WORKING HERE: Those are the indexes on the ones on screen!
                //               We only show 50 at a time
                //               Need to figure out how to change these
                //               We want to change it permanently

                int onScreenIndex = LookUpOnScreenIndexFromMainIndex(index);

                if (onScreenIndex != -1)
                {
                    AnimateOneNucleotideOffScreen(_nucleotideWalkersLeft[onScreenIndex]);
                    AnimateOneNucleotideOffScreen(_nucleotideWalkersRight[onScreenIndex]);
                }
            }
        }

        void AnimateOneNucleotideOffScreen(Nucleotide nucleo)
        {
            Vector2 offset = new Vector2(_random.Next(-200, 200), -500);
            float duration = _random.Next(20, 50) / 10f;
            float delay = _random.Next(5, 20) / 10f;

            _offsetNucleotidesTweener.TweenTo(nucleo, a => a.Offset, offset, duration, delay).Easing(EasingFunctions.SineInOut);
        }

        void ChangeNucleotidesWhileOffsceen()
        {
            int PAMmarker = (_nucleotideGlobalIndex + 33);

            int minMarker = PAMmarker - 4;

            for (int index = minMarker; index <= PAMmarker; ++index)
            {
                // randomly choose one
                // make sure the opposite is correct
                var leftNucleotide = NucleotideEnum.None;
                var rightNucleotide = NucleotideEnum.None;

                leftNucleotide = GenerateRandomNucleotide();
                rightNucleotide = GetMatchingNucleotide(leftNucleotide);

                // firstly replace in the long sequence

                NucleotidePair pair = new NucleotidePair(leftNucleotide, rightNucleotide);
                _longNucleotideSequence[index] = pair;

                // secondly - replace in the ones on screen

                var leftTex = GetLeftTexture(pair.Left);
                var rightTex = GetRightTexture(pair.Right);

                var leftTexAlt = GetLeftTextureAlt(pair.Left);
                var rightTexAlt = GetRightTextureAlt(pair.Right);


                int onScreenIndex = LookUpOnScreenIndexFromMainIndex(index);

                if (onScreenIndex != -1)
                {
                    _nucleotideWalkersLeft[onScreenIndex].LoadContent(leftTex, leftTexAlt, pair.Left);
                    _nucleotideWalkersRight[onScreenIndex].LoadContent(rightTex, rightTexAlt, pair.Right);

                    // also change offset to bottom of the screen
                    _nucleotideWalkersLeft[onScreenIndex].Offset = new Vector2(_nucleotideWalkersLeft[onScreenIndex].Offset.X, 1000);
                    _nucleotideWalkersRight[onScreenIndex].Offset = new Vector2(_nucleotideWalkersRight[onScreenIndex].Offset.X, 1000);
                }
            }
        }

        void AnimateCutNucloetidesBackOnScreen()
        {
            _offsetNucleotidesTweener.CancelAll();

            // This takes the current PAM marker ID and works backwards from there
            int PAMmarker = (_nucleotideGlobalIndex + 33);

            int minMarker = PAMmarker - 4;

            for (int index = minMarker; index <= PAMmarker; ++index)
            {
                int onScreenIndex = LookUpOnScreenIndexFromMainIndex(index);

                if (onScreenIndex != -1)
                {
                    AnimateOneNucleotideOnScreen(_nucleotideWalkersLeft[onScreenIndex]);
                    AnimateOneNucleotideOnScreen(_nucleotideWalkersRight[onScreenIndex]);
                }
            }
        }

        void AnimateOneNucleotideOnScreen(Nucleotide nucleo)
        {
            Vector2 offset = new Vector2(0, 0);
            float duration = _random.Next(20, 50) / 10f;
            float delay = _random.Next(5, 20) / 10f;

            _offsetNucleotidesTweener.TweenTo(nucleo, a => a.Offset, offset, duration, delay).Easing(EasingFunctions.SineInOut);
        }

        private void RemoveOverlayDisplay()
        {
            _showOverlayFailure = false;
            _showOverlaySuccess = false;
            _movementAllowed = true;
            _scissorsComponent.ResetAfterAnimation();
        }

        void GotoNextState()
        {
            //ChallengeManager.Instance.CurrentLevelSuccess = _showOverlaySuccess;
            GameManager.Instance.NextGameState(_nextPhase);
        }

        public void ScissorCut_callback()
        {
            // block all movement
            _movementAllowed = false;

            CutAnim_CreateAllDataAndBegin();
        }

        public void ScissorAnimationComplete_callback()
        {
            //AnimateCutNucleotidesOffScreen();
        }

        private void ShowFailureOverlay()
        {
            _showOverlayFailure = true;
            _overlayDisplayTime = 3;
        }

        private void ShowSuccessOverlay()
        {
            _showOverlaySuccess = true;
            _overlayDisplayTime = 3;
        }

        int LookUpOnScreenIndexFromMainIndex(int index)
        {
            for( int i = 0; i< _nucleotideWalkersLeft.Count; ++i )
            {
                var n = _nucleotideWalkersLeft[i];
                if (n.SequenceIndex == index) return i;
            }

            return -1;
        }
    }
}