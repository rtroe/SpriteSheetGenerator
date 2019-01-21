using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteSheetGenerator
{
    /// <summary>
    /// This class holds Texture Sprite Sheet Info such as Width, Height, Position and
    /// the Texture Data Its Self
    /// </summary>
    public class SpriteSheet
    {
        [JsonIgnore]
        public string SaveDir { get; private set; }
        
        public int Width { get; private set; }

        public int Height { get; private set; }

        public int SpriteCount
        {
            get { return Sprites.Count; }
        }

        public List<TextureSpriteInfo> Sprites { get; private set; } 

        [JsonIgnore]
        public RenderTarget2D Sheet;

        [JsonIgnore]
        GraphicsDevice GraphicsDevice;

        public SpriteSheet(GraphicsDevice graphicsDevice, string SaveDir, int Width, int Height)
        {
            this.SaveDir = SaveDir;

            if (Directory.Exists(this.SaveDir) == false)
                Directory.CreateDirectory(this.SaveDir);

            this.Width = Width;
            this.Height = Height;
            this.GraphicsDevice = graphicsDevice;
            Sheet = new RenderTarget2D(graphicsDevice, Width, Height);
            Sprites = new List<TextureSpriteInfo>();
        }

        public void Sort()
        {
            Sprites = Sprites.OrderByDescending(o => o.Width).ToList();
        }

        /// <summary>
        /// Generates a Sprite Sheet
        /// </summary>
        public void GenerateSpriteSheet(SpriteBatch spriteBatch)
        {
            GraphicsDevice.SetRenderTarget(Sheet);
            GraphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            foreach (var texture in Sprites)
            {
                spriteBatch.Draw(texture.Sprite, texture.Position, Color.White);
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);
        }

        public void SaveSpriteSheet()
        {
            Console.Write("Saving Sprite Sheet...");
            TextureUtil.SaveToDisk(Sheet, Path.Combine(SaveDir, "spritesheet.png"));
            Console.WriteLine("Done!");
        }


        public void SaveJSON()
        {
            Console.Write("Saving Json...");
            // Now save the json file
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            StreamWriter jsonWriter = new StreamWriter(Path.Combine(SaveDir, "spritesheet.json"));
            jsonWriter.Write(json);
            jsonWriter.Flush();
            jsonWriter.Close();
            Console.WriteLine("Done!");
        }

        public void SaveEnumKeys()
        {
            Console.Write("Saving enums...");
            StreamWriter enumWriter = new StreamWriter(Path.Combine(SaveDir, "spritesheetkeys.cs"));

            enumWriter.WriteLine("/*");
            enumWriter.WriteLine("\tAuto Generated enum Keys By Sprite Gen v. " + SpriteSheetGenerator.Version);
            enumWriter.WriteLine("\t---------------------------------------------------------");
            enumWriter.WriteLine("\n\tUse this enum to retrieve sprite sheet texture locations from the accompanying spritesheet.json file.");
            enumWriter.WriteLine("\n\tGenerated On Date: " + DateTime.Now.ToString());
            enumWriter.WriteLine("\tSprite Sheet Size: " + Width + "x" + Height);
            enumWriter.WriteLine("\tSprite Count: " + Sprites.Count);
            enumWriter.WriteLine("\n\tSprite Gen - A simple sprite sheet gen tool for C# game engines");
            enumWriter.WriteLine("\tBy: R.T.Roe");
            enumWriter.WriteLine("\thttps://github.com/rtroe");

            enumWriter.WriteLine("*/\n");
            enumWriter.WriteLine("namespace SpriteSheetCreator");
            enumWriter.WriteLine("{");
            enumWriter.WriteLine("\tpublic enum SpriteSheetKeys");
            enumWriter.WriteLine("\t{");
            
            foreach(var sprite in Sprites)
                enumWriter.WriteLine("     " + sprite.Name +",");

            enumWriter.WriteLine("\t}");
            enumWriter.WriteLine("}");
            enumWriter.WriteLine("// End of auto generated enum list");
            enumWriter.Flush();
            enumWriter.Close();
            Console.WriteLine("Done!");
        }
    }
}
