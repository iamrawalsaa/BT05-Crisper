using Microsoft.Xna.Framework;
using MonoGame.SplineFlower.Content;
using MonoGame.SplineFlower.Spline.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    /// <summary>
    /// These are all the editor bits
    /// </summary>
    public partial class ScreenGame
    {



        public override void MouseMiddleClicked(int x, int y)
        {
            FindClosestSplineTangent(new Vector2(x, y));
        }

        public void FindClosestSplineTangent(Vector2 target)
        {
            int closestID = -1;
            float closestDist = 9999;

            for (int i = 0; i < _lefttand.Length; ++i)
            {
                var p = _lefttand[i];
                var dist = (p.Position - target).Length();
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestID = i;
                }
            }

            _closestSplineTangent = closestID;
            _closestSplinePoint = -1;
        }

        public void ToggleDrawLeft()
        {
            _drawLeft = !_drawLeft;
        }

        public void ToggleDrawRight()
        {
            _drawRight = !_drawRight;
        }

        float _splineEditorMovement = 10;

        public void Up(float ratio)
        {
            if (_closestSplinePoint != -1)
            {
                var closest = _leftpd[_closestSplinePoint];
                closest.Position = new Vector2(closest.Position.X, closest.Position.Y - _splineEditorMovement * ratio);
            }

            if (_closestSplineTangent != -1)
            {
                var closest = _lefttand[_closestSplineTangent];
                closest.Position = new Vector2(closest.Position.X, closest.Position.Y - _splineEditorMovement * ratio);
            }
            RegenerateSplines();
        }

        public void Down(float ratio)
        {
            if (_closestSplinePoint != -1)
            {
                var closest = _leftpd[_closestSplinePoint];
                closest.Position = new Vector2(closest.Position.X, closest.Position.Y + _splineEditorMovement * ratio);
            }

            if (_closestSplineTangent != -1)
            {
                var closest = _lefttand[_closestSplineTangent];
                closest.Position = new Vector2(closest.Position.X, closest.Position.Y + _splineEditorMovement * ratio);
            }

            RegenerateSplines();
        }

        public void Left(float ratio)
        {
            if (_closestSplinePoint != -1)
            {
                var closest = _leftpd[_closestSplinePoint];
                closest.Position = new Vector2(closest.Position.X - _splineEditorMovement * ratio, closest.Position.Y);
            }

            if (_closestSplineTangent != -1)
            {
                var closest = _lefttand[_closestSplineTangent];
                closest.Position = new Vector2(closest.Position.X - _splineEditorMovement * ratio, closest.Position.Y);
            }
            RegenerateSplines();
        }

        public void Right(float ratio)
        {
            if (_closestSplinePoint != -1)
            {
                var closest = _leftpd[_closestSplinePoint];
                closest.Position = new Vector2(closest.Position.X + _splineEditorMovement * ratio, closest.Position.Y);
            }

            if (_closestSplineTangent != -1)
            {
                var closest = _lefttand[_closestSplineTangent];
                closest.Position = new Vector2(closest.Position.X + _splineEditorMovement * ratio, closest.Position.Y);
            }

            RegenerateSplines();
        }

        void ReplaceSplineForNucleotides()
        {
            ReplaceSplineForLeftNucleotides();
            ReplaceSplineForRightNucleotides();
        }

        void ReplaceSplineForLeftNucleotides()
        {
            foreach (var nucleo in _nucleotideWalkersLeft)
            {
                nucleo.ReplaceSpline(_helixSplineLeft);
            }
        }

        void ReplaceSplineForRightNucleotides()
        {
            foreach (var nucleo in _nucleotideWalkersRight)
            {
                nucleo.ReplaceSpline(_helixSplineRight);
            }
        }

        //
        public void ShiftEntireX(float xDiff)
        {
            foreach (var p in _leftpd)
            {
                p.Position = new Vector2(p.Position.X + xDiff, p.Position.Y);
            }

            foreach (var t in _lefttand)
            {
                t.Position = new Vector2(t.Position.X + xDiff, t.Position.Y);
            }

            RegenerateSplines();
        }

        public override void MouseRightClicked(int x, int y)
        {
            FindClosestSplinePoint(new Vector2(x, y));
        }

        public void FindClosestSplinePoint(Vector2 target)
        {
            int closestID = -1;
            float closestDist = 9999;

            for (int i = 0; i < _leftpd.Length; ++i)
            {
                var p = _leftpd[i];
                var dist = (p.Position - target).Length();
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestID = i;
                }
            }

            _closestSplinePoint = closestID;
            _closestSplineTangent = -1;
        }

        public void ToggleNucleotideDrawing()
        {
            _drawNucleotides = !_drawNucleotides;
        }


        /// <summary>
        /// Take all the X positions on the _leftPD and spread evenly
        /// </summary>
        public void SpreadHorizontally()
        {
            float minX = 1920 / 2;
            float maxX = 1920 / 2;

            foreach (var splinePoint in _leftpd)
            {
                if (splinePoint.Position.X < minX) minX = splinePoint.Position.X;
                if (splinePoint.Position.X > maxX) maxX = splinePoint.Position.X;
            }

            float diff = (maxX - minX) / (_leftpd.Length - 1);

            float startX = minX;
            foreach (var splinePoint in _leftpd)
            {
                splinePoint.Position = new Vector2(startX, splinePoint.Position.Y);
                startX += diff;
            }

            float minX2 = 1920 / 2;
            float maxX2 = 1920 / 2;

            foreach (var splinePoint in _leftpd)
            {
                if (splinePoint.Position.X < minX2) minX2 = splinePoint.Position.X;
                if (splinePoint.Position.X > maxX2) maxX2 = splinePoint.Position.X;
            }

            RegenerateSplines();
        }

        void RegenerateSplines()
        {
            CreateLeftSpline();
            CreateRightSpline();
            //ReplaceSplineForNucleotides();

            AddNucleotideWalkersFromLongerSection(_singleNucleotideSection, 0.02f);
        }

        /// <summary>
        /// This is a flipped version from the left data
        /// </summary>
        private void CreateRightSpline()
        {
            FlipSpline();

            _helixSplineRight = new HermiteSpline();
            _helixSplineRight.LoadJsonSplineData(_rightpd, _rightpmd, _righttd, _righttand);

            FlipSpline();
        }

        private void FlipSpline()
        {
            // need to duplicate array

            //_rightpd = new TransformDummy[_leftpd.Length];
            //_righttand = new TransformDummy[_lefttand.Length];

            //for(int i=0; i< _leftpd.Length; ++i)
            //{
            //    _rightpd[i] = new TransformDummy(_leftpd[i].Index, new Vector2(_leftpd[i].Position.X, _leftpd[i].Position.Y));
            //}

            //for (int i = 0; i < _lefttand.Length; ++i)
            //{
            //    _righttand[i] = new TransformDummy(_lefttand[i].Index, new Vector2(_lefttand[i].Position.X, _lefttand[i].Position.Y));
            //}

            TransformDummy[] rightpd = _leftpd.Clone() as TransformDummy[];
            TransformDummy[] righttand = _lefttand.Clone() as TransformDummy[];

            // create a flipped version
            // take the mid-point of Y values - then flip everything
            var minY = _rightpd[0].Position.Y;
            var maxY = _rightpd[0].Position.Y;

            foreach (var p in _rightpd)
            {
                if (minY > p.Position.Y) minY = p.Position.Y;
                if (maxY < p.Position.Y) maxY = p.Position.Y;
            }

            var reflectY = ((maxY - minY) / 2.0) + minY;

            float offsetY = 250;

            foreach (var p in _rightpd)
            {
                float newValue = (float)(reflectY - p.Position.Y) + offsetY;
                p.Position = new Vector2(p.Position.X, newValue);
            }

            foreach (var t in _righttand)
            {
                float newValue = (float)(reflectY - t.Position.Y) + offsetY;
                t.Position = new Vector2(t.Position.X, newValue);
            }
        }

        private void CreateLeftSpline()
        {
            _helixSplineLeft = new HermiteSpline();
            _helixSplineLeft.LoadJsonSplineData(_leftpd, _leftpmd, _lefttd, _lefttand);
        }

        private void ScaleSplineData()
        {
            float centreX = 1920 / 2;
            //float scale = 1.22f;
            float scale = 1.0f;

            // stretch it out
            // x values from centre (both position & tangent)
            foreach (var p in _leftpd)
            {
                float xDiff = centreX - p.Position.X;
                xDiff *= scale;
                p.Position = new Vector2(centreX - xDiff, p.Position.Y);
            }

            foreach (var t in _lefttand)
            {
                float xDiff = centreX - t.Position.X;
                xDiff *= scale;
                t.Position = new Vector2(centreX - xDiff, t.Position.Y);
            }
        }
    }
}
