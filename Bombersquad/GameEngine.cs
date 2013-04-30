#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion
/*
 * GameEngine File
 * All Rendering Logic/Code goes here
 * @Citation: Using MonoGame based on the XNA framework [ http://www.monogame.net/ ]
 */


namespace bombersquad_ai
{
    /// This is the main type for your game
    public class GameEngine : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // BomberSquad MainClass
        MainClass bomberSquadMainClass;
        
        // Declare rectangles and textures
        Rectangle[,] gameTiles;
        Rectangle backgroundRect;
        Texture2D logoTexture;
        Texture2D backgroundTexture;
        Texture2D backgroundSwampTexture;
        Texture2D playerTexture;
        Texture2D AI1Texture;
        Texture2D AI2Texture;
        Texture2D AI3Texture;
        Texture2D AI4Texture;
        Texture2D breakableWallTexture;
        Texture2D breakableWallSwampTexture;
        Texture2D unbreakableWallTexture;
        Texture2D unbreakableWallSwampTexture;
        Texture2D explosionTexture;
        Texture2D explosionBiggerTexture;
        Texture2D bombTexture;
        Texture2D AISuper2Texture;
        Texture2D AISuper3Texture;
        Texture2D AISuper4Texture;

        // Player input
        KeyboardState keyboardState;
        DateTime lastUpdated;

        // Temporary variables
        bool updateOnce = false;
        Random rand;
        int levelID;

