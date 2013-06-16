//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

//namespace ChuckAmuckPlatformer
//{
//    class Physics
//    {
//        public void ApplyPlayerPhysics(Player player, GameTime gameTime)
//        {         
//            // Constants for controling horizontal movement
//            const float MoveAcceleration = 13000.0f;
//            const float MaxMoveSpeed = 1750.0f;
//            const float GroundDragFactor = 0.48f;
//            const float AirDragFactor = 0.58f;

//            // Constants for controlling vertical movement
//            const float GravityAcceleration = 3400.0f;
//            const float MaxFallSpeed = 550.0f;
            
//            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
//            Vector2 velocity = player.Velocity;
            
//            float movement = player.Movement;

//            Vector2 previousPosition = player.Position;
            
//            velocity.X += movement * MoveAcceleration * elapsed;
//            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

//            if (player.IsJumping)
//                        velocity.Y = player.DoJump(velocity.Y, gameTime);

//            // Apply pseudo-drag horizontally.
//            if (player.IsOnGround)
//                velocity.X *= GroundDragFactor;
//            else
//                  velocity.X *= AirDragFactor;

//            // Prevent the player from running faster than his top speed.            
//            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

//            // Apply velocity.
//            player.Position += velocity * elapsed;
//            player.Position = new Vector2((float)Math.Round(player.Position.X), (float)Math.Round(player.Position.Y));
            
//            // If the player is now colliding with the level, separate them.
//            player.HandleCollisions();

//            // If the collision stopped us from moving, reset the velocity to zero.
//            if (player.Position.X == previousPosition.X)
//                velocity.X = 0;

//            if (player.Position.Y == previousPosition.Y)
//                velocity.Y = 0;

//            player.Velocity = velocity;
//            player.Velocity = new Vector2((float)Math.Round(velocity.X), (float)Math.Round(velocity.Y));
//        }

//        public void ApplyEnemyPhysics(Enemy enemy, GameTime gameTime)
//        {
//            // Constants for controling horizontal movement
//            const float MoveAcceleration = 5000.0f;
//            const float MaxMoveSpeed = 1250.0f;
//            const float GroundDragFactor = 0.48f;
//            const float AirDragFactor = 0.58f;

//            // Constants for controlling vertical movement
//            const float GravityAcceleration = 3400.0f;
//            const float MaxFallSpeed = 550.0f;

//            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
//            Vector2 velocity = enemy.Velocity;

//            float movement = enemy.Movement;

//            Vector2 previousPosition = enemy.Position;

//            velocity.X += movement * MoveAcceleration * elapsed;
//            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

//            if (enemy.IsJumping)
//                velocity.Y = enemy.DoJump(velocity.Y, gameTime);

//            // Apply pseudo-drag horizontally.
//            if (enemy.IsOnGround)
//                velocity.X *= GroundDragFactor;
//            else
//                velocity.X *= AirDragFactor;

//            // Prevent the enemy from running faster than his top speed.            
//            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

//            // Apply velocity.
//            enemy.Position += velocity * elapsed;
//            enemy.Position = new Vector2((float)Math.Round(enemy.Position.X), (float)Math.Round(enemy.Position.Y));

//            // If the enemy is now colliding with the level, separate them.
//            enemy.HandleCollisions();

//            // If the collision stopped us from moving, reset the velocity to zero.
//            if (enemy.Position.X == previousPosition.X)
//                velocity.X = 0;

//            if (enemy.Position.Y == previousPosition.Y)
//                velocity.Y = 0;

//            enemy.Velocity = velocity;
//            enemy.Velocity = new Vector2((float)Math.Round(velocity.X), (float)Math.Round(velocity.Y));
//        }
//    }
//}
