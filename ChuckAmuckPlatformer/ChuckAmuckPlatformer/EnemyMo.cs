using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ChuckAmuckPlatformer
{
    class EnemyMo : Enemy
    {
        public EnemyMo(Level levelMo, Vector2 positionMo, string spriteSetMo, int contactDamageMo)
        {
            this.level = levelMo;
            this.position = positionMo;
            this.ContactDamage = contactDamageMo;
            LoadContent(spriteSetMo);
        }

        public override void LoadContent(string spriteSet)
        {
            // Load animations.
            spriteSet = "EnemySprites/mospritesheet";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet /*+ "Run"*/), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet /*+ "Idle"*/), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }
    }
}
