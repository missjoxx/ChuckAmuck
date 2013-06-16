using System;
using Microsoft.Xna.Framework.Graphics;

namespace ChuckAmuckPlatformer
{
    class Animation
    {
        Texture2D texture;
        float frameTime;
        bool isLooping;

        public Texture2D Texture
        {
            get { return texture; }
        }
        
        public float FrameTime
        {
            get { return frameTime; }
        }
        
        public bool IsLooping
        {
            get { return isLooping; }
        }
        
        public int FrameCount
        {
            get { return (Texture.Width / FrameWidth) * (Texture.Height / FrameHeight); }
        }

        public int FrameWidth
        {
            get { return Texture.Height; }
        }

        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}
