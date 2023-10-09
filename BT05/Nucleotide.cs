using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MonoGame.SplineFlower.Spline;
using MonoGame.SplineFlower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MonoGame.SplineFlower.SplineWalker;
using SharpDX.XAudio2;
using shared;

namespace BT05
{
    public enum RotationTypeEnum
    {
        Normal,
        FixedLeft,
        FixedRight
    }

    public class Nucleotide : SplineWalker
    {
        public RotationTypeEnum RotationType { get; set; } = RotationTypeEnum.Normal;

        public bool ShowTexture { get; set; } = true;

        public bool Stop { get; set; }

        private Texture2D _texture;
        private Texture2D _alternativeTexture;
        //    private SoundEffect _Horn, _HandBrake, _CarDrive;


        bool _isLeft = false;
        public bool IsLeft { get { return _isLeft; } set { _isLeft = value; } }

        Vector2 _offset = new Vector2(0, 0);

        public Vector2 Offset { get { return _offset; } set { _offset = value; } }

        int _sequenceIndex = -1;
        public int SequenceIndex { get { return _sequenceIndex; } set { _sequenceIndex = value; } }

        NucleotideEnum _nucleotideEnum = NucleotideEnum.None;
        public NucleotideEnum NucleotideEnum { get { return _nucleotideEnum; } }

        public void LoadContent(Texture2D tex, Texture2D alternativeTexture, shared.NucleotideEnum nucleotideEnum)
        {
            _texture = tex;
            _alternativeTexture = alternativeTexture;
            _nucleotideEnum = nucleotideEnum;

  //          Font = font;

//            _texture = Content.Load<Texture2D>(@"car");

//            _Horn = Content.Load<SoundEffect>(@"Audio/horn");
//            _HandBrake = Content.Load<SoundEffect>(@"Audio/handbrake");
//            _CarDrive = Content.Load<SoundEffect>(@"Audio/cardrive");
        }

        public void ReplaceSpline( SplineBase splineBase)
        {
            _Spline = splineBase;
        }

        public override void CreateSplineWalker(SplineBase spline, SplineWalkerMode mode, int duration, bool canTriggerEvents = true, SplineWalkerTriggerDirection triggerDirection = SplineWalkerTriggerDirection.Forward, bool autoStart = true)
        {
            base.CreateSplineWalker(spline, mode, duration, canTriggerEvents, triggerDirection, autoStart);
        }

        protected override void EventTriggered(Trigger obj)
        {
            // Calling the base action afterwards. 
            // Otherwise the EventTrigger action here won't work or would be called mutliple times
            // if there is only one TriggerEvent on the Spline.
            base.EventTriggered(obj);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Stop) base.Update(gameTime);
        }

        bool _displayAltTexture = false;

        public bool DisplayAltTexture
        {
            get { return _displayAltTexture; }
            set { _displayAltTexture = value; }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (ShowTexture)
            {
                float rotation = 0.0f;

                switch (RotationType)
                {
                    case RotationTypeEnum.Normal:
                        rotation = Rotation;
                        break;
                    case RotationTypeEnum.FixedLeft:
                        rotation = (float)(Math.PI / 2.0);
                        break;
                    case RotationTypeEnum.FixedRight:
                        rotation = (float)(Math.PI * 3.0 / 2.0);
                        break;
                    default:
                        break;
                }


                bool showGhost = false;

                if (showGhost)
                {
                    Color alpha = new Color(Color.White, 0.5f);

                    {
                        spriteBatch.Draw(_texture,
                                         Position,
                                         null,
                                         alpha,
                                         rotation,
                                         new Vector2(_texture.Width / 2, _texture.Height / 2),
                                         0.1f,
                                         SpriteEffects.None,
                                         0f);
                    }
                }

                // Need a new drawing system
                // it needs to stretch to the output
                // the only way to do this is with a target Rect...
                // or switching to Monogame Sprite..

                int extra = 12;

                int length = Math.Abs( 260 - (int)Position.Y) + extra;

                // There's complexity here around figuring out if you're a above / below the line
                Rectangle targetRect = new Rectangle((int)(Position.X + _offset.X), (int)(Position.Y + _offset.Y), length, 35);

                if (Position.Y < 260)
                {
                    targetRect.Y += length/2;
                }
                else
                {
                    targetRect.Y -= length / 2;

                    rotation += (float)Math.PI;
                }

                // extra code to switch the left ot outside
                if (_isLeft)
                {
                    // need to switch it
                    if (Position.X>566 && Position.X<1533)
                    {
                        rotation += (float)Math.PI;
                        targetRect.Y += length;
                    }
                }

                //if (GetProgress >= _minProgess && GetProgress <= _maxProgress)

                if (_displayAltTexture)
                {
                    spriteBatch.Draw(_alternativeTexture, targetRect, null, Color.White, rotation, new Vector2(_texture.Width / 2, _texture.Height / 2), SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(_texture, targetRect, null, Color.White, rotation, new Vector2(_texture.Width / 2, _texture.Height / 2), SpriteEffects.None, 0f);
                }

            }

            foreach (Trigger trigger in GetTriggers("Counter"))
            {
            //    if (trigger.Custom != null) spriteBatch.DrawString(Font, trigger.Custom.ToString(), GetPositionOnCurve(trigger.Progress), Color.White);
            }
        }

    }
}
