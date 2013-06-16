using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace ChuckAmuckPlatformer
{
    class Pickup
    {
        Level level;

        private Texture2D texture;
        private Vector2 origin;
        //private SoundEffect collectedSound;

        public const int PointValue = 25;
        public readonly Color Color = Color.Coral;

        
        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        public Pickup(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Pickups/pizza");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            //collectedSound = Level.Content.Load<SoundEffect>("Sounds/PickupCollected");
        }

        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.1f;
            const float BounceRate = 2.0f;
            const float BounceSync = -0.75f;
       
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        public void OnCollected(Player collectedBy)
        {
            //collectedSound.Play();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}