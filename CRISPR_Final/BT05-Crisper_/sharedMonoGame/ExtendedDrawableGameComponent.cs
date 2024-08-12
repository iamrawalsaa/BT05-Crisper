using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using viewer;

namespace SharedMonoGame
{
    /// <summary>
    /// Drawable Game Objects that need a RecreateTexture method (on viewer)
    /// </summary>
    public abstract class ExtendedDrawableGameComponent : DrawableGameComponent
    {
        public ExtendedDrawableGameComponent(MyGameBase game) : base(game)
        {
        }

        public abstract void RecreateTextures();

    }
}
