using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using shared;

namespace SharedMonoGame
{
    public enum MusicEnum
    {
        ATTRACT,
        INTRO,
        GAME
    }

    /// <summary>
    /// Responsible for playing background music.
    /// </summary>
    public sealed class MusicManager
    {
        enum FadeType
        {
            fadeIn,
            playing,
            fadeOut
        }

        private static readonly MusicManager _instance = new MusicManager();

        public static MusicManager Instance
        {
            get { return _instance; }
        }

        List<string> _names = new List<string>();
        List<Song> _loadedSongs = new List<Song>();

        string contentDirectory = "music";
        string filetype = "mp3";
        Song _playingSong;
        Song _nextSong;

        float _volume = 0.0f;
        float _targetVolume = 0.5f;

        FadeType _fadeType = FadeType.fadeIn;

        private void ScanDirectoryForSongs()
        {
            string[] songs = Directory.GetFiles(".\\Content\\" + contentDirectory + "\\", "*." + filetype);

            int contentDirectoryLength = contentDirectory.Length + 1;

            for (int i = 0; i < songs.Length; ++i)
            {
                int indexDirectory = songs[i].IndexOf(contentDirectory);

                string filename = songs[i].Substring(indexDirectory + contentDirectoryLength);
                int filenameLength = filename.Length - 4;
                filename = filename.Substring(0, filenameLength);
                _names.Add(filename);
            }
        }

        string GetPlayingSongName()
        {
            if (_playingSong != null)
            {
                if (!_playingSong.IsDisposed)
                {
                    string songName = _playingSong.Name;
                    if (songName.Contains('/') || songName.Contains('\\'))
                        {
                        int index = contentDirectory.Length + 1;
                        string simpleSongName = songName.Substring(index);
                        return simpleSongName;
                    }
                    return songName;
                }
            }
            return "";
        }

        public int SongID(string name)
        {
            for (int i = 0; i < _names.Count; ++i)
            {
                if (name == _names[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public string GetRandomSongName()
        {
            return _names.GetRandomElement();
        }

        bool SongExists(string songName)
        {
            return (SongID(songName) != -1);
        }

        Song GetSong(string songName)
        {
            int id = SongID(songName);
            if (id != -1)
            {
                return _loadedSongs[id];
            }
            return null;
        }

        public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            if (GlobalConstants.Instance.SONGS_ENABLED)
            {
                ScanDirectoryForSongs();
                foreach (string name in _names)
                {
                    string fullPath = content.RootDirectory +"/" +contentDirectory + "/" + name + "." + filetype;
                    Uri uri = new Uri(fullPath, UriKind.Relative);
                    Song loadSong= Song.FromUri(name, uri);
//                    Song loadSong = content.Load<Song>(fullPath);
                    _loadedSongs.Add(loadSong);
                }
            }
        }

        public void PlaySong( MusicEnum song )
        {
            PlaySong(song.ToString());
        }

        public void PlaySong(string songName)
        {
            // this is viewable to the outside world
            // it queues up the next track...
            //   begins the fade out and then fade in of the new track
            if (GlobalConstants.Instance.SONGS_ENABLED)
            {
                if (_playingSong == null || GetPlayingSongName() != songName)
                {
                    if (SongExists(songName))
                    {
                        if (_playingSong == null)
                        {
                            _playingSong = GetSong(songName);
                            _fadeType = FadeType.fadeIn;
                            PlaySongImmediate(_playingSong);
                        }
                        else
                        {
                            _nextSong = GetSong(songName);
                            _fadeType = FadeType.fadeOut;
                        }
                    }
                    else
                    {
                        DebugOutput.Instance.WriteError("SongManager - unable to find song: " +songName );
                    }
                }
            }
        }

        void PlaySongImmediate(Song song)
        {
            if (GlobalConstants.Instance.SONGS_ENABLED)
            {
                _playingSong = song;
                MediaPlayer.Play(_playingSong);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void Update(GameTime gameTime)
        {
            float masterMusicVolume = MathHelper.Clamp(GlobalConstants.Instance.VOLUME_MUSIC, 0.0f, 1.0f);
            _targetVolume = masterMusicVolume; // track the live game constant

            float currentVolume = MediaPlayer.Volume;
            // this is to fade out and fade in different tracks
            switch (_fadeType)
            {
                case FadeType.fadeIn:
                    _volume += (float)gameTime.ElapsedGameTime.TotalSeconds * _targetVolume;
                    if (_volume >= _targetVolume)
                    {
                        _volume = _targetVolume;
                        _fadeType = FadeType.playing;
                    }
                    MediaPlayer.Volume = _volume;
                    break;
                case FadeType.playing:
                    if (Math.Abs(currentVolume - _targetVolume) > 0.001f)
                    {
                        MediaPlayer.Volume = _targetVolume;
                        _volume = _targetVolume;
                    }
                    break;
                case FadeType.fadeOut:
                    _volume -= (float)gameTime.ElapsedGameTime.TotalSeconds * _targetVolume;
                    if (_volume <= 0.0f)
                    {
                        _volume = 0.0f;
                        _fadeType = FadeType.fadeIn;
                        PlaySongImmediate(_nextSong);
                        _nextSong = null;
                    }
                    MediaPlayer.Volume = _volume;

                    break;
                default:
                    break;
            }
        }

        public void PlayRandomSong()
        {
            string randomName = GetRandomSongName();
            PlaySong(randomName);
        }

        public string SongName(int songID)
        {
            if (songID >= 0 && songID < _names.Count)
            {
                return _names[songID];
            }
            return "";
        }

        public int SongCount()
        {
            return _names.Count;
        }
    }
}