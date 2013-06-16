using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace ChuckAmuckPlatformer
{
    class Level : IDisposable
    {
        //Structure of the level
        private Tile[,] tiles;
        private Layer[] layers;
        private const int EntityLayer = 2;

        Player player;
        //Physics levelPhysics = new Physics();
        int score;
        bool reachedEndpoint;

        public Player Player
        {
            get { return player; }
        }
        

        private List<Pickup> pickups = new List<Pickup>();
        private List<Enemy> enemy = new List<Enemy>();

        //Key locations in the level
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        private float cameraPosition;

        //Arbitrary, but constant seed
        private Random random = new Random(354668);

        public int Score
        {
            get { return score; }
        }
        
        public bool ReachedEndpoint
        {
            get { return reachedEndpoint; }
        }

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;

        private const int PointsPerSecond = 5;

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        //private SoundEffect endpointReachedSound;


        ///==================================
        ///      Constructs a new level
        ///================================== 
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            content = new ContentManager(serviceProvider, "Content");

            timeRemaining = TimeSpan.FromMinutes(2.0);

            LoadTiles(fileStream);

            layers = new Layer[3];
            layers[0] = new Layer(Content, "Backgrounds/Layer0", 0.2f);
            layers[1] = new Layer(Content, "Backgrounds/Layer1", 0.5f);
            layers[2] = new Layer(Content, "Backgrounds/Layer2", 0.8f);

            //endpointReachedSound = Content.Load<SoundEffect>("Sounds/EndpointReached");
        }

        private void LoadTiles(Stream fileStream)
        {
            int width;
            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;

                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            //Tile grid
            tiles = new Tile[width, lines.Count];

            //Loop over every tile
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    //Load each tile
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                //Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);
                
                //Exit
                case 'X':
                    return LoadExitTile(x, y);
                
                //Pickup
                case 'P':
                    return LoadPickupTile(x, y);

                //Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                //Enemies
                case 'A':
                    return LoadNibblesTile(x, y, "Nibbles");
                case 'B':
                    return LoadWigglesTile(x, y, "Wiggles");
                case 'C':
                    return LoadPiddlesTile(x, y, "Piddles");
                case 'D':
                    return LoadMoTile(x, y, "Mo");

                //Platform
                case '~':
                    return LoadVarietyTile("Backgrounds/sidewalkup", 2, TileCollision.Platform);

                 //Passable platform
                case ':':
                    return LoadVarietyTile("Backgrounds/sidewalkacross", 2, TileCollision.Passable);
                
                //Load player at starting point
                case 'J':
                    return LoadStartTile(x, y);

                //Impassible block
                case '#':
                    return LoadVarietyTile("Backgrounds/sidewalkcorner", 7, TileCollision.Impassable);

                //Unknown tile type
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type {0} at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>(name), collision);
        }
          
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName /*+ index*/, collision);
        }
            
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);          
        }

        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Pickups/candy", TileCollision.Passable);
        }

        //private Tile LoadEnemyTile(int x, int y, string spriteSet)
        //{
        //    Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
        //    enemy.Add(new Enemy(this, position, spriteSet));

        //    return new Tile(null, TileCollision.Passable);
        //}

        private Tile LoadMoTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemy.Add(new EnemyMo(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadWigglesTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemy.Add(new EnemyWiggles(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadNibblesTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemy.Add(new EnemyNibbles(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadPiddlesTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemy.Add(new EnemyPiddles(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadPickupTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            pickups.Add(new Pickup(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        public void Dispose()
        {
            Content.Unload();
        }

        ///==================================
        ///       Bounds and Collision
        ///==================================
        
        public TileCollision GetCollision(int x, int y)
        {
            //Prevent escaping past the level end
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            //Allow jumping past the level's top and falling through the bottom
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        public void Update (GameTime gameTime, KeyboardState keyboardState)
        {
            if (!Player.IsAlive || TimeRemaining == TimeSpan.Zero)
            {
                Player.ApplyPhysics(gameTime);
            }
            else if (ReachedEndpoint)
            {
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score += seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;
                Player.Update(gameTime, keyboardState);
                UpdatePickups(gameTime);

                //If the player reaches the bottom edge of the level, they die
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

                UpdateEnemies(gameTime);

                if (Player.IsAlive && Player.IsOnGround && Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }

            if (timeRemaining < TimeSpan.Zero)
                timeRemaining = TimeSpan.Zero;
        }
        
        private void UpdatePickups(GameTime gameTime)
        {
            for (int i = 0; i < pickups.Count; ++i)
            {
                Pickup pickup = pickups[i];

                pickup.Update(gameTime);

                if (pickup.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    pickups.RemoveAt(i--);
                    OnPickupCollected(pickup, Player);
                }
            }
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemies in enemy)
            {
                enemies.Update(gameTime);

                if (enemies.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(enemies);
                }
            }
        }

        private void OnPickupCollected(Pickup pickup, Player collectedBy)
        {
            score += Pickup.PointValue;

            pickup.OnCollected(collectedBy);
        }

        private void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
        }

        private void OnExitReached()
        {
            Player.OnReachedExit();
            //exitReachedSound.Play();
            reachedEndpoint = true;
        }

        public void StartNewLife()
        {
            Player.Reset(start);
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i <= EntityLayer; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();

            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition, 0.0f, 0.0f);
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, cameraTransform);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, null, null, null, null, cameraTransform);

            DrawTiles(spriteBatch);

            foreach (Pickup pickup in pickups)
                pickup.Draw(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            foreach (Enemy hamster in enemy)
                hamster.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();
            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();
        }

        private void ScrollCamera(Viewport viewport)
        {
            const float ViewMargin = 0.35f;

            // Calculate the edges of the screen.
            float marginWidth = viewport.Width * ViewMargin;
            float marginLeft = cameraPosition + marginWidth;
            float marginRight = cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.0f;
            if (Player.Position.X < marginLeft)
                cameraMovement = Player.Position.X - marginLeft;
            else if (Player.Position.X > marginRight)
                cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.Width * Width - viewport.Width;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(cameraPosition / Tile.Width);
            int right = left + spriteBatch.GraphicsDevice.Viewport.Width / Tile.Width;
            right = Math.Min(right, Width - 1);
            
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
    }
}
    
