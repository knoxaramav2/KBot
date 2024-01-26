using System;
using System.IO;

namespace KBot.Util
{
    public static class UFile
    {
        public static string GameDataDir { get => Path.Join(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName, "GameData"); }
        public static string PartsDir { get => Path.Join(GameDataDir, "Parts"); }
    }
}
