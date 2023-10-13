using BT05;
using Microsoft.Xna.Framework;
using MonoGame.SplineFlower;
using MonoGame.SplineFlower.Content;
using MonoGame.SplineFlower.Spline;
using MonoGame.SplineFlower.Spline.Types;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using MonoGame.Extended;
using SharpDX.Direct3D9;
using MonoGame.Extended.Tweening;
using System.CodeDom.Compiler;
using SharpDX.MediaFoundation;

namespace screens
{
    public partial class ScreenGame : GameScreenExtended
    {
        int _singleNucleotideSection = 50;

        SplineBase _helixSplineLeft;
        SplineBase _helixSplineRight;

        Random _random = new Random();
        const int TIME_TO_LOOP = 20;

        List<Nucleotide> _nucleotideWalkersLeft = new List<Nucleotide>();
        List<Nucleotide> _nucleotideWalkersRight = new List<Nucleotide>();

        List<NucleotidePair> _longNucleotideSequence = new List<NucleotidePair>();
        Dictionary<NucleotideEnum, string> _leftNucleotideTextureNames = new Dictionary<NucleotideEnum, string>();
        Dictionary<NucleotideEnum, string> _rightNucleotideTextureNames = new Dictionary<NucleotideEnum, string>();
        Dictionary<NucleotideEnum, string> _rightPAMTextureNames = new Dictionary<NucleotideEnum, string>();
        Dictionary<NucleotideEnum, NucleotideEnum> _rightPAMMatch = new Dictionary<NucleotideEnum, NucleotideEnum>();

        private int _nucleotideGlobalIndex = 0;
        private float _splineScale = 1.0f;

        bool DEBUG_MODE = false;
        private float _xDiff;

        string _completeDNAStrandRight = "";
        string _completeDNAStrandRight_NoReturns = "";
        private float _targetMouseXDiff;
        ScissorsComponent _scissorsComponent = null;

        float _minProgress = 0.05f;
        float _maxProgress = 0.95f;

        List<int> _pamMarkerIndexes = new List<int>();
        int _correctIndex = 0;

        float _overlayDisplayTime = 0;
        Dictionary<NucleotideEnum, NucleotideEnum> _nucleotideMatches = new Dictionary<NucleotideEnum, NucleotideEnum>();

        private bool _showOverlayFailure = false;
        private bool _showOverlaySuccess = false;
        private bool _contentLoaded = false;
        private bool _drawNucleotides = true;

        private TransformDummy[] _leftpd;
        private ControlPointModeDummy[] _leftpmd;
        private TriggerDummy[] _lefttd;
        private TransformDummy[] _lefttand;

        private int _closestSplinePoint = -1;
        private int _closestSplineTangent = -1;
        private bool _drawLeft = true;
        private bool _drawRight = true;

        private TransformDummy[] _rightpd;
        private ControlPointModeDummy[] _rightpmd;
        private TriggerDummy[] _righttd;
        private TransformDummy[] _righttand;

        Tweener _offsetNucleotidesTweener = new Tweener();

        private byte _foregroundAlpha;

        /// <summary>
        /// This allows incoming movement from the keyboard to be processed
        /// </summary>
        bool _movementAllowed = true;

        float _markerAlignmentOffset = 0;