        public GameEngine()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = GameConfig.isFullScreen;
            graphics.PreferredBackBufferHeight = GameConfig.graphicsHeight;
            graphics.PreferredBackBufferWidth = GameConfig.graphicsWidth;
            Content.RootDirectory = GameConfig.contentDirectory;
        }


        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            bomberSquadMainClass = new MainClass();
            bomberSquadMainClass.runOnce();
            this.lastUpdated = DateTime.Now;
            
            // Prioritize call of Draw()
            graphics.SynchronizeWithVerticalRetrace = false;
            
            // Call update once every 100 milliseconds
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 20.0f);
            
            gameTiles = new Rectangle[GameConfig.gameHeight, GameConfig.gameWidth];

            base.Initialize();
        }


        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here           
            this.rand = new Random();
            this.levelID = rand.Next(0, 2); // 0 = normal, 1 = swamp (minValue is inclusive, but maxValue is exclusive)

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
			
            // Initialize textures
            logoTexture = Content.Load<Texture2D>("logo");
            backgroundTexture = Content.Load<Texture2D>("background_640x640");
            backgroundSwampTexture = Content.Load<Texture2D>("background_swamp_640x640");
            playerTexture = Content.Load<Texture2D>("bombermanfront_32x32");
            AI1Texture = Content.Load<Texture2D>("bluebomberman_32x32");
            AI2Texture = Content.Load<Texture2D>("orangebomberman_32x32");
            AI3Texture = Content.Load<Texture2D>("pinkbomberman_32x32");
            AI4Texture = Content.Load<Texture2D>("redbomberman_32x32");
            breakableWallTexture = Content.Load<Texture2D>("breakable_32x32");
            unbreakableWallTexture = Content.Load<Texture2D>("unbreakable_32x32");
            breakableWallSwampTexture = Content.Load<Texture2D>("breakable_swamp_32x32");
            unbreakableWallSwampTexture = Content.Load<Texture2D>("unbreakable_swamp_32x32");
            explosionTexture = Content.Load<Texture2D>("explosion_32x32");
            explosionBiggerTexture = Content.Load<Texture2D>("explosionbigger_32x32");
            bombTexture = Content.Load<Texture2D>("bomb_32x32");
            AISuper2Texture = Content.Load<Texture2D>("super2bomberman_32x32");
            AISuper3Texture = Content.Load<Texture2D>("super3bomberman_32x32");
            AISuper4Texture = Content.Load<Texture2D>("super4bomberman_32x32");
            
            // Initialize rectangles
            backgroundRect = new Rectangle(0, 0, GameConfig.graphicsWidth, GameConfig.graphicsHeight);
            int tileWidth = GameConfig.tileWidth;
            int tileHeight = GameConfig.tileHeight;
            for (int y = 0; y < GameConfig.gameHeight; y++)
            {
                for (int x = 0; x < GameConfig.gameHeight; x++)
                {
                    gameTiles[y,x] = new Rectangle(y * tileWidth, x * tileHeight, tileWidth, tileHeight);
                }
            }



        }


        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DateTime currentTime = DateTime.Now;

            // Handle keyboard input
            Action playerAction = null;
            this.keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                playerAction = new Action(Action.ActionType.BOMB);
                Console.WriteLine("> Key Pressed: SPACE");
			}
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
				playerAction = new Action(Action.ActionType.NORTH);
                Console.WriteLine("> Key Pressed: UP");
			}
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                playerAction = new Action(Action.ActionType.WEST);
                Console.WriteLine("> Key Pressed: LEFT");
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                playerAction = new Action(Action.ActionType.EAST);
                Console.WriteLine("> Key Pressed: RIGHT");
			}
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                playerAction = new Action(Action.ActionType.SOUTH);
                Console.WriteLine("> Key Pressed: DOWN");
            }

            if(currentTime - this.lastUpdated > new TimeSpan(0,0,0,0,150))
            {
                DateTime start = DateTime.Now;
                Action[] botActions = bomberSquadMainClass.game.getAI().GetActions();
                //Action[] botActions = new Action[4];
                DateTime end = DateTime.Now;
                Console.WriteLine("GetActions took " + (end - start));
                bomberSquadMainClass.game.getGameState().UpdateGame(playerAction, botActions);
                Console.WriteLine("> Completed one update cycle inside time-check condition.");
                this.lastUpdated = currentTime;
            }

            /*
            DateTime end = DateTime.Now;
            TimeSpan diff = end - begin;

            if (diff < bomberSquadMainClass.game.getLoopTime()) {
                TimeSpan wait = bomberSquadMainClass.game.getLoopTime() - diff;
                //Thread.Sleep(wait);
            }
            */

            base.Update(gameTime);
        }


        /// This is called when the game should draw itself.
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Background color
            graphics.GraphicsDevice.Clear(Color.LightSeaGreen);

            spriteBatch.Begin();
            // --- START Draw Cod

            Console.WriteLine("*** LEVEL ID = " + this.levelID.ToString() + " ***");

            // Draw background
            spriteBatch.Draw((this.levelID == 0) ? backgroundTexture : backgroundSwampTexture, backgroundRect, Color.White);


            Texture2D tempTexture;

            for (int y = 0; y < GameConfig.gameHeight; y++)
            {
                for (int x = 0; x < GameConfig.gameHeight; x++)
                {
                    LocationData loc = this.bomberSquadMainClass.game.getGameState().GetLocationData(x, y);
                    if (loc != null)
                    {
                        char tileType = ConsoleCanvas.GetLocationChar(loc);
                        switch (tileType)
                        {
                            case '1':
                                tempTexture = AI1Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '2':
                                tempTexture = AI2Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '3':
                                tempTexture = AI3Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '4':
                                tempTexture = AI4Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case 'B':
                                tempTexture = bombTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '-':
                                tempTexture = (this.levelID == 0) ? breakableWallTexture : breakableWallSwampTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '=':
                                tempTexture = (this.levelID == 0) ? unbreakableWallTexture : unbreakableWallSwampTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case 'e':
                                tempTexture = explosionTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case 'E':
                                tempTexture = explosionBiggerTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case 'P':
                                tempTexture = playerTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '@':
                                tempTexture = AISuper2Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '#':
                                tempTexture = AISuper3Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '$':
                                tempTexture = AISuper4Texture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            case '!':
                                tempTexture = logoTexture;
                                spriteBatch.Draw(tempTexture, gameTiles[x, y], Color.White);
                                break;
                            default:
                                // Do nothing
                                break;
                        }
                    }
                }
            }
            
            

            // --- END Draw Code
            spriteBatch.End();

            base.Draw(gameTime);

            //bomberSquadMainClass.drawGameConsole();
        }
    }
}
