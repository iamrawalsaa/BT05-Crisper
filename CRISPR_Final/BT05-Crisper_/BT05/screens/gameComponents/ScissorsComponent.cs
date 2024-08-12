using Microsoft.VisualBasic.Devices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using SharedMonoGame;
//using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BT05
{
    public enum ScissorState
    {
        Open,
        Close
    }

    public sealed class Ref<T> where T : struct
    {
        public Ref(T value) => Value = value;

        public T Value { get; set; }
        public static implicit operator T(Ref<T> value) => value.Value;
    }

    public class ScissorsComponent : ExtendedDrawableGameComponent
    {
        readonly Vector2 SCISSORS_START = new Vector2(1140, 600);

        public bool ScissorsAvailable { get; set; } = false;

        public delegate void CutCompleteDelegate();
        public delegate void ScissorAnimationComplete();

        CutCompleteDelegate _cutCompleteDelegate = null;
        ScissorAnimationComplete _animationCompleteDelegate = null;

        /// <summary>
        /// Sets the delegate for the 3x cut has occurred
        /// and when the scissor animation completes
        /// </summary>
        /// <param name="CCdelegate"></param>
        /// <param name="ACdelegate"></param>
        public void SetCompleteDelegates(CutCompleteDelegate CCdelegate, ScissorAnimationComplete ACdelegate)
        {
            _cutCompleteDelegate = CCdelegate;
            _animationCompleteDelegate = ACdelegate;
        }

        Tweener _scissorTweener = new Tweener();

        private MyGameBase _game;
        ScissorState _previousScissorState = ScissorState.Open;

        public ScissorsComponent(MyGameBase game) : base(game)
        {
            _game = game;
        }

        Texture2D _open, _closed, _oneSideErrorMsg, _scissorsNotAvailable;

        public void SetTextures(Texture2D open, Texture2D closed, Texture2D oneSideErrorMsg, Texture2D scissorsNotAvailable)
        {
            _scissorsRect = new Rectangle(720, 500, 200, 300);
            //_spriteBatch = new SpriteBatch(GraphicsDevice);

            _oneSideErrorMsg = oneSideErrorMsg;
            _open = open;
            _closed = closed;
            _scissorsNotAvailable = scissorsNotAvailable;
        }

        bool _scissorsClosed = false;
        private bool _showOneSideError = false;
        private Rectangle _scissorsRect;

        int _scissorCount = 0;
        float _scissorTimer = 0;
        //Vector2 _scissorPosition = new Vector2(1140, 600);

        Ref<Vector2> _scissorRef = null;
        private bool _scissorAnimationPlaying = false;
        private float _scissorSwitchTime;
        private bool _scissorsAnimationClosed;

        public bool PlayScissorAnimationOnCompletion { get; set; } = true;

        public Vector2 ScissorCountPosition { get; set; } = new Vector2(1350, 750);

        public override void Update(GameTime gameTime)
        {
            _scissorTimer -= gameTime.GetElapsedSeconds();

            if (_scissorTimer < 0)
            {
                _scissorCount = 3;
                _scissorTimer = 3;
            }

            _scissorTweener.Update(gameTime.GetElapsedSeconds());

            UpdateAnimation(gameTime.GetElapsedSeconds());

            base.Update(gameTime);
        }

        private void UpdateAnimation(float elapsedSeconds)
        {
            if (_scissorAnimationPlaying)
            {
                _scissorSwitchTime -= elapsedSeconds;
                if (_scissorSwitchTime < 0)
                {
                    _scissorSwitchTime = 0.5f;
                    _scissorsAnimationClosed = !_scissorsAnimationClosed;
                }
            }
        }

        public void Init()
        {
            CreateScissorRef();

            //    _scissorTweener = new Tweener();
            //            _scissorTweener.TweenTo(this, _scissorPosition, new Vector2(1140, 600), 0, 0);
        }

        private void CreateScissorRef()
        {
            _scissorRef = new Ref<Vector2>(SCISSORS_START);
        }

        public void AnimateScissors()
        {
            if (_scissorRef == null)
            {
                CreateScissorRef();
            }

            Vector2 scissorAnimationTarget = new Vector2(1140,-500);

            _scissorTweener.TweenTo(_scissorRef, a=> a.Value, scissorAnimationTarget, 5, 0.5f).Easing(EasingFunctions.SineInOut).OnEnd((tween) => ScissorAnimationEnded() );
            _scissorAnimationPlaying = true;
        }

        private void ScissorAnimationEnded()
        {
            if (_animationCompleteDelegate != null)
            {
                _animationCompleteDelegate();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (!Visible) return;

            if (_scissorRef == null)
            {
                CreateScissorRef();
            }

            //_scissorsRect = new Rectangle((int)_scissorPosition.X, (int)_scissorPosition.Y, 300, 400);
            _scissorsRect = new Rectangle((int)_scissorRef.Value.X, (int)_scissorRef.Value.Y, 300, 400);


            //_spriteBatch.Begin();

            if (_scissorAnimationPlaying)
            {
                if (_scissorsAnimationClosed)
                {
                    _game._spriteBatch.Draw(_closed, _scissorsRect, Color.White);
                }
                else
                {
                    _game._spriteBatch.Draw(_open, _scissorsRect, Color.White);
                }
            }
            else
            {
                var showColor = Color.White;

                if (!ScissorsAvailable)
                {
                    showColor.A = 64;
                    showColor.R = 64;
                    showColor.G = 64;
                    showColor.B = 64;
                }

                if (_scissorsClosed)
                {
                    _game._spriteBatch.Draw(_closed, _scissorsRect, showColor);
                }
                else
                {
                    _game._spriteBatch.Draw(_open, _scissorsRect, showColor);
                }

                if (ScissorsAvailable)
                {
                    if (_showOneSideError)
                    {
                        _game._spriteBatch.Draw(_oneSideErrorMsg, _scissorsRect, Color.White);
                    }
                }
                else
                {
                    _game._spriteBatch.Draw(_scissorsNotAvailable, _scissorsRect, Color.White);
                }

                if (ScissorsAvailable)
                {
                    _game._spriteBatch.DrawString(SharedAssetManager.Instance.FontRegular, "x" + _scissorCount, ScissorCountPosition, Color.White);
                }
            }

            //_spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ScissorsClosed()
        {
            _scissorsClosed = true;
            _showOneSideError = false;

            if (ScissorsAvailable)
            {

                if (_previousScissorState == ScissorState.Open)
                {
                    if (_scissorCount > 0)
                    {
                        --_scissorCount;

                        CheckNotifyParent();
                    }
                    _scissorTimer = 3;
                }
            }

            _previousScissorState = ScissorState.Close;
        }

        private void CheckNotifyParent()
        {
            if (_scissorCount == 0)
            {
                if (_cutCompleteDelegate != null)
                {
                    _cutCompleteDelegate();
                    if (PlayScissorAnimationOnCompletion)
                    {
                        AnimateScissors();
                    }
                }
            }
        }

        public void ScissorsOpen()
        {
            _scissorsClosed = false;
            _showOneSideError = false;

            _previousScissorState = ScissorState.Open;
        }

        public void OneSideClosed()
        {
            _showOneSideError = true;
        }


        public override void RecreateTextures()
        {

        }

        public void ResetAfterAnimation()
        {
            Visible = true;
            _scissorAnimationPlaying = false;
            _scissorRef.Value = SCISSORS_START;
        }

        public void SetPosition(Vector2 pos)
        {
            if (_scissorRef == null)
            {
                _scissorRef = new Ref<Vector2>(new Vector2());
            }

            _scissorRef.Value = new Vector2(pos.X, pos.Y);
        }

        //public void SetCountPosition(Vector2 pos)
        //{

        //}
    }
}