        public ScreenGame(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void ScreenArriving()
        {
            ShowPrimaryText = false;
            ShowSecondaryText = false;
            _highlightCorrectAnswerOnSecondScreen = false;
            _movementAllowed = true;
            ++GameManager.Instance.Turns;
            base.ScreenArriving();
        }

        public override void LoadContent()
        {
            LoadContentOnce();

            CreateLongerNucleotideSequences(_singleNucleotideSection * 5);
            AddNucleotideWalkersFromLongerSection(_singleNucleotideSection, 0.02f);
            BlankOutAllTheNucleotides();

            _nextNucleotideToDisplay = 0;
            _timeToDisplayNextNucleotide = 2.0f;

            base.LoadContent();
        }

        private void LoadContentOnce()
        {
            if (!_contentLoaded)
            {

                MonoGame.SplineFlower.Content.Setup.Initialize(GraphicsDevice);

                _scissorsComponent = new ScissorsComponent(Game);
                _scissorsComponent.SetCompleteDelegates(ScissorCut_callback, ScissorAnimationComplete_callback);
                AddDrawable(_scissorsComponent);

                //Components.Add(_scissorsComponent);

                CreateNucelotideMatches();

                _leftNucleotideTextureNames.Add(NucleotideEnum.A, "NucleotideLeftA");
                _leftNucleotideTextureNames.Add(NucleotideEnum.C, "NucleotideLeftC");
                _leftNucleotideTextureNames.Add(NucleotideEnum.G, "NucleotideLeftG");
                _leftNucleotideTextureNames.Add(NucleotideEnum.T, "NucleotideLeftT");
                _leftNucleotideTextureNames.Add(NucleotideEnum.None, "NucleotideLeftNone");


                _rightNucleotideTextureNames.Add(NucleotideEnum.A, "NucleotideRightA");
                _rightNucleotideTextureNames.Add(NucleotideEnum.C, "NucleotideRightC");
                _rightNucleotideTextureNames.Add(NucleotideEnum.G, "NucleotideRightG");
                _rightNucleotideTextureNames.Add(NucleotideEnum.T, "NucleotideRightT");
                _rightNucleotideTextureNames.Add(NucleotideEnum.None, "NucleotideRightNone");
                _rightNucleotideTextureNames.Add(NucleotideEnum.PAM_A, "NucleotideRightPAMA");
                _rightNucleotideTextureNames.Add(NucleotideEnum.PAM_C, "NucleotideRightPAMC");
                _rightNucleotideTextureNames.Add(NucleotideEnum.PAM_G, "NucleotideRightPAMG");
                _rightNucleotideTextureNames.Add(NucleotideEnum.PAM_T, "NucleotideRightPAMT");

                _rightPAMTextureNames.Add(NucleotideEnum.A, "NucleotideRightPAMA");
                _rightPAMTextureNames.Add(NucleotideEnum.C, "NucleotideRightPAMC");
                _rightPAMTextureNames.Add(NucleotideEnum.G, "NucleotideRightPAMG");
                _rightPAMTextureNames.Add(NucleotideEnum.T, "NucleotideRightPAMT");

                _rightPAMMatch.Add(NucleotideEnum.A, NucleotideEnum.PAM_A);
                _rightPAMMatch.Add(NucleotideEnum.C, NucleotideEnum.PAM_C);
                _rightPAMMatch.Add(NucleotideEnum.G, NucleotideEnum.PAM_G);
                _rightPAMMatch.Add(NucleotideEnum.T, NucleotideEnum.PAM_T);
                _rightPAMMatch.Add(NucleotideEnum.None, NucleotideEnum.None);

                LoadSplineJSON();

                var open = SharedAssetManager.Instance.GetTextureFromName("ScissorsOpen");
                var closed = SharedAssetManager.Instance.GetTextureFromName("ScissorsClosed");
                var msg = SharedAssetManager.Instance.GetTextureFromName("ScissorsOneSideMsg");
                var notAvailable = SharedAssetManager.Instance.GetTextureFromName("ScissorsNotAvailable");
                _scissorsComponent.SetTextures(open, closed, msg, notAvailable);

                Setup.ShowPoints = false;
                Setup.ShowDirectionVectors = false;
                Setup.ShowLines = false;
                Setup.ShowBaseLine = false;
                Setup.ShowTangents = false;
                Setup.ShowCenterSpline = false;
                Setup.ShowSpline = false;
            }

            _contentLoaded = true;
        }

        /// <summary>
        /// This is what moves the spline walkers along
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns></returns>
        GameTime GameTimeLocal(GameTime gameTime)
        {
            if (_autoScroll)
            {
                int currentIDUnderMarker = _nucleotideGlobalIndex + 29;
                int target = _correctIndex;

                int distance = Math.Abs(currentIDUnderMarker - target);
                distance = Math.Clamp(distance, 0, 5) * _autoscrollDirection;

                // probably need to know distance between current and correct
                if (IsCorrectPAMMarker())
                {
                    _autoScroll = false;
                    return new GameTime(gameTime.TotalGameTime, TimeSpan.FromSeconds(0));
                }
                else
                {
                    return new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * distance);
                }
            }    

            return new GameTime(gameTime.TotalGameTime, gameTime.ElapsedGameTime * _targetMouseXDiff);
        }

        public void ScrollToCorrectPamMarker()
        {
            _autoScroll = true;

            int currentIDUnderMarker = _nucleotideGlobalIndex + 29;
            int target = _correctIndex;

            int distance = Math.Abs(currentIDUnderMarker - target);

            if (currentIDUnderMarker> target)
            {
                _autoscrollDirection = 1;
            }
            else
            {
                _autoscrollDirection = -1;
            }

            if (distance> _longNucleotideSequence.Count /2)
            {
                _autoscrollDirection *= -1;
            }
        }

