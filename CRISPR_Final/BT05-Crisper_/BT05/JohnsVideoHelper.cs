using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BT05
{

    public class JVideoFrame
    {
        public Texture2D Texture2D = null;
        public bool IsReady = false;
        public bool IsLoading = false;
        public byte[] JpgData = null;

        public void RequestFrame( GraphicsDevice graphicsDevice )
        {
            if (this.Texture2D == null)
            {
                MemoryStream stream = new MemoryStream(JpgData);
                this.Texture2D = Texture2D.FromStream(  graphicsDevice, stream);

                IsReady = true;
                JpgData = null;
            }
        }
    }



    public class JVideo
    {
        // list or dictionary here... I guess it doesn't matter loads

        public List<JVideoFrame> Frames = new List<JVideoFrame>();
        int _currentFrame = 0;

        int CurrentFrame
        {
            get
            {
                return _currentFrame;
            }

            set
            {
                _currentFrame = value;
                RequestFrame();
            }
        }

        float _closestFrame = 0;
        public Texture2D GetCurrentFrame()
        {
            var frame = Frames[CurrentFrame];

            if (frame != null)
            {
                if (frame.IsReady)
                {
                    return frame.Texture2D;
                }
                else
                {
                    return SharedAssetManager.Instance.Missing;
                }
            }

            return null;
        }

        public void FrameChange(float delta)
        {
            _closestFrame += delta;

            if (_closestFrame < 0) { _closestFrame += Frames.Count; }
            if (_closestFrame > Frames.Count) { _closestFrame -= Frames.Count; }

            CurrentFrame = (int)_closestFrame;
        }

        int _requests = 0;
        int REQUESTS_PER_ADVANCE = 6;

        public int RequestsPerAdvance
        {
            set { REQUESTS_PER_ADVANCE = value; }
        }

        public void NextFrame()
        {
            ++_requests;
            if (_requests >= REQUESTS_PER_ADVANCE)
            {
                _requests = 0;
                ++CurrentFrame;
                if (CurrentFrame == Frames.Count)
                {
                    CurrentFrame = 0;
                }
                RequestFrame(CurrentFrame + 1);
            }
        }

        public void PrevFrame()
        {
            --CurrentFrame;
            if (CurrentFrame < 0)
            {
                CurrentFrame = Frames.Count - 1;
            }
            RequestFrame(CurrentFrame - 1);
        }

        public int TotalFrames
        {
            get
            {
                return Frames.Count;
            }
        }

        public int TotalDecodedFrames
        {
            get
            {
                int total = 0;
                foreach (var frame in Frames)
                {
                    if (frame.IsReady) ++total;
                }
                return total;
            }
        }

        void RequestFrame()
        {
            RequestFrame(CurrentFrame);
            //            JohnsVideoHelper.Instance.RequestFrame( CurrentFrame );
        }

        void RequestFrame(int frameID)
        {
            //if (frameID >= 0 && frameID < Frames.Count)
            //{
            //    JohnsVideoHelper.Instance.RequestFrame(frameID);
            //}
        }

        public void DecodeFrame(int decodeFrameID)
        {
            Frames[decodeFrameID].RequestFrame(_graphicsDevice);
        }

        GraphicsDevice _graphicsDevice;

        public JVideo(string folderPath, GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;

            foreach (var file in Directory.EnumerateFiles(@"content\videos\"+folderPath+@"\"))
            {
                JVideoFrame frame = new JVideoFrame();
                frame.JpgData = File.ReadAllBytes(file);
                frame.RequestFrame(graphicsDevice);
                Frames.Add(frame);
                
            }
        }
    }


    //public sealed class JohnsVideoHelper
    //{


    //    private static readonly JohnsVideoHelper _instance = new JohnsVideoHelper();

    //    public static JohnsVideoHelper Instance
    //    {
    //        get { return _instance; }
    //    }

    //    public JVideo CurrentVideo = new JVideo();

    //    public void CreateNewVideo(string folderPath)
    //    {
    //        // can I load a jpeg file now and then process it later?
    //        // convert a stream into bytes and then back again

    //        foreach (var file in Directory.EnumerateFiles(folderPath))
    //        {
    //            JVideoFrame frame = new JVideoFrame();
    //            frame.JpgData = File.ReadAllBytes(file);

    //            CurrentVideo.Frames.Add(frame);
    //        }
    //    }

    //    object _frameListLock = new object();
    //    List<int> _frameList = new List<int>();

    //    public void RequestFrame(int frameID)
    //    {
    //        // add to frameList
    //        lock (_frameListLock)
    //        {
    //            if (!_frameList.Contains(frameID))
    //            {
    //                _frameList.Add(frameID);
    //            }
    //        }
    //    }

    //    int NextFrameToLoad()
    //    {
    //        int frameID = -1;
    //        lock (_frameListLock)
    //        {
    //            if (_frameList.Count > 0)
    //            {
    //                frameID = _frameList[0];
    //                _frameList.RemoveAt(0);
    //            }
    //        }
    //        return frameID;
    //    }
    //}
}
