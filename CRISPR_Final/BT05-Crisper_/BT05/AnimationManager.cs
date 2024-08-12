using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    /// <summary>
    /// This is used to hold an animation and it's onscreen location / rotation
    /// Potentially extend this to transition on
    /// Using tweening
    /// </summary>
    public class OnScreenAnimation
    {
        string _animName;
        Animation _animation;
        Rectangle _screenRect;
        float _rotation = 0;
        MyGameBase _myGame;
        bool _show = true;

        public OnScreenAnimation(MyGameBase myGame, string animName, Rectangle rect, float rotation = 0, float length = 5, AnimationType animType = AnimationType.LOOPING)
        {
            _myGame = myGame;
            _animName = animName;
            _screenRect = rect;
            _rotation = MathHelper.ToRadians(rotation);

            _animation = AnimationManager.Instance.GetAnimation(animName);

            if (_animation != null)
            {
                _animation.SetAnimationLength(length);
                _animation.Reset();
                _animation.AnimationType = animType;
            }
        }

        public void DrawSpriteSheetAnim()
        {
            if (_animation != null && _show)
            {
                Rectangle source = _animation.GetCurrentFrameRect();
                Vector2 origin = new Vector2(source.Width / 2, source.Height / 2);

                _myGame._spriteBatch.Draw(_animation.Texture, _screenRect, source, Microsoft.Xna.Framework.Color.White, _rotation, origin, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
        }

        public void Reset()
        {
            if ( _animation != null )
            {
                _animation.Reset();
            }
        }

        public void Pause()
        {
            if (_animation != null)
            {
                _animation.Pause();
            }
        }

        public void Play()
        {
            if (_animation != null)
            {
                _animation.Play();
            }
        }

        public void Show()
        {
            _show = true;
        }

        public void Hide()
        {
            _show = false;
        }

    }

    public enum AnimationType
    {
        ONCE,
        LOOPING
    }

    public class Animation
    {
        //AnimationType _type = AnimationType.LOOPING;
        bool _isRunning = true;
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

        public void Pause()
        {
            _isRunning = false;
        }

        public void Play()
        {
            _isRunning=true;
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

        public AnimationType AnimationType { get { return _animationType; } set { _animationType = value; } }

        public void Update( float elapsedTime )
        {
            if (_isRunning)
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