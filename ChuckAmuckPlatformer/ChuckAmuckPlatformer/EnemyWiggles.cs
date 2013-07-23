using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChuckAmuckPlatformer
{
    class EnemyWiggles : Enemy
    {
        protected const float MoveSpeed = 32.0f;
        
        public EnemyWiggles(Level levelWiggles, Vector2 positionWiggles, string spriteSetWiggles, int contactDamageWiggles)
        {
            this.level = levelWiggles;
            this.position = positionWiggles;
            this.ContactDamage = contactDamageWiggles;
            LoadContent(spriteSetWiggles);
        }

        public override void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "EnemySprites/wigglesspritesheet";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet /*+ "Run"*/), 0.15f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet /*+ "Idle"*/), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Wait for some amount of time.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Move in the current direction.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }
    }
}
