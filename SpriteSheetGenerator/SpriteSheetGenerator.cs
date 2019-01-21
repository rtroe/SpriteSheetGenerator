using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SpriteSheetGenerator
{
    enum SpriteGenState
    {
        Initial,
        LoadingTextures,
        TexturesLoaded,
        LayingOutTextures,
        GeneratingSpriteSheet,
        Finished
    }

    /// <summary>
    /// This game is a Sprite Sheet Generator Utility.
    /// </summary>
    public class SpriteSheetGenerator : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;


        public static string Version;

        string[] files;

        float progressBarPercent = 0;

        SpriteGenState SpriteGenState = SpriteGenState.Initial;

        SpriteFont Font;

        string dir = "";

        string titleText = "";

        int MaxWidth = 256;

        int MaxHeight = 256;


        int currentLoadIndex = 0;

        SpriteSheet SpriteSheet;

        string status = "";

        bool execute = false;

        string[] args = { };

        string outputDir = "";

        public SpriteSheetGenerator(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.args = args;

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            dir = Environment.CurrentDirectory;

            outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "SpriteGen", 
                DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("THHmmss"));

            titleText = "Sprite Sheet Creator - v." + Version;

            titleText += "\n========================================================";
            titleText += "\nargs: ";

            Console.ForegroundColor = ConsoleColor.Yellow;
            try
            {
                for (int i = 1; i < args.Length; i++)
                {
                    titleText += " " + args[i];

                    switch (args[i])
                    {
                        case "-w":
                            Console.WriteLine("\tSetting Width to " + int.Parse(args[i + 1]));
                            MaxWidth = int.Parse(args[i + 1]);
                            break;
                        case "-h":
                            Console.WriteLine("\tSetting Height to " + int.Parse(args[i + 1]));
                            MaxHeight = int.Parse(args[i + 1]);
                            break;
                        case "-dir":
                            Console.WriteLine("\tUsing Directory " + args[i + 1]);
                            dir = args[i + 1];
                            break;
                        case "-output":
                            Console.WriteLine("\tOutputing To Directory " + args[i + 1]);
                            outputDir = args[i + 1];
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }

            Console.ResetColor();
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load Content
            Font = Content.Load<SpriteFont>("File");
            TextureUtil.DefaultTexture = TextureUtil.CreateTexture(GraphicsDevice, 32, 32, pixel => Color.White);

            // this is a JSON serializable SpriteSheet Object
            SpriteSheet = new SpriteSheet(GraphicsDevice, outputDir, MaxWidth, MaxHeight);


            // now get the list of all pngs in the provided directory
            var ext = new List<string> { ".png" };
            files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                 .Where(s => ext.Contains(Path.GetExtension(s))).ToArray<string>();

            status = "Directory to Load From: " + dir + "\nPress 'Enter' to load all images";

            execute = true;
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (files.Length > 0)
                    execute = true;
                else
                {
                    errorText = "ERROR: NO *.PNG FILES IN DIRECTORY";
                    progressBarColor = Color.Red;
                }
            }
            if (execute)
            {

                // TODO: Add your update logic here
                if (currentLoadIndex < files.Length)
                {
                    SpriteGenState = SpriteGenState.LoadingTextures;

                    var txtr = TextureUtil.LoadTexture(GraphicsDevice, files[currentLoadIndex]);
                    SpriteSheet.Sprites.Add(new TextureSpriteInfo(txtr, files[currentLoadIndex]));

                    currentLoadIndex++;
                    status = dir;
                    status += string.Format("\nLoading {0} of {1} Images", currentLoadIndex, files.Length);
                    progressBarPercent = ((float)currentLoadIndex / files.Length);
                }
                else if(currentLoadIndex >= files.Length)
                {
                    // if this is the first time then sort the list by width 
                    if(SpriteGenState == SpriteGenState.LoadingTextures)
                    {
                        progressBarPercent = 0.25f;
                          SpriteGenState = SpriteGenState.LayingOutTextures;

                        SpriteSheet.Sort();
                    }
                    else if (SpriteGenState == SpriteGenState.LayingOutTextures)
                    {
                        progressBarPercent = 0.5f;
                        Rectangle bounds = SpriteSheet.Sheet.Bounds;

                        Vector2 point = Vector2.Zero;

                        int lineHeight = 0;
                        foreach (var sprite in SpriteSheet.Sprites)
                        {
                            sprite.Position = point;

                            // now move the running point over the width of this texture
                            point += Vector2.UnitX * sprite.Width;

                            // check if the line height can move down
                            lineHeight = Math.Max(lineHeight, sprite.Height);

                            // if we've passed a line, then drop down
                            if (point.X > bounds.Width)
                            {
                                point.X = 0;
                                point += Vector2.UnitY * lineHeight;
                                lineHeight = 0;
                            }

                            if (point.Y > SpriteSheet.Sheet.Height)
                            {
                                errorText = "ERROR: SPRITE SHEET IS NOT LARGE ENOUGH TO CONTAIN ALL TEXTURES IN DIRECTORY";
                                progressBarColor = Color.Red;
                                break;
                            }
                        }
                        SpriteGenState = SpriteGenState.GeneratingSpriteSheet;
                    }
                    else if(SpriteGenState == SpriteGenState.GeneratingSpriteSheet)
                    {
                        progressBarPercent = 0.75f;
                        SpriteSheet.GenerateSpriteSheet(spriteBatch);
                        SpriteGenState = SpriteGenState.Finished;

                    }
                }
            }
            base.Update(gameTime);
        }




        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            runningHeight = 10;
            int c = 40;
            GraphicsDevice.Clear(new Color(c, c, c, 255));

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            #region Texture Display

            Rectangle bounds = GraphicsDevice.Viewport.Bounds;

            Vector2 point = Vector2.Zero;

            int lineHeight = 0;

            if (SpriteGenState == SpriteGenState.Finished)
            {
                spriteBatch.Draw(SpriteSheet.Sheet, new Vector2(0, 100), Color.White);
                if(progressBarPercent < 1.0f)
                {
                    SpriteSheet.SaveSpriteSheet();
                    SpriteSheet.SaveJSON();
                    SpriteSheet.SaveEnumKeys();
                    progressBarPercent = 1.0f;
                    Process.Start(SpriteSheet.SaveDir);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }
            else
            {
                foreach (var texture in SpriteSheet.Sprites)
                {
                    if (SpriteGenState == SpriteGenState.LayingOutTextures ||
                        SpriteGenState == SpriteGenState.GeneratingSpriteSheet)
                    {
                        spriteBatch.Draw(texture.Sprite, texture.Position, Color.White * 0.5f);
                    }
                    else
                    {
                        // draw the texture
                        spriteBatch.Draw(texture.Sprite, point, Color.White * 0.5f);

                        // now move the running point over the width of this texture
                        point += Vector2.UnitX * texture.Width;

                        // check if the line height can move down
                        lineHeight = Math.Max(lineHeight, texture.Height);

                        // if we've passed a line, then drop down
                        if (point.X > bounds.Width)
                        {
                            point.X = 0;
                            point += Vector2.UnitY * lineHeight;
                            lineHeight = 0;
                        }
                    }
                }
            }

            #endregion


            DrawLine(titleText);
            DrawLine("");
            DrawLine("");
            DrawLine(status);


            #region -- Progress Bar --

            Rectangle fullRect = new Rectangle(50, GraphicsDevice.Viewport.Height - 50,
                (GraphicsDevice.Viewport.Width - 100), 8);
            Rectangle statusRect = new Rectangle(50, GraphicsDevice.Viewport.Height - 50,
                (int)((GraphicsDevice.Viewport.Width - 100) * progressBarPercent), 8);


            DrawString("Status: " + SpriteGenState.ToString().ToSentance(), new Vector2(fullRect.Location.X, fullRect.Location.Y - Font.LineSpacing));
            spriteBatch.Draw(TextureUtil.DefaultTexture, fullRect, Color.Black);
            fullRect.Inflate(-1, -1);
            spriteBatch.Draw(TextureUtil.DefaultTexture, fullRect, Color.DimGray);
            statusRect.Inflate(-1, -1);
            spriteBatch.Draw(TextureUtil.DefaultTexture, statusRect, progressBarColor);

            #endregion


            DrawErrors();

            spriteBatch.End();

            base.Draw(gameTime);
        }

        Color progressBarColor = Color.DeepSkyBlue;

        int runningHeight = 10;
        void DrawLine(string text)
        {
            DrawLine(text, Color.White);
        }
        void DrawLine(string text, Color color)
        {
            DrawString(text, new Vector2(16, runningHeight), color);
            runningHeight += Font.LineSpacing;
        }

        string errorText = "";
        void DrawErrors()
        {
            runningHeight += Font.LineSpacing;
            DrawString(errorText, new Vector2(16, runningHeight), Color.Red);
        }

        void DrawString(string text, Vector2 position)
        {
            DrawString(text, position, Color.White);
        }

        void DrawString(string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(Font, text, position + Vector2.One, Color.Black);
            spriteBatch.DrawString(Font, text, position, color);
        }
    }
}
