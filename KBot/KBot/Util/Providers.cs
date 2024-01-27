using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var exts = new[] { ".png", ".jpg", ".jpeg" };
            StandardImages = Directory.GetFiles(UFile.StdAssetDir)
                .Where(x => exts.Any(y => x.EndsWith(y, System.StringComparison.OrdinalIgnoreCase)))
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToList();

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
        public static Images Sprites { get; set; }

        public static void Init(
            GraphicsDeviceManager graphics, 
            SpriteBatch batch,
            ContentManager content)
        {
            Graphics = graphics;
            DrawCtx = batch;
            Fonts = new Fonts(content);
            Sprites = new Images(content);
        }
    }
}
