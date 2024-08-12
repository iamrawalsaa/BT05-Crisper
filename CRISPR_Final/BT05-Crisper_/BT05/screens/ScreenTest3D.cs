using BT05;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using shared;
using SharedMonoGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace screens
{
    public class SplineSegment
    {
        public float Start = 0;
        public float End = 0;

        public Vector3 StartVector = new Vector3();
        public Vector3 EndVector = new Vector3();

        public float StartYaw { get; internal set; }
        public float StartPitch { get; internal set; }
        public float StartRoll { get; internal set; }

        public Matrix StartMatrix { get; internal set; }

        // repeat for yaw / pitch / row
        // need to tween between these
    }

    public class ScreenTest3D : GameScreenExtended
    {
        private float _timeInState;
        private bool _leavingToNextState;

        public ScreenTest3D(MyGameBase game, shared.GamePhase phase) : base(game, phase) { }

        public override void LoadContent()
        {
            base.LoadContent();
        }

        public override void ScreenArriving()
        {
            base.ScreenArriving();

            //PrimaryText = "{{BLACK}}3D Test";// Thanks for playing!\n Learn more about CAS9 and the other gene editing tools here in the Frontiers Gallery.";
            //SecondaryText = "{{GREEN}}Game will restart shortly!";

            ShowPrimaryText = false;
            ShowSecondaryText = false;

            _timeInState = 3.0f;
            _leavingToNextState = false;
        }


        List<SplineSegment> _splineSegments = new List<SplineSegment>();


        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 10), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.1f, 100f);

        private float _xRot = 1;
        private float _yRot = 1;
        private float _zRot = 1;
        bool _processedDoubleHelixModel = false;
        private int _globalYawDegrees = 0;
        private int _globalRollDegrees = 0;
        private float _globalPitchDegrees = -90;

        public override void Update(GameTime gameTime)
        {
            PrimaryTextOffset = new Vector2(0, -100);
            UpdateDrawables(gameTime);

            _xRot += gameTime.GetElapsedSeconds() * 0.8f;
            _yRot += gameTime.GetElapsedSeconds() * 0.6f;
            _zRot += gameTime.GetElapsedSeconds() * 0.5f;

            if (!_processedDoubleHelixModel)
            {
                _processedDoubleHelixModel = true;
                PullOutAllTheMarkers();
            }
        }

        public override void DrawInner(GameTime gameTime)
        {
            //CheckTextureNeedsRecreating();

            //Game.GraphicsDevice.Clear(Color.GhostWhite);
            //var transformMatrix = Game.Camera.GetViewMatrix();
            //Game._spriteBatch.Begin(transformMatrix: transformMatrix);
            //DrawPre(gameTime);
            //DrawTextTexture();
            //DrawPost(gameTime);

            DrawNucleotides();
            //DrawNucleotides_Alt();
            DrawNucleotides_AltSingle();
            DrawDoubleHelix();

            //DrawEntireDoubleHelxi();
            //Game._spriteBatch.End();
        }

        /// <summary>
        /// A debug helper method
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void DrawEntireDoubleHelxi()
        {
            Matrix world = CreateDoubleHelixWorldMatrix_Working();

            DrawModelUsingBones(SharedAssetManager.Instance.DoubleHelixMeshModel, world, view, projection, SharedAssetManager.Instance.ModelTextureDoubleHelix);
        }

        private Matrix CreateDoubleHelixWorldMatrix_Working()
        {
            var yaw = MathHelper.ToRadians(_globalYawDegrees);
            var pitch = MathHelper.ToRadians(_globalPitchDegrees);
            var roll = MathHelper.ToRadians(_globalRollDegrees);

            var x = -0.5f;
            var y = 0;
            var z = 0;

            var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var translationMatrix = Matrix.CreateTranslation(x, y, z);
            // X Positive is left
            // Y positive is up
            // Z position is towards the camera

            var scaleMatrix = Matrix.CreateScale(60f, 60f, 60f);
            var world = scaleMatrix * rotationMatrix * translationMatrix;
            return world;
        }

        /// <summary>
        /// This looks for all the individual Cone markers I added
        /// </summary>
        private void PullOutAllTheMarkers()
        {
            var model = SharedAssetManager.Instance.DoubleHelixMeshModel;

            // TODO: Remove all these bones too...
            // TODO: Remove all the models too

            Dictionary<int, ModelBone> coneBones = new Dictionary<int, ModelBone>();

            foreach (var bone in model.Bones)
            {
                if (bone.Name.Contains("Cone01"))
                {
                    var numbers = bone.Name.Right(3);
                    if (Int32.TryParse(numbers, out int index))
                    {
                        coneBones.Add(index, bone);
                    }
                }
            }

            int totalIndex = coneBones.Count;
            float segmentDiff = 1.0f / totalIndex;
            float segmentStart = 0;

            for (int i = 0; i < totalIndex; i++)
            {
                var bone = coneBones[i];
                var matrix = bone.ModelTransform * CreateDoubleHelixWorldMatrix_Working();

                // This was the working line
                //var matrix = bone.ModelTransform * CreateDoubleHelixWorldMatrix_Working();


                var transformedVector = Vector3.Transform(Vector3.Zero, matrix);

                //var source = Vector3.Transform(new Vector3(0, 0, 0), matrix);
                var source = new Vector3(1, 0, 0);
                var target = Vector3.Transform(source, matrix) - transformedVector;

                Vector3 direction = Vector3.Normalize(target - source);

                float yaw = (float)Math.Atan2(direction.X, -direction.Z);
                float pitch = (float)Math.Asin(direction.Y);
                float roll = 0f;

                var yawDegrees = MathHelper.ToDegrees(yaw);
                var pitchDegrees = MathHelper.ToDegrees(pitch);

                //float yaw = (float)Math.Atan2(matrix.M13, matrix.M11);
                //float pitch = (float)Math.Asin(-matrix.M12);
                //float roll = (float)Math.Atan2(matrix.M23, matrix.M22);

                var splineSegment = new SplineSegment();
                splineSegment.Start = segmentStart;
                splineSegment.End = segmentStart + segmentDiff;
                splineSegment.StartVector = transformedVector;

                splineSegment.StartYaw = yaw;
                splineSegment.StartPitch = pitch;
                splineSegment.StartRoll = roll;

                splineSegment.StartMatrix = bone.ModelTransform;

                _splineSegments.Add(splineSegment);

                segmentStart += segmentDiff;
            }
        }

        /// <summary>
        /// This creates a new segment at the precise location
        /// </summary>
        /// <param name="lerp"></param>
        /// <returns></returns>
        SplineSegment CreateSplineSegment(float lerp)
        {
            for (int i = 0; i < _splineSegments.Count; ++i)
            {
                var s = _splineSegments[i];

                if (lerp > s.Start && lerp < s.End)
                {
                    if (i + 1 < _splineSegments.Count)
                    {
                        var e = _splineSegments[i + 1];
                        var createdSegment = new SplineSegment();

                        var matrixStart = s.StartMatrix;
                        var matrixEnd = e.StartMatrix;


                        float width = s.End - s.Start;
                        float withinRange = lerp - s.Start;

                        float localLerp = withinRange / width;

                        var matrixLerp = MatrixLerp(matrixStart, matrixEnd, localLerp);

                        createdSegment.StartMatrix = matrixLerp;

                        return createdSegment;
                    }
                }
            }

            return null;
        }

        private void DrawDoubleHelix()
        {
            var model = SharedAssetManager.Instance.DoubleHelixMeshModel;
            Matrix world = CreateDoubleHelixWorldMatrix_Working();

            //world = Matrix.Identity;
            //world = scaleMatrix;

            //DrawModel(model, world, view, projection, SharedAssetManager.Instance.ModelTextureDoubleHelix);
            DrawDoubleHelixModel(model, world, view, projection, SharedAssetManager.Instance.ModelTextureDoubleHelixTrans);
        }

        //private static Matrix CreateDoubleHelixWorldMatrix()
        //{
        //    var yaw = MathHelper.ToRadians(0);
        //    var pitch = MathHelper.ToRadians(-90);
        //    var roll = MathHelper.ToRadians(0);

        //    var x = -150;
        //    var y = 0;
        //    var z = 0;

        //    var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
        //    var translationMatrix = Matrix.CreateTranslation(x, y, z);
        //    // X Positive is left
        //    // Y positive is up
        //    // Z position is towards the camera

        //    //            var scaleMatrix = Matrix.CreateScale(0.5f, 0.5f, 0.5f);
        //    var scaleMatrix = Matrix.CreateScale(60f, 60f, 60f);
        //    var world = scaleMatrix * rotationMatrix * translationMatrix;
        //    return world;
        //}

        private void DrawDoubleHelixModel(Model model, Matrix world, Matrix view, Matrix projection, Texture2D texture = null)
        {
            //var bottom = model.Meshes[0];
            //var top = model.Meshes[1];


            ModelBone bottomBone = model.Bones[1];
            ModelBone topBone = model.Bones[2];

            var bottom = bottomBone.Meshes[0];
            var top = topBone.Meshes[0];

            BasicEffect bottomEffect = bottom.Effects[0] as BasicEffect;
            BasicEffect topEffect = top.Effects[0] as BasicEffect;


            bottomEffect.World = bottomBone.ModelTransform * world;
            bottomEffect.View = view;
            bottomEffect.Projection = projection;

            bottom.Draw();

            topEffect.World = topBone.ModelTransform * world;
            topEffect.View = view;
            topEffect.Projection = projection;

            top.Draw();
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection, Texture2D texture = null)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    if (texture != null)
                    {
                        effect.Texture = texture;
                    }
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw();
            }
        }

        private void DrawModelUsingBones(Model model, Matrix world, Matrix view, Matrix projection, Texture2D texture = null)
        {
            int boneCount = 0;
            int meshCount = 0;
            int effectCount = 0;

            foreach (ModelBone bone in model.Bones)
            {
                ++boneCount;
                foreach (ModelMesh mesh in bone.Meshes)
                {
                    ++meshCount;
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        ++effectCount;

                        if (texture != null)
                        {
                            effect.Texture = texture;
                        }
                        //effect.World = world * bone.ModelTransform;
                        effect.World = bone.ModelTransform * world;

                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }

            string total = "Bone: " + boneCount + " Mesh: " + meshCount + " Effect: " + effectCount;
        }

        private void DrawNucleotidesRenderTest()
        {
            int x = -20, y = -5;
            for (int i = 0; i < 800; ++i)
            {
                var model = SharedAssetManager.Instance.PAMModel;

                var yaw = _yRot;
                var pitch = _xRot;
                var roll = _zRot;

                var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
                var translationMatrix = Matrix.CreateTranslation(x, y, -20);
                // X Positive is left
                // Y positive is up
                // Z position is towards the camera

                var scaleMatrix = Matrix.CreateScale(0.5f, 0.5f, 2.0f);
                var world = scaleMatrix * rotationMatrix * translationMatrix;

                DrawModel(model, world, view, projection, SharedAssetManager.Instance.ModelTextureNone);

                ++x;
                if (x == 20)
                {
                    x = -20;
                    ++y;
                }
            }
        }

        private void DrawNucleotides()
        {
            var model = SharedAssetManager.Instance.PAMModel;

            var yaw = _yRot;
            var pitch = _xRot;
            var roll = _zRot;

            // we're just drawing these at the markers
            foreach (var v in _splineSegments)
            {
                yaw = v.StartYaw;
                pitch = v.StartPitch;
                roll = v.StartRoll;

                yaw = MathHelper.ToRadians(0);
                pitch = MathHelper.ToRadians(-90);
                roll = MathHelper.ToRadians(0);

                yaw = v.StartYaw;
                pitch = v.StartPitch;
                roll = v.StartRoll;

                var translationMatrix = Matrix.CreateTranslation(v.StartVector);// * CreateDoubleHelixWorldMatrix();

                var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
                // X Positive is left
                // Y positive is up
                // Z position is towards the camera

                //var scaleMatrix = Matrix.CreateScale(0.05f, 0.05f, 0.2f);
                var scaleMatrix = Matrix.CreateScale(0.2f, 0.2f, 1f);

                //                var world = scaleMatrix * rotationMatrix * translationMatrix * CreateDoubleHelixWorldMatrix_Working();
                //var world = scaleMatrix * translationMatrix * CreateDoubleHelixWorldMatrix_Working();

                // This bit is working
                var world = scaleMatrix * v.StartMatrix * CreateDoubleHelixWorldMatrix_Working();

                DrawModel(model, world, view, projection, SharedAssetManager.Instance.ModelTextureNone);
            }
        }

        int _globalNucleotideDrawing = 0;

        /// <summary>
        /// This is supposed to use the loaded Nucleotide markers to interpolate the inbetween
        /// locations.
        /// However, this isn't working.
        /// It's only showing a handful of nucleotides
        /// I'm not sure why it's not at least showing a number equal to the count
        /// 
        /// It's possible that lerping between matrices just doesn't work.
        /// Maybe I can try pulling out the individual translate and rotations out of 
        /// the loaded data?
        /// I got the translate working before but not the rotation
        /// 
        /// </summary>
        private void DrawNucleotides_Alt()
        {
            _globalNucleotideDrawing = 0;

            int count = 10;
            float diff = 1.0f / count;
            float index = 0;

            for (int i = 0; i < count; ++i)
            {
                var splineSegment = CreateSplineSegment(index);
                DrawNucleotide(splineSegment);

                index += diff;
            }
        }


        float _singleNucleotideIndex = 0.05f;

        /// <summary>
        /// This draws a single Nucleotide but keeps it moving
        /// </summary>
        private void DrawNucleotides_AltSingle()
        {
            _globalNucleotideDrawing = 0;

            int count = 500;
            float diff = 1.0f / count;
            var splineSegment = CreateSplineSegment(_singleNucleotideIndex);
            DrawNucleotide(splineSegment);

            _singleNucleotideIndex += diff;
            if (_singleNucleotideIndex > 0.95) _singleNucleotideIndex = 0.05f;
        }

        private void DrawNucleotide(SplineSegment splineSegment)
        {
            if (splineSegment != null)
            {
                var model = SharedAssetManager.Instance.PAMModel;
                //                var scaleMatrix = Matrix.CreateScale(0.2f, 0.2f, 1f);
                var scaleMatrix = Matrix.CreateScale(0.2f, 0.2f, 2f);


                var world = scaleMatrix * splineSegment.StartMatrix * CreateDoubleHelixWorldMatrix_Working();

                ++_globalNucleotideDrawing;

                DrawModel(model, world, view, projection, SharedAssetManager.Instance.ModelTextureA);
            }
        }

        public override void MouseDragChanged(int xChange, int yChange)
        {
            _globalYawDegrees += xChange;
            _globalPitchDegrees += yChange;

            base.MouseDragChanged(xChange, yChange);
        }

        public Matrix MatrixLerp(Matrix matrix1, Matrix matrix2, float lerpAmount)
        {
            if (lerpAmount < 0 || lerpAmount > 1f)
            {
                int errorLerpIsOutOfBounds = 0;
                ++errorLerpIsOutOfBounds;
            }

            // Two 3D matrices

            // Interpolate translation components
            Vector3 translation = Vector3.Lerp(matrix1.Translation, matrix2.Translation, lerpAmount);

            // Interpolate rotation components using Quaternion.Slerp
            Quaternion rotation1 = Quaternion.CreateFromRotationMatrix(matrix1);
            Quaternion rotation2 = Quaternion.CreateFromRotationMatrix(matrix2);
            Quaternion rotation = Quaternion.Slerp(rotation1, rotation2, lerpAmount);

            Vector3 scale1;
            scale1.X = new Vector4(matrix1.M11, matrix1.M12, matrix1.M13, matrix1.M14).Length();
            scale1.Y = new Vector4(matrix1.M21, matrix1.M22, matrix1.M23, matrix1.M24).Length();
            scale1.Z = new Vector4(matrix1.M31, matrix1.M32, matrix1.M33, matrix1.M34).Length();

            // Extract scale components from matrix2
            Vector3 scale2;
            scale2.X = new Vector4(matrix2.M11, matrix2.M12, matrix2.M13, matrix2.M14).Length();
            scale2.Y = new Vector4(matrix2.M21, matrix2.M22, matrix2.M23, matrix2.M24).Length();
            scale2.Z = new Vector4(matrix2.M31, matrix2.M32, matrix2.M33, matrix2.M34).Length();

            // Interpolate scale components using Vector3.Lerp
            Vector3 scale = Vector3.Lerp(scale1, scale2, lerpAmount);

            // Reconstruct the midpoint matrix
            Matrix midpointMatrix = Matrix.CreateScale(scale) *
                                   Matrix.CreateFromQuaternion(rotation) *
                                   Matrix.CreateTranslation(translation);

            return midpointMatrix;
        }

    }
}