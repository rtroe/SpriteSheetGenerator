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
    public class TextureSpriteInfo
    {

        public string Name
        {
            get { return _sprite.Name; }
        }

        /// <summary>
        /// This is the position of the Texture Sprite
        /// </summary>
        [JsonIgnore]
        public Vector2 Position { get; set; }

        [JsonIgnore]
        public Texture2D Sprite
        {
            get { return _sprite; }
        }
        Texture2D _sprite;

        [JsonIgnore]
        public int Width
        {
            get { return _sprite.Width; }
        }

        [JsonIgnore]
        public int Height
        {
            get { return _sprite.Height; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, Width, Height); }
        }

        public TextureSpriteInfo(Texture2D texture, string path)
        {
            _sprite = texture;
            _sprite.Name = Path.GetFileNameWithoutExtension(path);
            _sprite.Tag = path;
        }
    }
}