        public override void Update(GameTime gameTime)
        {
            ShowTheNucleotides(gameTime);

            _offsetNucleotidesTweener.Update( gameTime.GetElapsedSeconds() );
            UpdateFSM(gameTime);

            float maxSpeed = 2;   // was 3
            float maxChange = 0.5f;  // was 2

            UpdateDrawables(gameTime);

            _xDiff = Math.Clamp(_xDiff, -maxChange, maxChange);

            // This is a bit more challenging to add smoothing to this.
            _targetMouseXDiff += _xDiff;
            _targetMouseXDiff = Math.Clamp(_targetMouseXDiff, -maxSpeed, maxSpeed);
            _targetMouseXDiff *= 0.85f;

            _helixSplineLeft.Update(gameTime);
            _helixSplineRight.Update(gameTime);

            GameTime gameTimeLocal = GameTimeLocal(gameTime);

            for (int i = 0; i < _nucleotideWalkersLeft.Count; ++i)
            {
                _nucleotideWalkersLeft[i].Update(gameTimeLocal);
                _nucleotideWalkersRight[i].Update(gameTimeLocal);
            }

            var direction = Math.Sign(gameTimeLocal.ElapsedGameTime.TotalSeconds);

            float progressDiff = _maxProgress - _minProgress;

            if (direction < 0)
            {
                // should find the lowest value and only process that
                int id = GetLowestNucleotide();

                if (id != -1)
                {
                    var progress = _nucleotideWalkersLeft[id].GetProgress;
                    if (progress < _minProgress)
                    {
                        var target = progress + progressDiff;

                        _nucleotideWalkersLeft[id].SetPosition(target);
                        _nucleotideWalkersRight[id].SetPosition(target);

                        var actual = _nucleotideWalkersRight[id].GetProgress;

                        if (target != actual)
                        {
                            int error = 1;
                            ++error;
                        }

                        NucleotideGlobalInc();

                        // HACK: Trying an off by one fix
                        // I think this has worked
                        int sequenceIndex = ExtensionMethods.Mod( NucleotideGlobalIndexOffset()-1,  250);
                        var nucleotidePair = _longNucleotideSequence[sequenceIndex];
                        shared.DebugOutput.Instance.WriteInfo("ID: " + id + " Original: " + progress + " Changed to: " + actual + " New Nuc: " + nucleotidePair.Right);

                        var leftTex = GetLeftTexture(nucleotidePair.Left);
                        var rightTex = GetRightTexture(nucleotidePair.Right);

                        var leftTexAlt = GetLeftTextureAlt(nucleotidePair.Left);
                        var rightTexAlt = GetRightTextureAlt(nucleotidePair.Right);


                        _nucleotideWalkersLeft[id].LoadContent(leftTex, leftTexAlt, nucleotidePair.Left);
                        _nucleotideWalkersRight[id].LoadContent(rightTex, rightTexAlt, nucleotidePair.Right);

                        _nucleotideWalkersLeft[id].SequenceIndex = sequenceIndex;
                        _nucleotideWalkersRight[id].SequenceIndex = sequenceIndex;

                    }
                }
            }

            if (direction > 0)
            {
                int id = GetHighestNucleotide();

                if (id != -1)
                {
                    var progress = _nucleotideWalkersLeft[id].GetProgress;

                    if (progress > _maxProgress)
                    {
                        var target = progress - progressDiff;
                        _nucleotideWalkersLeft[id].SetPosition(target);
                        _nucleotideWalkersRight[id].SetPosition(target);

                        var actual = _nucleotideWalkersRight[id].GetProgress;

                        if (target != actual)
                        {
                            int error = 1;
                            ++error;
                        }

                        //NucleotideGlobalInc();
                        NucleotideGlobalDec();

                        var nucleotidePair = _longNucleotideSequence[_nucleotideGlobalIndex];
                        var leftTex = GetLeftTexture(nucleotidePair.Left);
                        var rightTex = GetRightTexture(nucleotidePair.Right);

                        var leftTexAlt = GetLeftTextureAlt(nucleotidePair.Left);
                        var rightTexAlt = GetRightTextureAlt(nucleotidePair.Right);

                        _nucleotideWalkersLeft[id].LoadContent(leftTex, leftTexAlt, nucleotidePair.Left);
                        _nucleotideWalkersRight[id].LoadContent(rightTex, rightTexAlt, nucleotidePair.Right);

                        int sequenceIndex = _nucleotideGlobalIndex;
                        _nucleotideWalkersLeft[id].SequenceIndex = sequenceIndex;
                        _nucleotideWalkersRight[id].SequenceIndex = sequenceIndex;
                    }
                }
            }

            CheckForValidSequenceOnScreen();

            //CheckOverlayDisplay(gameTime);

            if (IsPAMMarker())
            {
                _scissorsComponent.ScissorsAvailable = true;
            }
            else
            {
                _scissorsComponent.ScissorsAvailable = false;
            }

            // Update Foreground overlay. Hide it during the scissor animation
            if (IsPAMMarker() && _movementAllowed)
            {
                if (_foregroundAlpha <= 250)
                {
                    _foregroundAlpha+=5;
                }
            }
            else
            {
                if (_foregroundAlpha > 2)
                {
                    _foregroundAlpha-=3;
                }
            }

            base.Update(gameTime);
        }

        int _nextNucleotideToDisplay = 0;

        /// <summary>
        /// This animates the Nucleotides on to the screen one by one at the beginning of a level
        /// </summary>
        /// <param name="gameTime"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void ShowTheNucleotides(GameTime gameTime)
        {
            if (_nextNucleotideToDisplay < _nucleotideWalkersLeft.Count )
            {
                _timeToDisplayNextNucleotide -= gameTime.GetElapsedSeconds();
                if (_timeToDisplayNextNucleotide <0)
                {
                    _nucleotideWalkersLeft[_nextNucleotideToDisplay].ShowTexture = true;
                    _nucleotideWalkersRight[_nextNucleotideToDisplay].ShowTexture = true;

                    ++_nextNucleotideToDisplay;
                    _timeToDisplayNextNucleotide = 0.1f;
                }
            }
        }

