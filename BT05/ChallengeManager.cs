using Microsoft.Xna.Framework;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BT05
{
    public enum ChallengeSequence
    {
        none,
        first,
        second, 
        third,
        max
    }

    public class ChallengeDetail
    {
        const int EASY_SCORE = 1;
        const int MEDIUM_SCORE = 3;
        const int HARD_SCORE = 6;

        ChallengeDifficulty _difficulty = ChallengeDifficulty.none;
        Level _level = Level.None;
        bool _success = false;
        ChallengeSequence _sequence = ChallengeSequence.none;

        public ChallengeDetail( ChallengeDifficulty gameDifficulty, ChallengeSequence sequence)
        {
            _difficulty = gameDifficulty;
            _sequence = sequence;
        }

        public ChallengeDifficulty Difficulty { get { return _difficulty; } }

        public bool Success { 
            get { return _success; }
            set { _success = value; }
        }

        public void Reset()
        {
            _success = false;
            _level = Level.None;
        }

        public Level Challenge
        {
            get { return _level; }
            set { _level = value; }
        }

        public int Score()
        {
            if (_success)
            {
                if (_difficulty == ChallengeDifficulty.easy) return EASY_SCORE;
                if (_difficulty == ChallengeDifficulty.medium) return MEDIUM_SCORE;
                if (_difficulty == ChallengeDifficulty.hard) return HARD_SCORE;
            }

            return 0;
        }

        public int MaxScore()
        {
            if (_difficulty == ChallengeDifficulty.easy) return EASY_SCORE;
            if (_difficulty == ChallengeDifficulty.medium) return MEDIUM_SCORE;
            if (_difficulty == ChallengeDifficulty.hard) return HARD_SCORE;
            return 0;
        }
    }

    public sealed class ChallengeManager
    {
        private static readonly ChallengeManager _instance = new ChallengeManager();

        public static ChallengeManager Instance
        {
            get {
                if (!_instance._initialised) _instance.Initialise();
                return _instance; 
            }
        }

        ChallengeSequence _currentChallenge = ChallengeSequence.first;

        bool _initialised = false;

        Dictionary<ChallengeSequence, ChallengeDetail> _challenges = new Dictionary<ChallengeSequence, ChallengeDetail>();
        public void Initialise()
        {
            _challenges.Clear();
            _challenges.Add(ChallengeSequence.first, new ChallengeDetail(ChallengeDifficulty.easy, ChallengeSequence.first));
            _challenges.Add(ChallengeSequence.second, new ChallengeDetail(ChallengeDifficulty.medium,ChallengeSequence.second));
            _challenges.Add(ChallengeSequence.third, new ChallengeDetail(ChallengeDifficulty.hard,ChallengeSequence.third));

            _initialised = true;
        }

        public string ResultsString()
        {
            return TotalScore() + " / " + MaxScore();
        }

        public Color ScoreColour()
        {
            int totalScore = TotalScore();

            if (totalScore >= 6) { return  Color.Green; }
            if (totalScore > 1) { return Color.Yellow; }
            return Color.Red;
        }

        public int TotalScore()
        {
            int score = 0;

            foreach (var p in _challenges)
            {
                score += p.Value.Score();
            }

            return score;
        }

        public int MaxScore()
        {
            int maxScore = 0;

            foreach (var p in _challenges)
            {
                maxScore += p.Value.MaxScore();
            }

            return maxScore;
        }

        public void Reset()
        {
            foreach(var p in  _challenges)
            {
                p.Value.Reset();
            }

            _currentChallenge = ChallengeSequence.none;
        }

        public void NextChallenge()
        {
            ++_currentChallenge;
        }

        public bool IsFinalChallenge()
        {
            return _currentChallenge == ChallengeSequence.third;
        }

        ChallengeDetail CurrentChallenge
        {
            get { return _challenges[_currentChallenge]; }
        }

        public Level CurrentLevel { 
            get { return _challenges[_currentChallenge].Challenge; } 
            set { _challenges[_currentChallenge].Challenge = value; } 
        }

        public int Turns { 
            get
            {
                if (_currentChallenge == ChallengeSequence.first) return 1;
                if (_currentChallenge == ChallengeSequence.second) return 2;
                if (_currentChallenge == ChallengeSequence.first) return 3;
                return 0;
            }
        }

        public bool CurrentLevelSuccess { 
            get
            {
                return CurrentChallenge.Success; 
            } 
            set
            { 
                CurrentChallenge.Success = value; 
            }
        }

        public ChallengeDifficulty CurrentLevelDifficulty()
        {
            ChallengeDetail challenge = CurrentChallenge;
            return challenge.Difficulty;
        }

        public void JumpToLevel(ChallengeDifficulty easy, Level level)
        {
            _currentChallenge = ChallengeSequence.first;
            CurrentLevel = level;

        }
    }
}
