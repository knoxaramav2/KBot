using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Util
{
    public class Fonts
    {
        private Dictionary<string, SpriteFont> __fonts;
        public Fonts(Microsoft.Xna.Framework.Content.ContentManager content) {
            __fonts = new Dictionary<string, SpriteFont>
            {
                ["Default"] = content.Load<SpriteFont>("Fonts")
            };
        }

        public SpriteFont Get(string name = "Default")
        {
            return __fonts[name];
        }
    }

    public class Images
    {
        private Dictionary<string, Texture2D> Storage;
        private List<string> StandardImages;

        public Texture2D Get(string name)
        {
            if (Storage.TryGetValue(name.ToUpper(), out Texture2D ret))
            {
                return ret;
            }

            Storage.TryGetValue("CROSS", out ret);
            return ret;
        }

        public Images(ContentManager content)
        {
            StandardImages = new List<string>
            {
                "Cross", "Box1", "DevShell1"
            };

            Storage = new();

            foreach(var img in StandardImages)
            {
                Storage.Add(img.ToUpper(), content.Load<Texture2D>(img));
            }
        }
    }

    public static class Providers
    {
        public static GraphicsDeviceManager Graphics { get; set; } = null;
        public static SpriteBatch DrawCtx { get; set; }
        public static Fonts Fonts { get; set; }
        public static Images Images { get; set; }

        public static void Init(
            GraphicsDeviceManager graphics, 
            SpriteBatch batch,
            ContentManager content)
        {
            Graphics = graphics;
            DrawCtx = batch;
            Fonts = new Fonts(content);
            Images = new Images(content);
        }
    }
}
