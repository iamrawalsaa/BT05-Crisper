//using Microsoft.Xna.Framework.Media;
//using MonoGame.Extended.VideoPlayback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MonoGame.Extended.VideoPlayback;
//using MonoGame.Extended.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using SharedMonoGame;
using System.Diagnostics;
using shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
//using Microsoft.Xna.Framework.Media;

namespace BT05
{
    public enum VideoKey
    {
        none,
        attract,
        success,
        failure
    }


    /// <summary>
    /// Do i need multiple Videoplayers to play multiple videos...
    /// </summary>

    public sealed class VideoManager
    {
        private static readonly VideoManager _instance = new VideoManager();

        public static VideoManager Instance
        {
            get { return _instance; }
        }

        Dictionary<VideoKey, JVideo> _videoDictionary = new Dictionary<VideoKey, JVideo>();
        Dictionary<VideoKey, string> _videoNames = new Dictionary<VideoKey, string>();
        Dictionary<VideoKey, int> _videoSpeed = new Dictionary<VideoKey, int>();

        VideoKey _currentKey = VideoKey.none;

//        VideoPlayer _videoPlayer = null;

        MyGameBase _gamebase = null;
        private bool _renderTexturesEnabled = true;

        public void ToggleRenderTextures()
        {
            _renderTexturesEnabled =!_renderTexturesEnabled;
        }

        void Initialise()
        {
            _videoNames.Add(VideoKey.attract, "attract");
            _videoNames.Add(VideoKey.success, "success");
            _videoNames.Add(VideoKey.failure, "failure");

            _videoSpeed.Add(VideoKey.attract, 3);
            _videoSpeed.Add(VideoKey.success, 6);
            _videoSpeed.Add(VideoKey.failure, 6);

            
        }

        public void LoadContent( MyGameBase mygame)
        {
            _gamebase = mygame;

            Initialise();
            //_videoPlayer = new VideoPlayer(mygame.GraphicsDevice);

            foreach (var key in _videoNames)
            {
                if (key.Key != VideoKey.none)
                {
                    JVideo video = new JVideo(key.Value, _gamebase.GraphicsDevice);
                    _videoDictionary.Add(key.Key, video);
                    video.RequestsPerAdvance = _videoSpeed[key.Key];

                    //string filename = @"Content/Videos/" + key.Value + ".ogv";

                    ////Video video = VideoHelper.LoadFromFile(filename);
                    //Video video = SharedAssetManager.Instance.GetVideo(key.Value);
                    //_videoDictionary.Add(key.Key, video);

                    //VideoPlayer player = new VideoPlayer();

                    ////player.IsLooped = true;
                    ////player.Play(video);
                    ////player.IsLooped = true;
                    ////player.Pause();
                    //_videoPlayers.Add(key.Key, player);
                }
            }
        }

        //public void StartAllVideos()
        //{
        //    foreach (var key in _videoNames)
        //    {
        //        if (key.Key != VideoKey.none)
        //        {
        //            var player = _videoPlayers[key.Key];
        //            player.Play(_videoDictionary[key.Key]);
        //            //player.IsLooped = true;
        //            //player.Pause();
        //        }
        //    }
        //}

        //        public void StopAllVideos()
        //        {
        //            foreach (var key in _videoNames)
        //            {
        //                if (key.Key != VideoKey.none)
        //                {
        //                    var player = _videoPlayers[key.Key];
        //                    if (player != null)
        //                    {
        //                        player.Stop();
        //                        player.Dispose();
        //                        _videoPlayers[key.Key] = null;
        //                    }
        ////                    player.Play(_videoDictionary[key.Key]);
        //                    //player.IsLooped = true;
        //                    //player.Pause();
        //                }
        //            }
        //        }

        public void Play(VideoKey videoID)
        {
            _currentKey = videoID;
        }

        public void Stop()
        {
            _currentKey = VideoKey.none;
        }

        //public void Play( VideoKey videoID, bool isLooped = false )
        //{
        //    Stop();

        //    if ( videoID != VideoKey.none)
        //    {
        //        var player = _videoPlayers[videoID];

        //        //var video = _videoDictionary[videoID];
        //        //player.Play(video);

        //        //player.Resume();
        //        _currentKey = videoID;
        //    }

        //    //if (_videoPlayer == null)
        //    //{
        //    //    _videoPlayer = new VideoPlayer(_gamebase.GraphicsDevice);
        //    //}

        //    //if (_videoPlayer != null )
        //    //{
        //    //    var videoFile = _videoDictionary[videoID];
        //    //    if ( videoFile != null )
        //    //    {
        //    //        _videoPlayer.IsLooped = isLooped;
        //    //        _videoPlayer.Play(videoFile);
        //    //        DebugOutput.Instance.WriteInfo("Playing video: " + videoID + " IsLooped: " + isLooped);
        //    //    }
        //    //}
        //}

        public Texture2D GetVideoTexture()
        {
            if (_currentKey!= VideoKey.none)
            {
                var video = _videoDictionary[_currentKey];
                video.NextFrame();
                return video.GetCurrentFrame();
            }

            return SharedAssetManager.Instance.Blank;

            //if (_currentKey != VideoKey.none && _renderTexturesEnabled)
            //{
            //    var player = _videoPlayers[_currentKey];

            //    if (player == null)
            //    {
            //        player = new VideoPlayer();
            //        _videoPlayers[_currentKey] = player;
                    
            //    }
            //    else
            //    {

            //        if (player.State != Microsoft.Xna.Framework.Media.MediaState.Playing)
            //        {
            //            //var player = _videoPlayers[key.Key];
            //            player.Play(_videoDictionary[_currentKey]);
            //        }

            //        if (player.State == Microsoft.Xna.Framework.Media.MediaState.Playing)
            //        {
            //            Texture2D videoTexture = null;
            //            try
            //            {
            //                videoTexture = player.GetTexture();
            //                DebugOutput.Instance.AppendToLiveString(player.PlayPosition.ToString());
            //                DebugOutput.Instance.AppendToLiveString(player.State.ToString());
            //            }
            //            catch (Exception e)
            //            {
            //                DebugOutput.Instance.WriteError("Issue with Video Texture: " + _currentKey);
            //            }

            //            return videoTexture;
            //        }
            //        else
            //        {

            //        }
            //    }
            //}
            //return null;

            //    if (_videoPlayer != null)
            //{
            //    if (_videoPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing)
            //    {
            //        var videoTexture = _videoPlayer.GetTexture();
            //        return videoTexture;
            //    }
            //}
            //return null;
        }



        //public void Stop()
        //{
        //    if (_currentKey != VideoKey.none)
        //    {
        //        //_videoPlayers[_currentKey].Pause();
        //        _currentKey = VideoKey.none;
        //    }
        //    StopAllVideos();

        //    //if (_videoPlayer != null)
        //    //{
        //    //    if (_videoPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Playing || _videoPlayer.State == Microsoft.Xna.Framework.Media.MediaState.Paused)
        //    //    {
        //    //        _videoPlayer.Stop();
        //    //        DebugOutput.Instance.WriteInfo("Stopping video: ");
        //    //    }
        //    //    else
        //    //    {
        //    //        DebugOutput.Instance.WriteInfo("Stopping video but video is already stopped ");
        //    //    }

        //    //    _videoPlayer.Dispose();
        //    //    _videoPlayer = null;
        //    //}
        //}

        /// <summary>
        /// TODO: probably need to do somethin cleverer here to trasition between videos neater
        /// </summary>
        /// <param name="attract"></param>
        //public void Stop(VideoKey attract)
        //{
        //    Stop();
        //}
    }
}
