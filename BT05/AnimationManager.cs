using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    public enum AnimationType
    {
        ONCE,
        LOOPING
    }

    public class Animation
    {
        //AnimationType _type = AnimationType.LOOPING;

        string _name = "none";
        int _currentFrame = 0;
        float _timeUntilNextFrame = 0;
        float _timeBetweenFrames = 1;
        Texture2D _texture = null;

        AnimationType _animationType = AnimationType.ONCE;

        List<Rectangle> _frames = new List<Rectangle>();

        public Animation(string name, Texture2D texture)
        {
            _name = name;
            _texture = texture;
        }

        public float TimeBetweenFrames
        {
            get { return _timeBetweenFrames; }
            set { _timeBetweenFrames = value; }
        }

        public void SetAnimationLength(float seconds)
        {
            if (_frames.Count > 0)
            {
                TimeBetweenFrames = seconds / _frames.Count;
            }
            else
            {
                TimeBetweenFrames = 1;
            }
        }

        public AnimationType AnimationType { get { return _animationType; } set { _animationType = value; }

        public void Update( float elapsedTime )
        {
            _timeUntilNextFrame -= elapsedTime;
            if (_timeUntilNextFrame < 0)
            {
                _timeUntilNextFrame = _timeBetweenFrames;

                //if (_name == "circlefill")
                {

                    ++_currentFrame;
                    if (_currentFrame >= _frames.Count)
                    {
                        // TODO: Animation - handle looping, should be freeze at the end?
                        if (_animationType == AnimationType.LOOPING)
                        {
                            _currentFrame = 0;
                        }
                        else
                        {
                            _currentFrame = _frames.Count - 1;
                        }
                    }
                }
            }
        }

        public Rectangle GetCurrentFrameRect() {
            return _frames[_currentFrame];
        }

        public Texture2D Texture { get { return _texture; } }

        public void AddFrame(Rectangle rect)
        {
            _frames.Add(rect);
        }

        public void Reset()
        {
            _currentFrame = 0;
            _timeUntilNextFrame = _timeBetweenFrames;
        }
    }

    public sealed class AnimationManager
    {
        Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

        private static readonly AnimationManager _instance = new AnimationManager();

        public static AnimationManager Instance
        {
            get { return _instance; }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var animation in _animations.Values)
            {
                animation.Update(gameTime.GetElapsedSeconds());
            }
        }

        public void AddAnimation(string animName, Texture2D texture, int frames, int width, int height, int columns, int paddingX = 0, int paddingY = 0)
        {
            Animation _animation = new Animation(animName, texture);

            int x = paddingX;
            int y = paddingY;
            int column = 0;

            for(int i = 0;i< frames; ++i)
            {
                Rectangle rect = new Rectangle(x, y, width, height);
                _animation.AddFrame(rect);

                x += width + paddingX;
                ++column;

                if (column == columns)
                {
                    column = 0;
                    x = paddingX;
                    y += height+paddingY;
                }
            }

            _animations.Add(animName, _animation);
        }

        public Animation GetAnimation(string animName)
        {
            if (_animations.ContainsKey(animName))
            {
                return _animations[animName];
            }
            else
            {
                return null;
            }
        }
    }
}