        /// <summary>
        /// This is used to look for the bug between the on-screen and complete list
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void CheckForValidSequenceOnScreen()
        {
            // TESTING
            // return immediately
            // I was getting an exception here
            // System.ExecutionEngineException
            
            string resultLeft = "";
            string resultRight = "";



            for(int i = 0; i< _nucleotideWalkersLeft.Count; ++i)
            {
                var left = _nucleotideWalkersLeft[i];
                var right = _nucleotideWalkersRight[i];

                resultLeft += " " + i + ":" + left.SequenceIndex;
                if ( i!=0 && i%10 == 0)
                {
                    resultLeft += "\n";
                }
            }

            int lowIndex = GetLowestNucleotide();
            int highIndex = GetHighestNucleotide();

            resultLeft += "\nLowest: " + lowIndex + " Highest: " + highIndex + "\n";
            _completeSequenceOnScreen = resultLeft;
        }

        public override void DrawSecondScreenInner(GameTime gameTime)
        {
            DisplaySecondScreenBackground();

            if (_movementAllowed)
            {
                _displayAlternativeTexture = true;
                DrawSpline();
            }
            else
            { 
                DrawStandard();
                DrawTextTexture();
            }
            base.DrawSecondScreenInner(gameTime);
        }

        Vector2 _drawOverlayLocation = new Vector2(0, 0);

        public override void DrawInner(GameTime gameTime)
        {
            DrawStandard();
        }

        private void DrawStandard()
        {
            if (_showDNABreakScreen)
            {
                Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("BackgroundGameBroken"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            }

            _displayAlternativeTexture = false;
            DrawSpline();

            string correctFeedback = CheckCorrectIndex();

            string _debugString = "                                                                 Level: " + GameManager.Instance.Level + "\n" +
                "                                                                 ID: " + _nucleotideGlobalIndex + " " + correctFeedback + "\n";
            _debugString += "\n" + CurrentArea();
            // _debugString+= "\n" + "\n" + _completeSequenceOnScreen;
            // _debugString += "\n" + _completeDNAStrandRight;


            //if (IsPAMMarker())
            {
                Microsoft.Xna.Framework.Color foreground = new Microsoft.Xna.Framework.Color(_foregroundAlpha, _foregroundAlpha, _foregroundAlpha, _foregroundAlpha);
                float offset = ClosestPAMMarkerDistance();

                _debugString += "Progress: " + offset;

                float offsetScaleFactor = 3;
                if (offset != 0)
                {
                    _drawOverlayLocation = new Vector2((offset * offsetScaleFactor) - 100, 0);
                }

                Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("GameForeground"), _drawOverlayLocation, foreground);
                Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("ForegroundMarker"), new Microsoft.Xna.Framework.Rectangle(0, 0, 1920, 1080), foreground);
            }

            if (_showOverlayFailure)
            {
                Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlayFailure"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            }

            if (_showOverlaySuccess)
            {
                Game._spriteBatch.Draw(SharedAssetManager.Instance.GetTextureFromName("OverlaySuccess"), Vector2.Zero, Microsoft.Xna.Framework.Color.White);
            }

            if (Setup.ShowPoints)
            {
                DrawMarkers();
            }

            Game._spriteBatch.DrawString(SharedAssetManager.Instance.FontConsole, _debugString, new Vector2(50, 850), Microsoft.Xna.Framework.Color.White);
        }

        private void DrawMarkers()
        {
            if (_closestSplinePoint != -1)
            {
                var pos = _leftpd[_closestSplinePoint].Position;
                Game._spriteBatch.DrawCircle(pos, 50, 10, Microsoft.Xna.Framework.Color.LightSkyBlue);
            }

            if (_closestSplineTangent != -1)
            {
                var pos = _lefttand[_closestSplineTangent].Position;
                Game._spriteBatch.DrawCircle(pos, 50, 10, Microsoft.Xna.Framework.Color.LightPink);
            }
        }

        private string CheckCorrectIndex()
        {
            string info = IsPAMMarker() ? "PAM" : "NOT PAM";

            if (IsCorrectPAMMarker() ) 
            { 
                info = "CORRECT PAM" ; 
            }

            return info;
        }

        bool IsPAMMarker()
        {
            foreach(var pamindex in _pamMarkerIndexes)
            {
                if ((_nucleotideGlobalIndex+33) == pamindex) return true;
            }
            return false;
        }

        /// <summary>
        /// This returns a value between 0.57 and 0.59
        /// </summary>
        /// <returns></returns>
        float ClosestPAMMarkerDistance()
        {
            //float progress = 0;
            foreach (var pamindex in _pamMarkerIndexes)
            {

                if ((_nucleotideGlobalIndex + 33) == pamindex)
                {
                    var progress = _nucleotideWalkersLeft[33].GetProgress;

                    return progress.Remap(0.57f, 0.59f, -10f, +10f);
                }
            }
            return 0;
        }

        bool IsCorrectPAMMarker()
        {
            return (_nucleotideGlobalIndex+29) == _correctIndex;
        }

