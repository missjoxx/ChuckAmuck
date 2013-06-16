using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ChuckAmuckPlatformer
{
    public class ChuckAmuckGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont hudFont;

        private Texture2D menuScreen;
        private Texture2D winScreen;
        private Texture2D deadScreen;
        //private Texture2D helpScreen;

        private int levelIndex = -1;
        private Level level;
        private bool isPressedContinue;

        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        private KeyboardState keyboardState;

        private const int numLevels = 1;

        public ChuckAmuckGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            winScreen = Content.Load<Texture2D>("GameScreens/win");
            deadScreen = Content.Load<Texture2D>("GameScreens/gameover");
            menuScreen = Content.Load<Texture2D>("GameScreens/intro");
            //helpScreen = Content.Load<Texture2D>("GameScreens/help");

            LoadNextLevel();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            level.Update(gameTime, keyboardState);

            base.Update(gameTime);
        }

        private void HandleInput()
        {
            keyboardState = Keyboard.GetState();

            bool continuallyPressed = keyboardState.IsKeyDown(Keys.Space);

            if (!isPressedContinue && continuallyPressed)
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedEndpoint)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            isPressedContinue = continuallyPressed;
        }

        private void LoadNextLevel()
        {
            //Move on to the next level
            levelIndex = (levelIndex + 1) % numLevels;

            //Clear out the previous level
            if (level != null)
                level.Dispose();

            //Load in the new level
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            level.Draw(gameTime, spriteBatch);

            DrawHud();

            base.Draw(gameTime);
        }

        private void DrawHud()
        {
            spriteBatch.Begin();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            string timeString = "Time Remaining: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");

            Color timeColor;

            if (level.TimeRemaining > WarningTime ||
                level.ReachedEndpoint ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = Color.Yellow;
            }
            else
            {
                timeColor = Color.Red;
            }

            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            //Draw the score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "Score: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.Yellow);

            //Determine Game Screen to draw
            Texture2D gameScreenStatus = null;

            if (level.TimeRemaining == TimeSpan.Zero)
            {
                if (level.ReachedEndpoint)
                {
                    gameScreenStatus = winScreen;
                }
                else
                {
                    gameScreenStatus = deadScreen;
                }
            }
            else if (!level.Player.IsAlive)
            {
                gameScreenStatus = deadScreen;
            }

            if (gameScreenStatus != null)
            {
                //Draw Game Screen overlay
                Vector2 statusSize = new Vector2(gameScreenStatus.Width, gameScreenStatus.Height);
                spriteBatch.Draw(gameScreenStatus, center - statusSize / 2, Color.White);
            }

            spriteBatch.End();
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
