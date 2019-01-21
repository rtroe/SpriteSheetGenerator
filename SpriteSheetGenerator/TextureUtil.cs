using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpriteSheetGenerator
{
    public static class TextureUtil
    {
        public static Texture2D DefaultTexture;

        /// <summary>
        /// Creates a Blank Texture
        /// </summary>
        /// <param name="device"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="paint"></param>
        /// <returns></returns>
        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            Texture2D texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (int pixel = 0; pixel < data.Count(); pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }

        public static Texture2D LoadTexture(GraphicsDevice device, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                return Texture2D.FromStream(device, fileStream);

            }
        }
        public static void SaveToDisk(Texture2D texture, string FileName)
        {
            // Stream
            System.IO.Stream stream = System.IO.File.Create(FileName);

            // Save the Texture
            texture.SaveAsPng(stream, texture.Width, texture.Height);

            stream.Flush();
            stream.Close();
            // Dispose
            stream.Dispose();
        }

        public static string ToSentance(this string source)
        {
            var words = System.Text.RegularExpressions.Regex.Split(source, @"(?<!^)(?=[A-Z])");

            string result = words[0];

            for (int i = 1; i < words.Length; i++)
                result += " " + words[i];

            return result;
        }
    }
}