        /// <summary>
        /// This removes the PAM marker at the current location
        /// </summary>
        void RemovePAMMarker()
        {
            int currentPAMMarkerID = _nucleotideGlobalIndex + 33;
            _pamMarkerIndexes.Remove(currentPAMMarkerID);
        }

        private void DrawSpline()
        {
            //_displayAlternativeTexture = true;
            //foreach (var keypair in _gameTextures)
            //{
            //    var texture = keypair.Value;
            //    //_spriteBatch.Draw(texture, RandomPostion(), Color.White);
            //}

            if (_drawLeft)
            {
                _helixSplineLeft.Draw(Game._spriteBatch);
            }

            if (_drawRight)
            {
                if (_helixSplineRight != null)
                {
                    _helixSplineRight.Draw(Game._spriteBatch);
                }
            }

            if (_drawNucleotides)
            {
                if (_drawLeft)
                {
                    for(int i = 0; i<_nucleotideWalkersLeft.Count; ++i)
//                    foreach (var n in _nucleotideWalkersLeft)
                    {
                        var n = _nucleotideWalkersLeft[i];
                        n.DisplayAltTexture = _displayAlternativeTexture;

                        //_highlightCorrectAnswerOnSecondScreen = true;
                        if (_highlightCorrectAnswerOnSecondScreen)
                        {
                            // enable the correct answer for certain
                            // 
                            bool isThisTheCorrectAnswer = IsCorrectPAMMarker();
                            if (isThisTheCorrectAnswer)
                            {
                                if (i >= 33 && i < 38)
                                {
                                    n.DisplayAltTexture = false;
                                }
                            }
                        }
                        n.Draw(Game._spriteBatch);
                    }
                }

                if (_drawRight)
                {
                    foreach (var n in _nucleotideWalkersRight)
                    {
                        n.DisplayAltTexture = _displayAlternativeTexture;
                        //n.DisplayAltTexture = false;
                        n.Draw(Game._spriteBatch);
                    }
                }
            }
        }

        bool _displayAlternativeTexture = true;

        private void CreateNucelotideMatches()
        {
            _nucleotideMatches.Add(NucleotideEnum.A, NucleotideEnum.T);
            _nucleotideMatches.Add(NucleotideEnum.C, NucleotideEnum.G);
            _nucleotideMatches.Add(NucleotideEnum.G, NucleotideEnum.C);
            _nucleotideMatches.Add(NucleotideEnum.T, NucleotideEnum.A);
        }

        NucleotideEnum GetMatchingNucleotide(NucleotideEnum nucleotide)
        {
            return _nucleotideMatches[nucleotide];
        }

        public NucleotideEnum GenerateRandomNucleotide()
        {
            List<NucleotideEnum> allNucleotides = new List<NucleotideEnum>();
            allNucleotides.Add(NucleotideEnum.A);
            allNucleotides.Add(NucleotideEnum.C);
            allNucleotides.Add(NucleotideEnum.G);
            allNucleotides.Add(NucleotideEnum.T);

            var index = _random.Next(allNucleotides.Count);
            return allNucleotides[index];
        }

        void NucleotideGlobalInc()
        {
            ++_nucleotideGlobalIndex;

            if (_nucleotideGlobalIndex >= _longNucleotideSequence.Count)
            {
                _nucleotideGlobalIndex = 0;
            }
        }

        void NucleotideGlobalDec()
        {
            --_nucleotideGlobalIndex;
            if (_nucleotideGlobalIndex < 0)
            {
                _nucleotideGlobalIndex = _longNucleotideSequence.Count - 1;
            }
        }

        /// <summary>
        /// This is used to get the value on the right hand side. 50 increment
        /// </summary>
        /// <returns></returns>
        int NucleotideGlobalIndexOffset()
        {
            var val = _nucleotideGlobalIndex + 50;

            if (val >= _longNucleotideSequence.Count)
            {
                val -= _longNucleotideSequence.Count;
            }

            return val;
        }

        private void AddNucleotideWalkersFromLongerSection(int count, float spacing, int indexOffset = 0)
        {
            ClearAllNucleotideWalkers();

            // copy from longer to start
            float startPosition = _minProgress;

            float actualSpacing = (_maxProgress - _minProgress) / count;
            spacing = actualSpacing;

            for (int i = indexOffset; i < count + indexOffset; ++i)
            {
                int index = i;
                if (index > 250) index -= 250;

                var leftNucleotide = _longNucleotideSequence[index].Left;
                var rightNucleotide = _longNucleotideSequence[index].Right;

                var leftNucleotideTex = LeftNucleotideName(leftNucleotide);
                var rightNucleotideTex = RightNucleotideName(rightNucleotide);

                AddNucleotideWalker(leftNucleotide, leftNucleotideTex, HelixSide.Left, startPosition, index);
                AddNucleotideWalker(rightNucleotide, rightNucleotideTex, HelixSide.Right, startPosition, index);

                startPosition += spacing;
            }
        }

        private void ClearAllNucleotideWalkers()
        {
            foreach( var v in _nucleotideWalkersLeft )
            {
                
            }

            _nucleotideWalkersLeft.Clear();
            _nucleotideWalkersRight.Clear();
        }

