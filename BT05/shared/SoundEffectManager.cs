using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMonoGame
{
    public enum SoundEffectEnum
    {
        LEVEL_SELECT_RANDOM,
        LEVEL_SELECT_CHOSEN
    }

    /// <summary>
    /// Handles all playback of sound effects on client and viewer
    /// 
    /// It supports multiple sound effects per name, randomly choosing from them to create variety
    /// </summary>
    public sealed class SoundEffectManager
    {
        Dictionary<SoundEffectEnum, List<string>> _soundMapping = new Dictionary<SoundEffectEnum, List<string>>();

        Dictionary<int, SoundEffect> _piano = new Dictionary<int, SoundEffect>();
        
        public void PlayPiano(int id)
        {
            // id is double the value so 0 to 20
            var p = _piano[id];
            p.Play(0.2f,1f,1f);
        }

        //public void LoadPianoSoundEffects()
        //{
        //    for (int i=0;i<=20;++i)
        //    {
        //        string path = "Content/" + _contentDirectory + "/piano/" + (i+20) + ".wav";
        //        _piano.Add(i, SoundEffect.FromStream(new FileStream(path, FileMode.Open)));
        //    }
        //}

        public void PlaySound( SoundEffectEnum soundEffect, float volume = 1.0f )
        {
            var listOfSounds = _soundMapping[soundEffect];

            if (listOfSounds.Count > 0 )
            {
                var filename = listOfSounds.GetRandomElement();
                PlaySound(filename, volume);
            }
        }

        void AddSoundMapping(SoundEffectEnum e, string filename)
        {
            _soundMapping[e].Add(filename);
        }

        public void SetupSoundMapping()
        {
            foreach( SoundEffectEnum e in Enum.GetValues(typeof(SoundEffectEnum)))
            {
                _soundMapping[e] = new List<string>();
                AddAllMatchingSounds(e);
            }

            

            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_CLICK_GENERIC, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_JOINGAME, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_NEWMESSAGE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_PLEDGE_INCREASE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_PLEDGE_DECREASE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_PLEDGE_PRESS, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_PLEDGE_RELEASE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_MONEYARRIVE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_MONEYSPEND, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_MONEYRECEIVE, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_CARDCLICK, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_WHOOSH, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_BUY, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_SELL, "");
            //AddSoundMapping(SoundEffectEnum.CLIENT_SND_IMP_NOTENOUGH, "");
        }

        private void AddAllMatchingSounds(SoundEffectEnum e)
        {
            int soundAdded = 0;
            foreach(var name in _names)
            {
                if (name.StartsWith(e.ToString()))
                {
                    AddSoundMapping(e, name);
                    ++soundAdded;
                }
            }

            if (soundAdded == 0) 
            {
                DebugOutput.Instance.WriteInfo("No sounds added for " + e );
            }
        }

        private static readonly SoundEffectManager _instance = new SoundEffectManager();

        public static SoundEffectManager Instance
        {
            get { return _instance; }
        }

        static SoundEffectManager()
        {

        }

        int SOUNDS;

        SoundEffect[] _soundEffect;
        SoundEffectInstance[] _soundInstance;
        float[] _minPlayInterval;
        float[] _disabledTimeRemaining;

        List<string> _names = new List<string>();

        string filetype = "wav";

        string _contentDirectory = "sounds";


        private void ScanDirectoryForSoundEffectFiles()
        {
            string[] soundFiles = Directory.GetFiles(".\\Content\\" + _contentDirectory + "\\", "*."+filetype);

            int contentDirectoryLength = _contentDirectory.Length + 1;

            for (int i = 0; i < soundFiles.Length; ++i)
            {
                int indexDirectory = soundFiles[i].IndexOf(_contentDirectory, StringComparison.CurrentCultureIgnoreCase);

                string filename = soundFiles[i].Substring(indexDirectory + contentDirectoryLength);
                int filenameLength = filename.Length - 4;
                filename = filename.Substring(0, filenameLength);
                _names.Add(filename);
            }
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            ScanDirectoryForSoundEffectFiles();

            SOUNDS = _names.Count;
            _soundEffect = new SoundEffect[SOUNDS];
            _soundInstance = new SoundEffectInstance[SOUNDS];
            _minPlayInterval = new float[SOUNDS];
            _disabledTimeRemaining = new float[SOUNDS];

            string path = "";

            for (int i = 0; i < SOUNDS; ++i)
            {
                try
                {
                    path = "Content/" + _contentDirectory + "/" + _names[i] + "." + filetype;
                    _soundEffect[i] = SoundEffect.FromStream(new FileStream(path, FileMode.Open));

                    _soundInstance[i] = _soundEffect[i].CreateInstance();
                    _soundInstance[i].IsLooped = true;
                    _minPlayInterval[i] = GlobalConstants.Instance.DEFAULT_MIN_PLAY_INTERVAL_PER_SOUND;
                    _disabledTimeRemaining[i] = 0f;
                }
                catch(Exception e)
                {
                    DebugOutput.Instance.WriteError("Unable to load Sound File: " + path + " | " + e.Message);
                }
            }
            DebugOutput.Instance.WriteInfo("Loaded " + SOUNDS + " Audio Files");

            SetupSoundMapping();
        }

        public void SetMinPlayInterval(string name, float interval)
        {
            int soundID = SoundEffectID(name);
            if (soundID >= 0)
            {
                _minPlayInterval[soundID] = Math.Max(interval, 0f);
            }
        }

        public string SoundEffectName(int soundID)
        {
            if (soundID >= 0 && soundID < _names.Count)
            {
                return _names[soundID];
            }
            return "";
        }

        public int SoundEffectID(string name)
        {
            for (int i = 0; i < SOUNDS; ++i)
            {
                if (name == _names[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int SoundEffectCount()
        {
            return _names.Count;
        }

        public bool PlaySound(int soundID, float volume)
        {
            float masterSfxVolume = Math.Max(GlobalConstants.Instance.VOLUME_SFX, 0.0f);

            if (soundID < SOUNDS && _disabledTimeRemaining[soundID] <= 0)
            {
                float outputVolume = MathHelper.Clamp(volume * masterSfxVolume, 0.0f, 1.0f);
                _soundEffect[soundID].Play(outputVolume, 0, 0);

                DebugOutput.Instance.WriteError("Playing sound: " + soundID + " " + outputVolume);

                _disabledTimeRemaining[soundID] = _minPlayInterval[soundID];
                return true;
            }
            DebugOutput.Instance.WriteInfo("ERROR Playing sound: " + soundID);

            return false; // sound invalid or blocked by repeat interval
        }

        public bool PlaySound(string name, float volume)
        {
            for (int i = 0; i < SOUNDS; ++i)
            {
                if (name == _names[i])
                {
                    DebugOutput.Instance.WriteInfo("Playing sound: " + name + " at vol:" + volume);
                    return PlaySound(i, volume);
                }
            }

            DebugOutput.Instance.WriteInfo("Sound not found: " + name);

            return false;
        }

        public void StopSound(string name)
        {
            for (int i = 0; i < SOUNDS; ++i)
            {
                if (name == _names[i])
                {
                    _soundInstance[i].Stop();
                }
            }
        }

        public TimeSpan GetSoundDuration(int soundID)
        {
            return _soundEffect[soundID].Duration;
        }

        public TimeSpan GetSoundDuration(string name)
        {
            for (int i = 0; i < SOUNDS; ++i)
            {
                if (name == _names[i])
                {
                    return GetSoundDuration(i);
                }
            }
            return new TimeSpan(0);
        }

        public void PlayRandomSound()
        {
            string randomSound = _names.GetRandomElement();

            PlaySound(randomSound, 1.0f);
        }

        public void Update(GameTime gameTime)
        {
            // updated disabled play timers (for preventing constant repeats)
            for (int i = 0; i < SOUNDS; ++i)
            {
                if (_disabledTimeRemaining[i] > 0f)
                {
                    _disabledTimeRemaining[i] = Math.Max(_disabledTimeRemaining[i] - (float)gameTime.ElapsedGameTime.TotalSeconds, 0f);
                }
            }
        }
    }
}
