using KBot.Util;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KBot.State
{
    public class GameState
    {
        private static GameState __State = null;
        public static GameState State { 
            get { return __State ??= new(); } 
            set { __State = value; } 
        }

        public PlayerState Player { get; set; }

        public Controllable Avatar { get; set; }

        public string SaveName { get; set; }

        private GameState() {
            SaveName = string.Empty;
            Player = new();
            Avatar = null;
        }

        private GameState(string gameName, string playerName) 
        {
            SaveName = gameName;
            Player = new PlayerState()
            {
                Name = playerName,
                BotInventory = { Depots.FabDepot.Depots.Get("DevBot") }
            };
        }

        public static GameState NewGame(string gameName, string playerName)
        {
            var ngame = Load(gameName, true) ?? new GameState(gameName, playerName);
            return ngame;
        }

        public static GameState Load(string gameName)
        {
            return Load(gameName, false);
        }

        private void RectifyState()
        {
            foreach(var part in Player.PartInventory)
            {
                part.RectifyLoadState();
            }

            foreach(var bot in Player.BotInventory)
            {
                bot.Base?.RectifyLoadState();
            }
        }

        private static GameState Load(string gameName, bool fromTemplate)
        {
            gameName = Path.HasExtension(gameName) ? gameName : gameName + ".sav";
            var trgPath = fromTemplate ? UFile.TemplateDir : UFile.SavesDir;
            var path = Path.Combine(trgPath, gameName);

            if (!File.Exists(path))
            {
                Debug.WriteLine($"ERR: Could not find {gameName}");
                return null;
            }

            var raw = File.ReadAllText(path);

            GameState loadState = JsonConvert.DeserializeObject<GameState>(raw);
            loadState.RectifyState();
            if (loadState == null)
            {
                Debug.WriteLine($"ERR: Failed to load {gameName}");
                return null;
            }

            return loadState;
        }

        public void Save()
        {
            Save(false);
        }

        private void Save(bool fromTemplate=false)
        {
            var saveName = SaveName + ".sav";
            var trgPath = fromTemplate ? UFile.TemplateDir : UFile.SavesDir;
            var path = Path.Combine(trgPath, saveName);
            var json = JsonConvert.SerializeObject(this, 
                Formatting.Indented, new JsonSerializerSettings() 
                { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            using StreamWriter writer = new(path);
            writer.Write(json);
        }
    }
}