        private void CreateLongerNucleotideSequences(int count)
        {
            _longNucleotideSequence = new List<NucleotidePair>();

            _pamMarkerIndexes = new List<int>();

            int nextPamMarkerIndex = 37;

            for (int i = 0; i < count; ++i)
            {
                var leftNucleotide = NucleotideEnum.None;
                var rightNucleotide = NucleotideEnum.None;

                if (!DEBUG_MODE)
                {
                    //leftNucleotide = GenerateRandomNucleotide();
                    leftNucleotide = GenerateNextNucleotide();
                    rightNucleotide = GetMatchingNucleotide(leftNucleotide);
                }

                // Replace right with PAM on occasion
                //if (i != 0 && i % 37 == 0)
                if (i == nextPamMarkerIndex)
                {
                    rightNucleotide = GetMatchingPAMEnum(rightNucleotide);

                    _pamMarkerIndexes.Add(i);
                    nextPamMarkerIndex += 50;
                }

                NucleotidePair pair = new NucleotidePair(leftNucleotide, rightNucleotide);

                _longNucleotideSequence.Add(pair);
            }

            AddPAMrnaSequences(_pamMarkerIndexes);
            CreateDebugInfo();
        }

        private void BlankOutAllTheNucleotides()
        {
            foreach(var nucleotide in _nucleotideWalkersLeft)
            {
                nucleotide.ShowTexture = false;
            }

            foreach (var nucleotide in _nucleotideWalkersRight)
            {
                nucleotide.ShowTexture = false;
            }
        }

        NucleotideEnum _previousRandomlyGenerated = NucleotideEnum.None;
        private string _completeSequenceOnScreen = "";
        private bool _autoScroll = false;
        private int _autoscrollDirection;
        private float _timeToDisplayNextNucleotide;

        private NucleotideEnum GenerateNextNucleotide()
        {
            switch (_previousRandomlyGenerated)
            {
                case NucleotideEnum.None:
                    _previousRandomlyGenerated = NucleotideEnum.C;
                    break;
                case NucleotideEnum.A:
                    _previousRandomlyGenerated = NucleotideEnum.T;
                    break;
                case NucleotideEnum.C:
                    _previousRandomlyGenerated = NucleotideEnum.A;
                    break;
                case NucleotideEnum.G:
                    break;
                case NucleotideEnum.T:
                    _previousRandomlyGenerated = NucleotideEnum.C;
                    break;
                case NucleotideEnum.PAM_A:
                    break;
                case NucleotideEnum.PAM_C:
                    break;
                case NucleotideEnum.PAM_G:
                    break;
                case NucleotideEnum.PAM_T:
                    break;
            }
            return _previousRandomlyGenerated;
        }

        private void CreateDebugInfo()
        {
            _completeDNAStrandRight = "";

            for (int i = 0; i < _longNucleotideSequence.Count; i++)
            {
                var pair = _longNucleotideSequence[i];
                if (i != 0 && i % 50 == 0) _completeDNAStrandRight += "\n";

                if (pair.Right == NucleotideEnum.None)
                {
                    _completeDNAStrandRight += ".";
                    _completeDNAStrandRight_NoReturns += ".";
                }
                else
                {
                    if (pair.Right == NucleotideEnum.PAM_A || pair.Right == NucleotideEnum.PAM_C || pair.Right == NucleotideEnum.PAM_G || pair.Right == NucleotideEnum.PAM_T)
                    {
                        var extra = pair.Right.ToString().Last().ToString().ToUpper();
                        _completeDNAStrandRight += extra;
                        _completeDNAStrandRight_NoReturns += extra;
                    }
                    else
                    {
                        var extra = pair.Right.ToString().Last().ToString().ToLower(); ;
                        _completeDNAStrandRight += extra;
                        _completeDNAStrandRight_NoReturns += extra;
                    }
                }
            }
        }

        private void AddPAMrnaSequences(List<int> pamMarkerIndexes)
        {
            int PAMOffset = 4;

            // copy the wrong ones in everywhere

            for (int i = 0; i < pamMarkerIndexes.Count; ++i)
            {
                int PAMIndex = pamMarkerIndexes[i];
                var sequence = LevelDatabase.Instance.GetWrongNucleotides(GameManager.Instance.Level, i);

                var startingIndex = PAMIndex - PAMOffset;
                ReplaceRNASequence(sequence, startingIndex);
            }

            // replace one with the right one
            // previously this was always at the same location
            // I think it should be anywhere by the first 1's in both direction
            // So, not 0 or 4... leaving 1,2,3
            int targetPAM = _random.Next(1, 3);
            var correctSequence = LevelDatabase.Instance.GetCorrectNucleotides(GameManager.Instance.Level);
            _correctIndex = pamMarkerIndexes[targetPAM] - PAMOffset;
            ReplaceRNASequence(correctSequence, _correctIndex);
        }

        /// <summary>
        /// This overwrites what is in the RNA sequence with a new one
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="startingIndex"></param>
        private void ReplaceRNASequence(List<NucleotideEnum> sequence, int startingIndex)
        {
            for (int j = 0; j < sequence.Count; ++j)
            {
                var leftNucleotide = sequence[j];
                var rightNucleotide = GetMatchingNucleotide(leftNucleotide);
                if (j == sequence.Count - 1)
                {
                    rightNucleotide = GetMatchingPAMEnum(rightNucleotide);
                }

                var index = startingIndex + j;
                _longNucleotideSequence[index] = new NucleotidePair(leftNucleotide, rightNucleotide);
            }
        }

        //private void AddNucleotideWalkers(int count, float spacing)
        //{
        //    float startPosition = 0;
        //    for (int i = 0; i < count; ++i)
        //    {
        //        var leftNucleotide = GenerateRandomNucleotide();
        //        var rightNucelotide = GetMatchingNucleotide(leftNucleotide);

        //        var leftNucleotideTex = LeftNucleotideName(leftNucleotide);
        //        var rightNucleotideTex = RightNucleotideName(rightNucelotide);

        //        // Replace right with PAM on occasion
        //        if (i != 0 && i % 37 == 0)
        //        {
        //            rightNucleotideTex = GetMatchingPamTex(rightNucelotide);
        //        }

        //        AddNucleotideWalker(leftNucleotideTex, HelixSide.Left, startPosition);
        //        AddNucleotideWalker(rightNucleotideTex, HelixSide.Right, startPosition);

        //        startPosition += spacing;
        //    }
        //}

        NucleotideEnum GetMatchingPAMEnum(NucleotideEnum rightNucelotide)
        {
            return _rightPAMMatch[rightNucelotide];
            //            return _nucleotideMatches[rightNucelotide];
        }

        private string GetMatchingPamTex(NucleotideEnum rightNucelotide)
        {
            return _rightPAMTextureNames[rightNucelotide];
        }

        private string RightNucleotideName(NucleotideEnum rightNucelotide)
        {
            return _rightNucleotideTextureNames[rightNucelotide];
        }

        private string LeftNucleotideName(NucleotideEnum leftNucleotide)
        {
            return _leftNucleotideTextureNames[leftNucleotide];
        }


        //string RandomlyGenerateNucleotideName()
        //{
        //    List<string> names = new List<string>();
        //    foreach (var keypair in _gameTextures)
        //    {
        //        names.Add(keypair.Key);
        //    }

        //    int index = _random.Next(names.Count);

        //    return names[index];
        //}

        void AddNucleotideWalker(NucleotideEnum nucleotideEnum, string nucleotideName, HelixSide helixSide, float position = 1, int index = 0)
        {
            var nucleotide = new Nucleotide();
            // TODO: This is not efficient
            // TODO: Add cache of nucleotides

            var texture = SharedAssetManager.Instance.GetTextureFromName(nucleotideName);
            var altTexture = SharedAssetManager.Instance.GetTextureFromName("Alt" + nucleotideName);

            //nucleotide.LoadContent(_gameTextures[nucleotideName]);
            nucleotide.LoadContent(texture, altTexture, nucleotideEnum);
            nucleotide.SequenceIndex = index;

            switch (helixSide)
            {
                case HelixSide.Left:
                    nucleotide.CreateSplineWalker(_helixSplineLeft, SplineWalker.SplineWalkerMode.Once, TIME_TO_LOOP);
                    break;
                case HelixSide.Right:
                    nucleotide.CreateSplineWalker(_helixSplineRight, SplineWalker.SplineWalkerMode.Once, TIME_TO_LOOP);
                    break;
                default:
                    break;
            }

            nucleotide.SetPosition(position);

            switch (helixSide)
            {
                case HelixSide.Left:
                    nucleotide.RotationType = RotationTypeEnum.FixedLeft;
                    nucleotide.IsLeft= true;
                    _nucleotideWalkersLeft.Add(nucleotide);
                    break;
                case HelixSide.Right:
                    nucleotide.RotationType = RotationTypeEnum.FixedRight;
                    _nucleotideWalkersRight.Add(nucleotide);
                    break;
                default:
                    break;
            }
        }

        private Texture2D GetRightTexture(NucleotideEnum right)
        {
            string textureName = "NucleotideRight" + right.ToString();
            return SharedAssetManager.Instance.GetTextureFromName(textureName);
        }

        private Texture2D GetLeftTexture(NucleotideEnum left)
        {
            string textureName = "NucleotideLeft" + left.ToString();
            return SharedAssetManager.Instance.GetTextureFromName(textureName);
        }

        private Texture2D GetRightTextureAlt(NucleotideEnum right)
        {
            string textureName = "AltNucleotideRight" + right.ToString();
            return SharedAssetManager.Instance.GetTextureFromName(textureName);
        }

        private Texture2D GetLeftTextureAlt(NucleotideEnum left)
        {
            string textureName = "AltNucleotideLeft" + left.ToString();
            return SharedAssetManager.Instance.GetTextureFromName(textureName);
        }

        /// <summary>
        /// gets the rightmost 
        /// </summary>
        /// <returns></returns>
        private int GetHighestNucleotide()
        {
            // only do this on the left as both will be the same

            if (_nucleotideWalkersLeft.Count == 0) return -1;

            int id = 0;
            float maxProgress = 0f;
            for (int i = 0; i < _nucleotideWalkersLeft.Count; ++i)
            {
                var progress = _nucleotideWalkersLeft[i].GetProgress;
                if (progress > maxProgress)
                {
                    maxProgress = progress;
                    id = i;
                }
            }

            return id;
        }

        /// <summary>
        /// returns the left most nucleotide
        /// </summary>
        /// <returns></returns>
        private int GetLowestNucleotide()
        {
            // only do this on the left as both will be the same
            int id = 0;
            float minProgress = 1f;
            for (int i = 0; i < _nucleotideWalkersLeft.Count; ++i)
            {
                var progress = _nucleotideWalkersLeft[i].GetProgress;
                if (progress < minProgress)
                {
                    minProgress = progress;
                    id = i;
                }
            }

            return id;
        }

        public void ResetSpline()
        {
            AddNucleotideWalkersFromLongerSection(_singleNucleotideSection, 0.02f, _nucleotideGlobalIndex);
        }

        private string CurrentArea()
        {
            int length = _completeDNAStrandRight_NoReturns.Length;

            int remaining = length - _nucleotideGlobalIndex;
            int l = Math.Min(remaining, _singleNucleotideSection);

            return _completeDNAStrandRight_NoReturns.Substring(_nucleotideGlobalIndex, l);
        }

        //Vector2 RandomPostion()
        //{
        //    var x = _random.Next(_graphics.PreferredBackBufferWidth);
        //    var y = _random.Next(_graphics.PreferredBackBufferHeight);

        //    return new Vector2(x, y);
        //}

        public void SaveSplineJSON()
        {
            var JsonSerializerSetup = new JsonSerializerSettings();
            JsonSerializerSetup.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            JsonSerializerSetup.NullValueHandling = NullValueHandling.Ignore;
            JsonSerializerSetup.DefaultValueHandling = DefaultValueHandling.Include;
            JsonSerializerSetup.TypeNameHandling = TypeNameHandling.Auto;
            JsonSerializerSetup.Converters.Add(new StringEnumConverter());

            var splineFilename = "Content/MainHelixSpline2.json";

            //var splineDataLeft =
            //    JsonConvert.DeserializeObject<SplineData>(
            //        File.ReadAllText(splineFilename), JsonSerializerSetup);

            SplineData splineData = new SplineData();
            splineData.PointData = _leftpd;
            splineData.PointModeData = _leftpmd;
            splineData.TriggerData = _lefttd;
            splineData.TangentData = _lefttand;

            string jsonString = JsonConvert.SerializeObject(splineData, JsonSerializerSetup);
            File.WriteAllText(splineFilename, jsonString);

        }

        void LoadSplineJSON()
        {
            var JsonSerializerSetup = new JsonSerializerSettings();
            JsonSerializerSetup.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            JsonSerializerSetup.NullValueHandling = NullValueHandling.Ignore;
            JsonSerializerSetup.DefaultValueHandling = DefaultValueHandling.Include;
            JsonSerializerSetup.TypeNameHandling = TypeNameHandling.Auto;
            JsonSerializerSetup.Converters.Add(new StringEnumConverter());

            var splineFilename = "Content/MainHelixSpline3.json";

            var splineDataLeft =
                JsonConvert.DeserializeObject<SplineData>(
                    File.ReadAllText(splineFilename), JsonSerializerSetup);

            var splineDataRight =
            JsonConvert.DeserializeObject<SplineData>(
                File.ReadAllText(splineFilename), JsonSerializerSetup);


            _leftpd = splineDataLeft.PointData;
            _leftpmd = splineDataLeft.PointModeData;
            _lefttd = splineDataLeft.TriggerData;
            _lefttand = splineDataLeft.TangentData;

            _rightpd = splineDataLeft.PointData;
            _rightpmd = splineDataLeft.PointModeData;
            _righttd = splineDataLeft.TriggerData;
            _righttand = splineDataLeft.TangentData;

            ScaleSplineData();

            CreateLeftSpline();
            CreateRightSpline();
        }

        public void RopeChanged(float ropeChangedX)
        {
            if (_movementAllowed)
            {
                _xDiff = ropeChangedX;
            }
        }

        public void ScissorsClosed()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.ScissorsClosed();
            }
        }

        public void ScissorsOpen()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.ScissorsOpen();
            }
        }

        public void OneSideClosed()
        {
            if (_scissorsComponent != null && _movementAllowed)
            {
                _scissorsComponent.OneSideClosed();
            }
        }

        public override void ScreenLeaving()
        {
            ClearAllNucleotideWalkers();
            _longNucleotideSequence.Clear();
            _pamMarkerIndexes.Clear();
            _nucleotideGlobalIndex = 0;
            base.ScreenLeaving();
        }

        public override void MouseLeftClicked(int x, int y)
        {
            base.MouseLeftClicked(x, y);
        }
    }
}